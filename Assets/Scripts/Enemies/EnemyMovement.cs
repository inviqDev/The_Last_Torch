using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private PlayerModel _player;
    private float _moveSpeed;

    public void SetMovementSettings(PlayerModel player, float moveSpeed)
    {
        _player = player;
        _moveSpeed = moveSpeed;
        
        enabled = true;
    }

    private void Update()
    {
        print(_player.transform.position);
        print(transform.position);
        
        var direction = (_player.transform.position - transform.position).normalized;
        transform.Translate(direction * (_moveSpeed * Time.deltaTime));
    }
}