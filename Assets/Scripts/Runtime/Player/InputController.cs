using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement movementComponent;
        [SerializeField] private PlayerDash playerDashComponent;
        [SerializeField] private CameraMover cameraMover;
        [SerializeField] private PlayerAttack playerAttack;
        
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
            
            _inputActions.Gameplay.Dash.performed += PerformDash;
            _inputActions.Gameplay.Scroll.performed += OnScrollPerformed;
            
            _inputActions.Gameplay.Enable();
        }

        private void OnScrollPerformed(InputAction.CallbackContext ctx)
        {
            cameraMover.SetOffset(ctx.ReadValue<Vector2>().y);
        }

        private void MoveIsStarted(InputAction.CallbackContext ctx)
        {
            if (playerDashComponent.dashState == PlayerDash.DashState.InProgress) return;
            
            var input = ctx.ReadValue<Vector2>();
            if (input == Vector2.zero) return;
            
            var direction = new Vector3(input.x, 0, input.y);
            movementComponent.StartMovement(direction);
        }

        private void MoveIsPerformed(InputAction.CallbackContext ctx)
        {
            if (playerDashComponent.dashState == PlayerDash.DashState.InProgress) return;
            
            var input = ctx.ReadValue<Vector2>();
            if (input == Vector2.zero) return;
            
            var currentMoveDirection = new Vector3(input.x, 0, input.y);
            movementComponent.Move(currentMoveDirection);
        }

        private void MoveIsCanceled(InputAction.CallbackContext ctx)
        {
            if (playerDashComponent.dashState == PlayerDash.DashState.InProgress) return;
            
            movementComponent.StopMovement();
        }

        private void PerformDash(InputAction.CallbackContext ctx)
        {
            if (playerDashComponent.dashState != PlayerDash.DashState.Ready) return;
            
            var input = _inputActions.Gameplay.Move.ReadValue<Vector2>();
            if (input == Vector2.zero) return;
            
            movementComponent.StopMovement();
            
            var dashDirection = new Vector3(input.x, 0, input.y);
            playerDashComponent.StartMovement(dashDirection);
            
            playerDashComponent.OnDashFinished += ResetInputAfterPlayerDash;
        }

        private void ResetInputAfterPlayerDash()
        {
            playerDashComponent.OnDashFinished -= ResetInputAfterPlayerDash;
            
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
            
            _inputActions.Gameplay.Dash.performed -= PerformDash;
            _inputActions.Gameplay.Scroll.performed -= OnScrollPerformed;
            
            _inputActions.Gameplay.Disable();
        }
    }
}