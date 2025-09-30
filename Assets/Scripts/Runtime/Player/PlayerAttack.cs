using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private AbilityConfig[] abilityConfigs;

        private Player _player;
        private EnemiesDetector _detector;

        // Master registry of all created abilities (for unsubscribing)
        private readonly List<Ability> _allCreatedAbilities = new();

        // Abilities that are not yet unlocked (activated on level up)
        private List<Ability> _allAbilities;

        // Queue of ready-to-use abilities + HashSet for O(1) "already in queue?" check
        private Queue<Ability> _readyQueue;
        private HashSet<Ability> _readySet;

        private Enemy _closestEnemy;
        private bool _isInitialized;

        public void Init(Player player)
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _player ??= player;
            _detector ??= _player.Detector;

            _allAbilities ??= new List<Ability>();
            _allAbilities.Clear();

            // Create all abilities once, subscribe, and put into two lists:
            // - _allAbilities: not yet activated (to unlock in order)
            // - _allCreatedAbilities: master registry for unsubscribing later
            foreach (var cfg in abilityConfigs)
            {
                var ability = new Ability(cfg);
                ability.OnAbilityReady += AddAbilityToReadyList;

                _allAbilities.Add(ability);
                _allCreatedAbilities.Add(ability);
            }

            _readyQueue ??= new Queue<Ability>();
            _readyQueue.Clear();

            _readySet ??= new HashSet<Ability>();
            _readySet.Clear();

            _detector.OnEnemyDetected += RecomputeEnabled;
            _detector.OnEnemiesListIsEmpty += RecomputeEnabled;

            _player.OnPlayerLevelChanged += OnPlayerLevelChanged;
            _player.OnCharacterDeath += OnCharacterDeath;

            RecomputeEnabled();
        }

        private void AddAbilityToReadyList(Ability a)
        {
            UnityEngine.Assertions.Assert.IsTrue(a.State == AbilityState.Ready,
                $"Ability {a.Name} enqueued while not Ready");

            // HashSet guarantees uniqueness
            if (_readySet.Add(a))
                _readyQueue.Enqueue(a);

            RecomputeEnabled();
        }

        private void RecomputeEnabled()
        {
            enabled = _detector.EnemyExists && _readyQueue.Count > 0;
        }

        private void Update()
        {
            
#if UNITY_EDITOR
            var peek = _readyQueue.Peek();
            print(peek.Name);
#endif
            
            if (!_detector.EnemyExists || _readyQueue.Count == 0) return;

            // Loop at most queue length (no infinite loop)
            var spins = _readyQueue.Count;
            while (spins-- > 0)
            {
                var ability = _readyQueue.Peek();

                _closestEnemy = _detector.GetClosestEnemy(out var distance);
                if (!_closestEnemy || distance > ability.MinAttackDistance)
                {
                    // If check failed => rotate head to tail, try next
                    _readyQueue.Enqueue(_readyQueue.Dequeue());
                    continue;
                }

                var vfx = Pool.Instance?.TryGet(ability.AbilityVFX);
                UnityEngine.Assertions.Assert.IsNotNull(vfx, "ability VFX is missing");
                if (!vfx)
                {
                    // If check failed => rotate head to tail, try next
                    _readyQueue.Enqueue(_readyQueue.Dequeue());
                    continue;
                }

                // Ready to start: remove from queue and from HashSet
                _readyQueue.Dequeue();
                _readySet.Remove(ability);

                vfx.Finished += OnFinished;

                var ctx = new AbilityContext(
                    player: GameManager.Instance?.Player,
                    initialTarget: _closestEnemy,
                    enemiesDetector: _detector,
                    ability: ability
                );

                ability.SetAbilityState(AbilityState.InProgress);
                vfx.Play(ctx);

                RecomputeEnabled();
                return; // only one ability per tick

                void OnFinished(Ability a)
                {
                    vfx.Finished -= OnFinished;

                    a.SetAbilityState(AbilityState.OnCooldown);
                    Pool.Instance?.ReturnToPool(vfx);

                    RecomputeEnabled();
                }
            }

            RecomputeEnabled();
        }


        private void ActivateNextAbility()
        {
            if (_allAbilities == null || _allAbilities.Count == 0) return;

            // Take the first "not yet unlocked" // Add Random ??
            var ability = _allAbilities[0];
            var slot = GameManager.Instance?.UIManager.GetAvailableAbilitySlot();
            if (!slot)
            {
                UnityEngine.Assertions.Assert.IsNotNull(slot, $"slot for {ability.Name} is not found");
                return;
            }

            ability.ActivateAbility(this);
            slot.SetAbilitySlotUI(ability);

            // Remove from "not yet unlocked" list
            _allAbilities.RemoveAt(0);
        }

        private void OnPlayerLevelChanged(Player player)
        {
            ActivateNextAbility();

            if (player.CurrentLevel == 1) return;
            OnLevelChangedUpdateAbilitiesStats();
        }

        private void OnLevelChangedUpdateAbilitiesStats()
        {
            foreach (var a in _allAbilities)
            {
                if (a.AbilityVFX is LightningChain chain)
                    chain.IncreaseBouncesAmount();

                a.ChangeAbilityDamage(StatChangeMode.Percent, 5f);
                a.ChangeAbilityCooldown(StatChangeMode.Percent, -5f);
            }

            foreach (var a in _readyQueue)
            {
                if (a.AbilityVFX is LightningChain chain)
                    chain.IncreaseBouncesAmount();

                a.ChangeAbilityDamage(StatChangeMode.Percent, 5f);
                a.ChangeAbilityCooldown(StatChangeMode.Percent, -5f);
            }
        }

        public void ChangeAbilitiesStats(float damageIncrement)
        {
            foreach (var a in _allAbilities)
            {
                a.ChangeAbilityDamage(StatChangeMode.SimpleAdd, damageIncrement);
                a.ChangeAbilityCooldown(StatChangeMode.SimpleAdd, -0.005f);
            }

            foreach (var a in _readyQueue)
            {
                a.ChangeAbilityDamage(StatChangeMode.SimpleAdd, damageIncrement);
                a.ChangeAbilityCooldown(StatChangeMode.SimpleAdd, -0.005f);
            }
        }

        private void UnsubscribeComponents()
        {
            if (_detector)
            {
                _detector.OnEnemyDetected -= RecomputeEnabled;
                _detector.OnEnemiesListIsEmpty -= RecomputeEnabled;
            }

            if (_player)
            {
                _player.OnPlayerLevelChanged -= OnPlayerLevelChanged;
                _player.OnCharacterDeath -= OnCharacterDeath;
            }
        }

        private void UnsubscribeAbilities()
        {
            foreach (var a in _allCreatedAbilities)
                a.OnAbilityReady -= AddAbilityToReadyList;
        }

        private void OnCharacterDeath(Character player)
        {
            _player.OnCharacterDeath -= OnCharacterDeath;
            _player.OnPlayerLevelChanged -= OnPlayerLevelChanged;

            enabled = false;
        }

        private void OnDestroy()
        {
            UnsubscribeComponents();
            UnsubscribeAbilities();

            _allAbilities?.Clear();
            _readyQueue?.Clear();
            _readySet?.Clear();
            _allCreatedAbilities?.Clear();
        }
    }
}