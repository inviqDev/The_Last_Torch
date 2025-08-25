using UnityEngine;

public class EnemyMovement_NON_NavMesh : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Rotation rotationComponent;
    
    private PlayerModel _player;
    private float _moveSpeed;

    public void SetMovementSettings(PlayerModel player, float moveSpeed, bool needToMove)
    {
        _player = player;
        _moveSpeed = moveSpeed;
        enabled = needToMove;
    }

    private void Update()
    {
        var direction = (_player.transform.position - transform.position).normalized;
        transform.Translate(direction * (_moveSpeed * Time.deltaTime));
    }
}