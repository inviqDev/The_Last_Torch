using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [SerializeField] private PlayerMovement movementComponent;
    [SerializeField] private Dash dashComponent;
    
    private InputActions _inputActions;

    private void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        if (_inputActions == null) return;
        
        _inputActions.Gameplay.Move.started += MoveIsStarted;
        _inputActions.Gameplay.Move.performed += MoveIsPerformed;
        _inputActions.Gameplay.Move.canceled += MoveIsCanceled;
        
        _inputActions.Gameplay.Dash.performed += DashIsPerformed;
        
        _inputActions.Gameplay.Level_Up.started += IncreasePlayerLevel;
        _inputActions.Gameplay.ChangeModifiers.started += ChangeModifiers;
        
        _inputActions.Gameplay.Enable();
    }

    private void IncreasePlayerLevel(InputAction.CallbackContext ctx)
    {
        PlayerManager.Instance?.IncreasePlayerLevel();
    }

    private void ChangeModifiers(InputAction.CallbackContext ctx)
    {
        PlayerManager.Instance.CurrentPlayer.ChangeAllModifiers();
    }
    
    private void MoveIsStarted(InputAction.CallbackContext ctx)
    {
        var input = ctx.ReadValue<Vector2>();
        if (input == Vector2.zero) return;
        
        var direction = new Vector3(input.x, 0, input.y);
        movementComponent.StartMovement(direction);
    }

    private void MoveIsPerformed(InputAction.CallbackContext ctx)
    {
        var input = ctx.ReadValue<Vector2>();
        if (input == Vector2.zero) return;
        
        var direction = new Vector3(input.x, 0, input.y);
        movementComponent.MoveInDirection(direction);
    }

    private void MoveIsCanceled(InputAction.CallbackContext ctx)
    {
        movementComponent.StopMovement();
    }

    private void DashIsPerformed(InputAction.CallbackContext ctx)
    {
        var dashDirection = movementComponent.Direction;
        if (dashDirection == Vector3.zero) return;
        
        movementComponent.StopMovement();
        dashComponent.PerformDash(dashDirection);
        dashComponent.OnDashFinished += ResetInputAfterDash;
    }

    private void ResetInputAfterDash()
    {
        dashComponent.OnDashFinished -= ResetInputAfterDash;
        
        var input = _inputActions.Gameplay.Move.ReadValue<Vector2>();
        if (input == Vector2.zero) return;
        
        var moveDirection = new Vector3(input.x, 0, input.y);
        movementComponent.StartMovement(moveDirection);
    }

    private void OnDisable()
    {
        if (_inputActions == null) return;
        
        _inputActions.Gameplay.Move.started -= MoveIsStarted;
        _inputActions.Gameplay.Move.performed -= MoveIsPerformed;
        _inputActions.Gameplay.Move.canceled -= MoveIsCanceled;
        
        _inputActions.Gameplay.Dash.performed -= DashIsPerformed;
        
        _inputActions.Gameplay.Level_Up.started -= IncreasePlayerLevel;
        _inputActions.Gameplay.ChangeModifiers.started -= ChangeModifiers;
        
        _inputActions.Gameplay.Disable();
    }
}