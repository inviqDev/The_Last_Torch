using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class PlayerAttack : MonoBehaviour
    {
        [Header("Find closest enemy settings")] [SerializeField]
        private LayerMask enemyLayerMask;

        [SerializeField] private float overlapRadius = 14f;

        [SerializeField] private AttackableEnemiesCollector collector;

        private List<Ability> autoAttackAbilities;

        private readonly Collider[] overlapColliders = new Collider[128];
        private Transform closestEnemy;

        private void Awake()
        {
            autoAttackAbilities = new List<Ability>();
        }

        public void AddNewAbility(Ability ability)
        {
            autoAttackAbilities.Add(ability);
        }

        private void Update()
        {
            if (!collector.EnemyExists) return;

            foreach (var a in autoAttackAbilities)
            {
                if (a.State != Ability.AbilityState.Ready) continue;

                var enemy = collector.GetClosestEnemyFromList();
                // var vfx = Instantiate(a.AbilityVFX, enemy.transform.position, Quaternion.identity);
                enemy.TakeDamage(a.Damage);
                a.SetAbilityState(Ability.AbilityState.OnCooldown);
                
                if (enemy.CurrentHealth <= 0) return;
            }
        }

        private EnemyModel GetClosestEnemyPhysicsOverlap()
        {
            var count = Physics.OverlapSphereNonAlloc(
                transform.position,
                overlapRadius,
                overlapColliders,
                enemyLayerMask);

            if (count == 0) return null;

            var minSqrMag = float.MaxValue;
            for (var i = 0; i < count; i++)
            {
                var currentCol = overlapColliders[i];
                if (!currentCol) continue;

                var currentSqrMag = (transform.position - currentCol.transform.position).sqrMagnitude;
                if (currentSqrMag < minSqrMag)
                {
                    minSqrMag = currentSqrMag;
                    closestEnemy = currentCol.transform;
                }
            }

            return closestEnemy.root.TryGetComponent(out EnemyModel enemy) ? enemy : null;
        }
    }
}