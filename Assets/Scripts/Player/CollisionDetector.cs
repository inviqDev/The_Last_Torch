using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    [Header("Enemy model layer")]
    [SerializeField] private LayerMask enemyLayerMask;

    private void OnTriggerEnter(Collider other)
    {
        if ((enemyLayerMask.value & 1 << other.gameObject.layer) == 0) return;
        
        var playerDamage = PlayerManager.Instance.CurrentPlayer.Damage;
        other.gameObject.GetComponentInParent<EnemyModel_NavMesh>().TakeDamage(playerDamage);
    }
}