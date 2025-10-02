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
                OnTriggerDetected?.Invoke(enemy);
                return;
            }

            other.gameObject.transform.name = "unexpectedCollision";
            UnityEngine.Assertions.Assert.IsNotNull(enemy,
                $"{gameObject.transform.root.name} has collided with {other.gameObject.transform.name}");
        }
    }
}
