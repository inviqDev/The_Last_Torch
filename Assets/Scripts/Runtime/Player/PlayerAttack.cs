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

                var enemy = collector.GetClosestEnemyFromList();
                if (!enemy) continue;

                // 1) Способность уходит в InProgress
                ability.SetAbilityState(Ability.AbilityState.InProgress);

                // 2) Инстансим VFX (без кастов)
                var vfx = Instantiate(ability.AbilityVFX); // тип: AbilityVFX
                void OnFinished(Ability a)
                {
                    vfx.Finished -= OnFinished;
                    a.SetAbilityState(Ability.AbilityState.OnCooldown);
                }
                vfx.Finished += OnFinished;

                // 3) Контекст — общая точка расширения для любых VFX
                var ctx = new AbilityContext(
                    player: GameManager.Instance.Player,
                    initialTarget: enemy,
                    enemiesCollector: collector,
                    ability: ability
                );

                vfx.Play(ctx);

                // не триггерим другие абилки в этот же кадр
                break;
            }
        }
    }
}