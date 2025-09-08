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
        
        // [SerializeField] private List<Ability> abilities;
        // [SerializeField] private List<Ability> targetAbilities;

        private readonly Collider[] overlapColliders = new Collider[128];
        private Transform closestEnemy;

        // private void Update()
        // {
        //     foreach (var a in abilities)
        //     {
        //         if (a.State != Ability.AbilityState.Ready) continue;
        //         
        //         var enemy = collector.GetClosestEnemyFromList();
        //         a.Activate(enemy);
        //     }
        // }

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