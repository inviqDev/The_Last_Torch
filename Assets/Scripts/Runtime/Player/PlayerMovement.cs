using UnityEngine;

namespace Runtime
{
    public sealed class PlayerMovement : Movement
    {
        private Rotation _playerRotation;
        private float _moveSpeed;

        protected override void InitMovement()
        {
            base.InitMovement();
            
            _playerRotation ??= GetComponent<Rotation>();
            UnityEngine.Assertions.Assert.IsNotNull(_playerRotation,
                "player rotation component is missing");
            
            enabled = false;
        }

        public override void StartMovement(Vector3 dir)
        {
            direction = dir * _moveSpeed;
            _playerRotation.FaceDirection(direction);
            enabled = true;
        }

        public override void Move(Vector3 dir)
        {
            if (dir == Vector3.zero) return;
            direction = dir.normalized * _moveSpeed;
            _playerRotation.FaceDirection(direction);
        }

        public override void StopMovement()
        {
            direction = Vector3.zero;
            _playerRotation.StopFacing();
            enabled = false;
        }

        public void SetMoveSettingsFromConfig(PlayerConfig config)
        {
            InitMovement();
            _moveSpeed = config.moveSpeed;
        }

        public void SetNewMoveSpeed(float speed)
        {
            _moveSpeed = speed;
        }
    }
}