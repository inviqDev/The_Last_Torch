using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    [Header("Enemy model layer")]
    [SerializeField] private LayerMask enemyLayerMask;

    private void OnTriggerEnter(Collider other)
    {
        if ((enemyLayerMask.value & 1 << other.gameObject.layer) == 0) return;
        
        var player = PlayerManager.Instance.CurrentPlayer;
        other.gameObject.GetComponent<EnemyModel>().TakeDamage(player.Damage);
    }
}