using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Runtime
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement movementComponent;
        [SerializeField] private Rotation lookComponent;
        [SerializeField] private Dash dashComponent;
        [SerializeField] private CameraMover cameraMover;
        [SerializeField] private PlayerAttack playerAttack;

        private InputActions _inputActions;
        private Vector3 _lastMoveDirection;

        [Header("Target select")] [SerializeField]
        private LayerMask enemyLayerMask; // слой врагов для raycast

        [SerializeField] private float aimMaxDistance = 200f;

        // --- простейший режим выбора цели ---
        // private bool _isSelectingTarget = false;
        // private int _pendingAbilitySlot = -1;


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

            // _inputActions.Gameplay.UseSkill.performed += OnUseAbilityPerformed;

            _inputActions.Gameplay.Enable();
        }

        private void OnScrollPerformed(InputAction.CallbackContext ctx)
        {
            cameraMover.SetOffset(ctx.ReadValue<Vector2>().y);
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

            _inputActions.Gameplay.Disable();
        }
    }
}

// private void Update()
//         {
//             if (!_isSelectingTarget) return;
//
//             // ЛКМ — попытка выбрать цель
//             if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
//             {
//                 var cameraMain = GameManager.Instance?.CameraMain; // достаточно для MVP
//                 if (cameraMain)
//                 {
//                     var ray = cameraMain.ScreenPointToRay(Mouse.current.position.ReadValue());
//                     if (Physics.Raycast(ray, out var hit, aimMaxDistance, enemyLayerMask))
//                     {
//                         if (hit.transform.TryGetComponent(out EnemyModel enemy))
//                         {
//                             playerAttack?.UseAbility(_pendingAbilitySlot, enemy);
//                             ExitTargetSelectMode();
//                             return;
//                         }
//                     }
//                 }
//                 // Кликнули мимо врага — остаёмся в режиме, можно повторять
//             }
//
//             // Отмена правой кнопкой или Escape
//             if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
//             {
//                 ExitTargetSelectMode();
//             }
//         }
//
//         private void EnterTargetSelectMode(int slot)
//         {
//             _isSelectingTarget = true;
//             _pendingAbilitySlot = slot;
//
//             // опционально — показать курсор (если у тебя он скрыт)
//             Cursor.visible = true;
//             Cursor.lockState = CursorLockMode.None;
//
//             // опционально — подсветка UI, подсказка "Выберите цель ЛКМ"
//         }
//
//         private void ExitTargetSelectMode()
//         {
//             _isSelectingTarget = false;
//         }
//
//         private void OnUseAbilityPerformed(InputAction.CallbackContext ctx)
//         {
//             if (ctx.control is not KeyControl key) return;
//             UnityEngine.Assertions.Assert.IsNotNull(playerAttack, "playerAttack component is not found");
//
//             switch (key.keyCode)
//             {
//                 // case Key.Digit1:
//                 //     playerAttack.UseAbility(0);
//                 //     break;
//                 // case Key.Digit2:
//                 //     playerAttack.UseAbility(1);
//                 //     break;
//                 // case Key.Digit3:
//                 //     playerAttack.UseAbility(2);
//                 //     break;
//                 // case Key.Digit4:
//                 //     playerAttack.UseAbility(3);
//                 //     break;
//                 // case Key.Digit5:
//                 //     playerAttack.UseAbility(4);
//                 //     break;
//                 
//                 case Key.Digit1:
//                     EnterTargetSelectMode(0);
//                     break;
//                 case Key.Digit2:
//                     EnterTargetSelectMode(1);
//                     break;
//                 case Key.Digit3:
//                     EnterTargetSelectMode(2);
//                     break;
//                 case Key.Digit4:
//                     EnterTargetSelectMode(3);
//                     break;
//                 case Key.Digit5:
//                     EnterTargetSelectMode(4);
//                     break;
//             }
//         }