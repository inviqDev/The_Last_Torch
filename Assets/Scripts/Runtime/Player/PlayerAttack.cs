using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class PlayerAttack : MonoBehaviour
    {
        [Header("Find closest enemy settings")] 
        [SerializeField] private LayerMask enemyLayerMask;
        [SerializeField] private float overlapRadius = 14f;

        [SerializeField] private AttackableEnemiesCollector collector;
        private readonly Collider[] overlapColliders = new Collider[128];

        private List<Ability> activeAutoAttackAbilities;
        private EnemyModel closestEnemy;

        private void Awake()
        {
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

                closestEnemy = collector.GetClosestEnemyFromList();
                var vfx = Instantiate(ability.AbilityVFX);
                vfx.UseAbility(transform, closestEnemy.transform);
                
                closestEnemy.TakeDamage(ability.Damage);
                ability.SetAbilityState(Ability.AbilityState.OnCooldown);

                if (closestEnemy.CurrentHealth <= 0) return;
            }
        }

        // private void UseAbility(Ability ability, EnemyModel enemy)
        // {
        //     if (ability.AbilityVFX)
        //     {
        //         var vfx = Instantiate(ability.AbilityVFX, transform.position, Quaternion.identity);
        //         vfx.GetComponent<LightningBolt>().Fire(transform, enemy.transform);
        //     }
        //     
        //     enemy.TakeDamage(ability.Damage);
        //     ability.SetAbilityState(Ability.AbilityState.OnCooldown);
        // }


        private EnemyModel GetClosestEnemyPhysicsOverlap()
        {
            var count = Physics.OverlapSphereNonAlloc(
                transform.position,
                overlapRadius,
                overlapColliders,
                enemyLayerMask);

            if (count == 0) return null;

            Transform closestTransform = null;
            var minSqrMag = float.MaxValue;
            for (var i = 0; i < count; i++)
            {
                var currentCol = overlapColliders[i];
                if (!currentCol) continue;

                var currentSqrMag = (transform.position - currentCol.transform.position).sqrMagnitude;
                if (currentSqrMag < minSqrMag)
                {
                    minSqrMag = currentSqrMag;
                    closestTransform = currentCol.transform;
                }
            }
            
            if (!closestTransform) return null;
            UnityEngine.Assertions.Assert.IsNotNull(closestTransform, "closest transform is not found");

            if (!closestTransform.root.TryGetComponent<EnemyModel>(out var enemyModel))
            {
                UnityEngine.Assertions.Assert.IsNotNull(enemyModel, "closest transform doesn't have EnemyModel component");
                return null;
            }

            return enemyModel;
        }
    }
}