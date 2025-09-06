using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;

namespace Runtime
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement movementComponent;
        [SerializeField] private Rotation lookComponent;
        [SerializeField] private Dash dashComponent;
        [SerializeField] private CameraMover cameraMover;
    
        private InputActions _inputActions;

        private Vector3 _lastMoveDirection;
    
        private void Awake()
        {
            _inputActions = new InputActions();
            _lastMoveDirection = Vector2.zero;
        }

        private void OnEnable()
        {
            if (_inputActions == null) return;
        
            _inputActions.Gameplay.Move.started += MoveIsStarted;
            _inputActions.Gameplay.Move.performed += MoveIsPerformed;
            _inputActions.Gameplay.Move.canceled += MoveIsCanceled;
        
            _inputActions.Gameplay.Dash.performed += DashIsPerformed;
            
            _inputActions.Gameplay.Scroll.performed += OnScrollPerformed;
        
            _inputActions.Gameplay.Level_Up.started += IncreasePlayerLevel;
            _inputActions.Gameplay.ChangeModifiers.started += ChangeModifiers;
        
            _inputActions.Gameplay.SpawnWave.started += SpawnWave;
        
            _inputActions.Gameplay.Enable();
        }

        private void OnScrollPerformed(InputAction.CallbackContext ctx)
        {
            cameraMover.SetOffset(ctx.ReadValue<Vector2>().y);
        }

        private void SpawnWave(InputAction.CallbackContext ctx)
        {
            Debug.Assert(EnemySpawner.Instance, "Spawner is not found");
            // EnemySpawner.Instance?.SpawnWave();
        }

        private void IncreasePlayerLevel(InputAction.CallbackContext ctx)
        {
            PlayerManager.Instance?.IncreasePlayerLevel();
        }

        private void ChangeModifiers(InputAction.CallbackContext ctx)
        {
            GameManager.Instance.Player.ChangeAllModifiers();
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
        
            var currentMoveDirection = new Vector3(input.x, 0, input.y);
            movementComponent.MoveInDirection(currentMoveDirection);
        
            if (_lastMoveDirection == currentMoveDirection) return;
        
            _lastMoveDirection = currentMoveDirection;
            lookComponent.FaceDirection(_lastMoveDirection);
        }

        private void MoveIsCanceled(InputAction.CallbackContext ctx)
        {
            _lastMoveDirection = Vector2.zero;
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
        
            _inputActions.Gameplay.Scroll.performed -= OnScrollPerformed;
            
            _inputActions.Gameplay.Level_Up.started -= IncreasePlayerLevel;
            _inputActions.Gameplay.ChangeModifiers.started -= ChangeModifiers;
        
            _inputActions.Gameplay.SpawnWave.started -= SpawnWave;
        
            _inputActions.Gameplay.Disable();
        }
    }
}