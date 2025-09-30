using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private AbilityConfig[] abilityConfigs;
        
        private Player _player;
        private EnemiesDetector _detector;
        
        private List<Ability> _availableAbilities;
        private int _nextAbilityIndex;
        
        private List<Ability> _activeAbilities;
        private Enemy closestEnemy;
        
        private bool _isInitialized;

        public void Init(Player player)
        {
            if (_isInitialized) return;
            _isInitialized = true;
            
            _player ??= player;
            _detector ??= _player.Detector;
            enabled = _detector.EnemyExists;
            
            _availableAbilities ??= new List<Ability>();
            _availableAbilities.Clear();
            _nextAbilityIndex = 0;
            
            foreach (var a in abilityConfigs)
            {
                var ability = new Ability(a);
                _availableAbilities.Add(ability);
            }
            
            _activeAbilities ??= new List<Ability>();
            _activeAbilities.Clear();

            _detector.OnEnemyDetected += ActivateUpdate;
            _detector.OnEnemiesListIsEmpty += DeactivateUpdate;

            _player.OnPlayerLevelChanged += OnPlayerLevelChanged;
            _player.OnCharacterDeath += OnCharacterDeath;
        }

        private void Update()
        {
            if (!_detector.EnemyExists) return;

            for (var i = 0; i < _activeAbilities.Count; i++)
            {
                var ability = _activeAbilities[i];
                if (ability.State != AbilityState.Ready) continue;

                closestEnemy = _detector.GetClosestEnemy(out var distanceToEnemy);
                if (!closestEnemy || distanceToEnemy > ability.MinAttackDistance) continue;

                var vfx = Pool.Instance?.TryGet(ability.AbilityVFX);
                UnityEngine.Assertions.Assert.IsNotNull(vfx, "ability VFX is missing");
                if (!vfx) continue;

                vfx.Finished += OnFinished;

                var context = new AbilityContext(
                    player: GameManager.Instance?.Player,
                    initialTarget: closestEnemy,
                    enemiesDetector: _detector,
                    ability: ability
                );

                ability.SetAbilityState(AbilityState.InProgress);
                vfx.Play(context);
                break;

                void OnFinished(Ability a)
                {
                    vfx.Finished -= OnFinished;

                    a.SetAbilityState(AbilityState.OnCooldown);
                    Pool.Instance?.ReturnToPool(vfx);
                }
            }
        }

        private void ActivateNextAbility()
        {
            UnityEngine.Assertions.Assert.IsTrue(
                _nextAbilityIndex < _availableAbilities.Count, 
                "index is out of \"_availableAbilities\" range");
            if (_nextAbilityIndex == _availableAbilities.Count) return;
            
            var ability = _availableAbilities[_nextAbilityIndex];
            var slot = GameManager.Instance?.UIManager.GetAvailableAbilitySlot();

            if (!slot)
            {
                UnityEngine.Assertions.Assert.IsNotNull(
                    slot, $"slot for {ability.Name} is not found");
                return;
            }
            
            
            ability.ActivateAbility(this, slot);
            slot.ActivateAbilityUI(ability);
            
            _availableAbilities.Remove(ability);
            _activeAbilities.Add(ability);
        }
        
        private void ActivateUpdate()
        {
            enabled = true;
        }
        
        private void DeactivateUpdate()
        {
            enabled = false;
        }
        
        private void OnPlayerLevelChanged(Player player)
        {
            ActivateNextAbility();
            
            if (player.CurrentLevel == 1) return;
            OnLevelChangedUpdateAbilitiesStats();
        }

        private void OnCharacterDeath(Character player)
        {
            _player.OnCharacterDeath -= OnCharacterDeath;
            _player.OnPlayerLevelChanged -= OnPlayerLevelChanged;
            
            enabled = false;
        }

        private void OnLevelChangedUpdateAbilitiesStats()
        {
            foreach (var a in _activeAbilities)
            {
                if (a.AbilityVFX is LightningChain chain)
                    chain.IncreaseBouncesAmount();
                
                a.UpdateAbility(5f, 1.05f);
            }

            foreach (var a in _availableAbilities)
            {
                if (a.AbilityVFX is LightningChain chain)
                    chain.IncreaseBouncesAmount();
                
                a.UpdateAbility(5f, 1.05f);
            }
        }
        
        public void UpdateAbilitiesStats(float damageIncrement, float attackSpeedDivider = 1f)
        {
            foreach (var a in _activeAbilities)
            {
                a.UpdateAbility(damageIncrement, attackSpeedDivider);
            }

            foreach (var a in _availableAbilities)
            {
                a.UpdateAbility(damageIncrement, attackSpeedDivider);
            }
        }
        
        private void UnsubscribeAll()
        {
            if (_detector)
            {
                _detector.OnEnemyDetected -= ActivateUpdate;
                _detector.OnEnemiesListIsEmpty -= DeactivateUpdate;
            }

            if (_player)
            {
                _player.OnPlayerLevelChanged -= OnPlayerLevelChanged;
                _player.OnCharacterDeath -= OnCharacterDeath;
            }
        }

        private void OnDestroy() => UnsubscribeAll();
    }
}