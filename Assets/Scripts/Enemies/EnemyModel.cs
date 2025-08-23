using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyModel : MonoBehaviour
{
    [SerializeField] private EnemyMovement movementComponent;
    
    [SerializeField] private float health;
    [SerializeField] private float moveSpeed;
    
    private PlayerModel _player;

    private void Start()
    {
        Debug.Assert(PlayerManager.Instance, "PlayerManager has not been found");
        if (!PlayerManager.Instance) return;
        
        _player = PlayerManager.Instance.CurrentPlayer;
        movementComponent.SetMovementSettings(_player, moveSpeed);
    }
}