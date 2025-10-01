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
        private List<Ability> _availableAbilities;

        // Queue of ready-to-use abilities + HashSet for O(1) "already in queue?" check
        private Queue<Ability> _readyQueue;
        private HashSet<Ability> _readySet;

        private Enemy _closestEnemy;
        private bool _isInitialized;

        public void Init(Player player)
        {
            if (_isInitialized) return;

            InitializeFields(player);

            // Create all abilities once, subscribe, and put into two lists:
            // - allCreatedAbilities: master registry for unsubscribing later
            // - availableAbilities: not yet activated (to unlock in order)
            foreach (var cfg in abilityConfigs)
            {
                var ability = new Ability(cfg);
                ability.OnAbilityReady += AddAbilityToReadyList;

                _allCreatedAbilities?.Add(ability);
                _availableAbilities?.Add(ability);
            }

            SubscribeOnComponentsEvents();
            RecomputeEnabled();
        }

        private void Update()
        {
            
#if UNITY_EDITOR
            // var peek = _readyQueue.Peek();
            // print(peek.Name);
#endif
            
            if (!_detector.EnemyExists || _readyQueue.Count == 0) return;

            // Loop at most queue length (no infinite loop)
            var spins = _readyQueue.Count;
            while (spins-- > 0)
            {
                var ability = _readyQueue.Peek();
                if (ValidateAbilityIsAbleToAttackThisFrame(ability) == false) continue;
                if (ValidateAbilityIsAbleToLaunch(ability, out var visual) == false) continue;

                // Ready to start: remove from queue and from HashSet
                _readyQueue.Dequeue();
                _readySet.Remove(ability);

                visual.Finished += OnFinished;
                var context = new AbilityContext(
                    player: GameManager.Instance?.Player,
                    initialTarget: _closestEnemy,
                    ability: ability
                );

                ability.SetAbilityState(AbilityState.InProgress);
                visual.LaunchAbilityVFX(context);

                RecomputeEnabled();
                return; // only one ability per tick

                void OnFinished(Ability a)
                {
                    visual.Finished -= OnFinished;
                    a.SetAbilityState(AbilityState.OnCooldown);
                    Pool.Instance?.ReturnToPool(visual);

                    RecomputeEnabled();
                }
            }

            RecomputeEnabled();
        }
        
        private void InitializeFields(Player player)
        {
            _player ??= player;
            UnityEngine.Assertions.Assert.IsNotNull(_player, 
                "player component is missing");
            
            _detector ??= _player.Detector;
            UnityEngine.Assertions.Assert.IsNotNull(_detector, 
                "detector component is missing");

            _availableAbilities ??= new List<Ability>();
            _availableAbilities.Clear();
            
            _readyQueue ??= new Queue<Ability>();
            _readyQueue.Clear();

            _readySet ??= new HashSet<Ability>();
            _readySet.Clear();
            
            _isInitialized = true;
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

        private bool ValidateAbilityIsAbleToAttackThisFrame(Ability ability)
        {
            _closestEnemy = _detector.GetClosestEnemy(out var distance);
            if (_closestEnemy && distance <= ability.MinAttackDistance) return true;
            
            // If check failed => rotate head to tail, try next
            _readyQueue.Enqueue(_readyQueue.Dequeue());
            return false;
        }
        
        private bool ValidateAbilityIsAbleToLaunch(Ability ability, out AbilityVFX vfx)
        {
            vfx = Pool.Instance?.TryGet(ability.AbilityVFX);
            UnityEngine.Assertions.Assert.IsNotNull(vfx, "ability VFX is missing");
            if (vfx) return true;
            
            // If check failed => rotate head to tail, try next
            _readyQueue.Enqueue(_readyQueue.Dequeue());
            return false;
        }

        private void ActivateNextAbility()
        {
            if (_availableAbilities == null || _availableAbilities.Count == 0) return;

            // Take the first "not yet unlocked" // Add Random ??
            var ability = _availableAbilities[0];
            var slot = GameManager.Instance?.UIManager.GetAvailableAbilitySlot();
            if (!slot)
            {
                UnityEngine.Assertions.Assert.IsNotNull(slot, 
                    $"slot for {ability.Name} is not found");
                return;
            }

            ability.ActivateAbility(this);
            slot.SetAbilitySlotUI(ability);

            // Remove from "not yet unlocked" list
            _availableAbilities.RemoveAt(0);
        }

        private void OnPlayerLevelChanged(Player player)
        {
            ActivateNextAbility();
            if (player.CurrentLevel == 1) return;
            OnLevelChangedUpdateAbilitiesStats();
        }

        private void OnLevelChangedUpdateAbilitiesStats()
        {
            foreach (var a in _availableAbilities)
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
            foreach (var a in _availableAbilities)
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
        
        private void SubscribeOnComponentsEvents()
        {
            if (_player)
            {
                _player.OnPlayerLevelChanged += OnPlayerLevelChanged;
                _player.OnCharacterDeath += OnCharacterDeath;
            }
            
            if (_detector)
            {
                _detector.OnEnemyDetected += RecomputeEnabled;
                _detector.OnEnemiesListIsEmpty += RecomputeEnabled;
            }
        }

        private void UnsubscribeOnComponentsEvents()
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

        private void OnCharacterDeath(Character player)
        {
            _player.OnCharacterDeath -= OnCharacterDeath;
            _player.OnPlayerLevelChanged -= OnPlayerLevelChanged;

            enabled = false;
        }
        private void UnsubscribeAbilities()
        {
            foreach (var a in _allCreatedAbilities)
                a.OnAbilityReady -= AddAbilityToReadyList;
        }
        
        private void ClearCollections()
        {
            _allCreatedAbilities?.Clear();
            _availableAbilities?.Clear();
            _readyQueue?.Clear();
            _readySet?.Clear();
        }

        private void OnDestroy()
        {
            UnsubscribeOnComponentsEvents();
            UnsubscribeAbilities();
            ClearCollections();
        }
    }
}