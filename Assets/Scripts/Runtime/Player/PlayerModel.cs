#region usings

using UnityEngine;
using MyAssert = UnityEngine.Assertions.Assert;

#endregion

namespace Runtime
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerModel : MonoBehaviour
    {
        [SerializeField] private BoxCollider _collisionDetector;

        [SerializeField] private PlayerMovement movementComponent;
        [SerializeField] private Dash dashComponent;

        private PlayerManager pm;

        private float _moveSpeed;

        private float _dashSpeed;
        private float _dashDuration;

        private float _damage;
        public float Damage => _damage;


        private void OnEnable()
        {
            MyAssert.IsNotNull(PlayerManager.Instance, "PlayerManager is null");

            pm = PlayerManager.Instance;
            pm.OnConfigChanged += ChangeConfig;
        }

        public void ChangeConfig(PlayerConfig config)
        {
            _moveSpeed = config.MoveSpeed;
            _dashSpeed = config.DashSpeed;
            _dashDuration = config.DashDuration;
            _damage = config.Damage;

            movementComponent.SetMoveSettingsFromConfig(config);
            dashComponent.SetDashSettingsFromConfig(config);

            print($"moveSpeed: {_moveSpeed} // _dashSpeed: {_dashSpeed} // _dashDuration: {_dashDuration}");
        }

        public void ChangeAllModifiers()
        {
            movementComponent.IncreaseMoveSpeedModifier();
            dashComponent.IncreaseDashSpeedModifier();
            dashComponent.IncreaseDashDurationModifier();
        }

        private void OnDisable()
        {
            if (!pm) return;
            
            pm.OnConfigChanged -= ChangeConfig;
        }
    }
}