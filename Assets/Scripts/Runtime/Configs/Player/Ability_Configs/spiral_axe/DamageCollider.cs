using System;
using UnityEngine;

namespace Runtime
{
    public class DamageCollider : MonoBehaviour
    {
        public Action<Character> OnTriggerDetected;
        
        [Header("Layer masks")]
        [SerializeField] protected LayerMask enemyLayerMask;
        
        private void OnTriggerEnter(Collider other)
        {
            if ((enemyLayerMask.value & 1 << other.gameObject.layer) == 0) return;
            if (other.transform.root.TryGetComponent<Enemy>(out var enemy))
            {
                print($"triggered with {enemy.name}");
                OnTriggerDetected?.Invoke(enemy);
                return;
            }
            
            UnityEngine.Assertions.Assert.IsNotNull(enemy,
                $"{gameObject.name} collided with {other.gameObject.name}");
        }
    }
}
