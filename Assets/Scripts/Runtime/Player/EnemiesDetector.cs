using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class EnemiesDetector : MonoBehaviour
    {
        public Action OnEnemyDetected;
        public Action OnEnemiesListIsEmpty;
        
        [Header("Enemy model layer")] 
        [SerializeField] private LayerMask enemyLayerMask;

        private Player _player;
        
        public List<Enemy> AvailableEnemies { get; private set; }
        public bool EnemyExists => AvailableEnemies.Count > 0;

        public void Initialize()
        {
            AvailableEnemies ??= new List<Enemy>();
            AvailableEnemies?.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (OtherIsNotEnemy(other, out var enemy)) return;

            enemy.OnCharacterDeath += OnTargetDeath;
            AvailableEnemies.Add(enemy);
            
            OnEnemyDetected?.Invoke();
        }

        private void OnTargetDeath(Character target)
        {
            target.OnCharacterDeath -= OnTargetDeath;
            RemoveEnemyFromAttackableCollection(target as Enemy);
        }

        private void RemoveEnemyFromAttackableCollection(Enemy enemy)
        {
            AvailableEnemies.Remove(enemy);
            if (AvailableEnemies.Count != 0) return;
            
            OnEnemiesListIsEmpty?.Invoke();
        }

        public Enemy GetClosestEnemy(out float distance)
        {
            UnityEngine.Assertions.Assert.IsTrue(AvailableEnemies.Count > 0, 
                "attackable enemies list is empty");
            
            if (AvailableEnemies.Count == 0)
            {
                distance = float.MaxValue;
                return null;
            }

            Enemy closestEnemy = null;
            var minSqrMag = float.MaxValue;
            
            foreach (var e in AvailableEnemies)
            {
                var currentSqrMag = (transform.position - e.transform.position).sqrMagnitude;
                if (!(currentSqrMag < minSqrMag)) continue;
                
                minSqrMag = currentSqrMag;
                closestEnemy = e;
            }
            
            distance = Mathf.Sqrt(minSqrMag);
            return closestEnemy;
        }

        private void OnTriggerExit(Collider other)
        {
            if (OtherIsNotEnemy(other, out var enemy)) return;
            RemoveEnemyFromAttackableCollection(enemy);
        }

        private bool OtherIsNotEnemy(Collider other, out Enemy enemy)
        {
            if ((enemyLayerMask.value & 1 << other.gameObject.layer) != 0)
            {
                if (other.transform.root.TryGetComponent(out enemy)) return false;
            }

            enemy = null;
            return true;
        }

        private void OnDisable()
        {
            AvailableEnemies?.Clear();
        }
    }
}