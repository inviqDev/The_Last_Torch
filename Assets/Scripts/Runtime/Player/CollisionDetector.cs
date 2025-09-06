using UnityEngine;

namespace Runtime
{
    public class CollisionDetector : MonoBehaviour
    {
        [Header("Enemy model layer")] 
        [SerializeField] private LayerMask enemyLayerMask;

        private void OnTriggerEnter(Collider other)
        {
            if ((enemyLayerMask.value & 1 << other.gameObject.layer) == 0) return;
            

            // var playerDamage = GameManager.Instance.Player.Damage;
            // other.gameObject.GetComponent<EnemyModel>().TakeDamage(playerDamage);
        }
    }
}