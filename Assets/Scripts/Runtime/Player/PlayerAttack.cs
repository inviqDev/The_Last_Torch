using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private EnemiesCollector collector;
        [SerializeField] private AbilityConfig[] abilityConfigs;
        
        private Player _player;
        
        private List<Ability> _availableAbilities;
        private int _nextAbilityIndex;
        
        private List<Ability> _activeAbilities;
        private EnemyModel closestEnemy;

        public void Init(Player player)
        {
            _player = player;
            
            _availableAbilities = new List<Ability>();
            _nextAbilityIndex = 0;
            
            foreach (var a in abilityConfigs)
            {
                var ability = new Ability(a);
                _availableAbilities.Add(ability);
            }
            
            _activeAbilities = new List<Ability>();
            
            _player.OnPlayerLevelChanged += OnPlayerLevelChanged;
            _player.OnCharacterDeath += OnCharacterDeath;
        }

        private void OnPlayerLevelChanged(Player player)
        {
            ActivateNextAbility();
            
            if (player.CurrentLevel == 1) return;
            OnLevelChangedUpdateAbilitiesStats(); // (10f, 1.25f);
        }

        private void OnCharacterDeath(Character player)
        {
            _player.OnCharacterDeath -= OnCharacterDeath;
            // _player.OnNextAbilityIsAvailable -= OnNextAbilityIsAvailable;
            _player.OnPlayerLevelChanged -= OnPlayerLevelChanged;
            
            enabled = false;
        }

        private void Update()
        {
            if (!collector.EnemyExists) return;

            foreach (var ability in _activeAbilities)
            {
                if (ability.State != Ability.AbilityState.Ready) continue;

                closestEnemy = collector.GetClosestEnemyFromList(out var distanceToClosestEnemy);
                if (!closestEnemy || distanceToClosestEnemy > ability.MinAttackDistance) continue;

                var abilityVFX = Pool.Instance?.TryGetObjectFromPool(ability.AbilityVFX);
                if (!abilityVFX)
                {
                    return;
                }
                
                void OnFinished(Ability a)
                {
                    abilityVFX.Finished -= OnFinished;
                    
                    Pool.Instance?.ReturnToPool(abilityVFX);
                    a.SetAbilityState(Ability.AbilityState.OnCooldown);
                }
                
                abilityVFX.Finished += OnFinished;

                // 3) Контекст — общая точка расширения для любых VFX
                var ctx = new AbilityContext(
                    player: GameManager.Instance.Player,
                    initialTarget: closestEnemy,
                    enemiesCollector: collector,
                    ability: ability
                );

                ability.SetAbilityState(Ability.AbilityState.InProgress);
                abilityVFX.Play(ctx);
                break;
            }
        }
        
        private void OnNextAbilityIsAvailable()
        {
            ActivateNextAbility();
        }

        private void ActivateNextAbility()
        {
            MyAssertions.EnsureIsTrue(_nextAbilityIndex == _availableAbilities.Count);
            if (_nextAbilityIndex == _availableAbilities.Count) return;
            
            var ability = _availableAbilities[_nextAbilityIndex];
            var slot = GameManager.Instance?.UIManager.GetAvailableAbilitySlot();

            if (!slot)
            {
                UnityEngine.Assertions.Assert.IsNotNull(slot, $"slot for {ability.Name} is not found");
                return;
            }
            
            ability.ActivateAbility(this, slot);
            slot.UpdateAbilityUI(ability);
            
            _availableAbilities.Remove(ability);
            _activeAbilities.Add(ability);
        }

        private void OnLevelChangedUpdateAbilitiesStats()
        {
            foreach (var a in _activeAbilities)
            {
                if (a.AbilityVFX is LightningChain chain)
                {
                    chain.IncreaseBouncesAmount();
                }
                
                a.UpdateAbility(5f, 1.05f);
            }

            foreach (var a in _availableAbilities)
            {
                if (a.AbilityVFX is LightningChain chain)
                {
                    chain.IncreaseBouncesAmount();
                }
                
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
    }
}