using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class AttackableEnemiesCollector : MonoBehaviour
    {
        [Header("Enemy model layer")] 
        [SerializeField] private LayerMask enemyLayerMask;
        [SerializeField] private PlayerAttack playerAttackComponent;

        public List<EnemyModel> _attackableEnemies;
        public bool EnemyExists => _attackableEnemies.Count > 0;

        private void OnEnable()
        {
            _attackableEnemies = new List<EnemyModel>();
            playerAttackComponent.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (OtherIsNotEnemy(other, out var enemy)) return;

            enemy.OnEnemyDeath += OnEnemyDeath;
            _attackableEnemies.Add(enemy);
            playerAttackComponent.enabled = true;
        }

        private void OnEnemyDeath(EnemyModel enemy)
        {
            enemy.OnEnemyDeath -= OnEnemyDeath;
            RemoveEnemyFromAttackableCollection(enemy);
        }

        private void RemoveEnemyFromAttackableCollection(EnemyModel enemy)
        {
            _attackableEnemies.Remove(enemy);
            if (_attackableEnemies.Count == 0)
            {
                playerAttackComponent.enabled = false;
            }
        }

        public EnemyModel GetClosestEnemyFromList()
        {
            UnityEngine.Assertions.Assert.IsTrue(_attackableEnemies.Count > 0, "attackable enemies list is empty");
            
            if (_attackableEnemies.Count == 0) return null;

            EnemyModel closestEnemy = null;
            var minSqrMag = float.MaxValue;
            
            foreach (var e in _attackableEnemies)
            {
                var currentSqrMag = (transform.position - e.transform.position).sqrMagnitude;
                if (!(currentSqrMag < minSqrMag)) continue;
                
                minSqrMag = currentSqrMag;
                closestEnemy = e;
            }
            
            return closestEnemy;
        }

        private void OnTriggerExit(Collider other)
        {
            if (OtherIsNotEnemy(other, out var enemy)) return;
            RemoveEnemyFromAttackableCollection(enemy);
        }

        private bool OtherIsNotEnemy(Collider other, out EnemyModel enemy)
        {
            if ((enemyLayerMask.value & 1 << other.gameObject.layer) != 0)
            {
                if (other.transform.root.TryGetComponent(out enemy)) return false;
            }

            enemy = null;
            return true;
        }
    }
}