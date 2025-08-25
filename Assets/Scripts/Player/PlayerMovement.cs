using UnityEngine;

// ask for sealed recommendation
public sealed class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;

    private float _moveSpeed;
    private float _moveSpeedModifier = 1.0f;
    
    private Vector3 _direction;
    public Vector3 Direction => _direction;

    private void Start()
    {
        enabled = false;
    }

    private void Update()
    {
        controller.Move(_direction * Time.deltaTime);
    }

    public void SetMoveSettingsFromConfig(PlayerConfig config)
    {
        _moveSpeed = config.MoveSpeed * _moveSpeedModifier;
    }
    
    public void IncreaseMoveSpeedModifier()
    {
        _moveSpeed *= _moveSpeedModifier;
        _moveSpeedModifier += 0.2f;
    }

    public void StartMovement(Vector3 direction)
    {
        if (direction == Vector3.zero) return;
        
        enabled = true;
        _direction = direction.normalized * _moveSpeed;
    }

    public void MoveInDirection(Vector3 direction)
    {
        if (direction == Vector3.zero) return;
        _direction = direction.normalized * _moveSpeed;
    }

    public void StopMovement()
    {
        enabled = false;
        _direction = Vector3.zero;
    }
}