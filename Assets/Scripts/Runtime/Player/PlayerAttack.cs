using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private EnemiesCollector collector;

        private List<Ability> activeAutoAttackAbilities;
        private EnemyModel closestEnemy;

        private void Awake()
        {
            UnityEngine.Assertions.Assert.IsNotNull(GameManager.Instance, "GameManager is not found");
            activeAutoAttackAbilities = new List<Ability>();
        }

        public void AddNewAbility(Ability ability)
        {
            activeAutoAttackAbilities.Add(ability);
        }

        private void Update()
        {
            if (!collector.EnemyExists) return;

            foreach (var ability in activeAutoAttackAbilities)
            {
                if (ability.State != Ability.AbilityState.Ready) continue;

                closestEnemy = collector.GetClosestEnemyFromList(out var distanceToClosestEnemy);
                if (!closestEnemy || distanceToClosestEnemy > ability.MinAttackDistance) continue;

                ability.SetAbilityState(Ability.AbilityState.InProgress);

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

                abilityVFX.Play(ctx);
                break;
            }
        }
    }
}