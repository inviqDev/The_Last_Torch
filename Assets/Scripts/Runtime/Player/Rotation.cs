using UnityEngine;

namespace Runtime
{
    public enum FacingMode
    {
        None,
        Direction, // к заданному мировому направлению (задаётся извне)
        TargetTransform, // к Transform цели (обновляется каждый кадр)
        TargetPosition // к фиксированной мировой точке
    }

    public class Rotation : MonoBehaviour
    {
        [Header("Transform to rotate")] 
        [SerializeField] private Transform rotateTarget;

        [Header("Smoothing")] 
        [SerializeField] private bool useSmoothDamp;
        [SerializeField] private float turnSpeedDegPerSec = 720f;
        [SerializeField] private float smoothTime = 0.08f;
        [SerializeField] private float deadZoneDeg = 0.25f;

        [Header("Rotation around axis:")] 
        [SerializeField] private Vector3 upAxis = Vector3.up;

        private FacingMode _mode;
        private Transform _target;
        private Vector3 _targetPos;
        private Vector3 _lookDir;
        private Vector3 _desiredDir;
        private float _yawVel;

        private void Awake()
        {
            // UnityEngine.Assertions.Assert.IsNotNull(rotateTarget, "rotateTarget is null");

            if (upAxis == default) upAxis = Vector3.up;
            _mode = FacingMode.None;
        }

        private void Update()
        {
            if (_mode == FacingMode.None || !rotateTarget) return;

            _lookDir = CalculateTargetDirection();
            if (_lookDir.sqrMagnitude < 1e-6f) return;

            var targetYaw = Mathf.Atan2(_lookDir.x, _lookDir.z) * Mathf.Rad2Deg;
            var currentYaw = rotateTarget.eulerAngles.y;

            var delta = Mathf.DeltaAngle(currentYaw, targetYaw);
            if (Mathf.Abs(delta) < deadZoneDeg) return;

            var newYaw = useSmoothDamp
                ? Mathf.SmoothDampAngle(currentYaw, targetYaw, ref _yawVel, smoothTime)
                : Mathf.MoveTowardsAngle(currentYaw, targetYaw, turnSpeedDegPerSec * Time.deltaTime);

            rotateTarget.rotation = Quaternion.Euler(0f, newYaw, 0f);
        }

        private Vector3 CalculateTargetDirection()
        {
            switch (_mode)
            {
                case FacingMode.TargetTransform:
                    return _target.position - rotateTarget.position;
                case FacingMode.TargetPosition:
                    return _targetPos - rotateTarget.position;
                case FacingMode.Direction:
                    return _desiredDir;
                default:
                    return Vector3.zero;
            }
        }

        public void FaceDirection(Vector3 currentMoveDirection)
        {
            _desiredDir = currentMoveDirection.normalized;
            _mode = _desiredDir.sqrMagnitude > 0f ? FacingMode.Direction : FacingMode.None;
        }

        public void FaceTarget(Transform target)
        {
            _target = target;
        
            // UnityEngine.Assertions.Assert.IsNotNull(_target, "_target is null");
            _mode = target ? FacingMode.TargetTransform : FacingMode.None;
        }

        public void StopFacing() => _mode = FacingMode.None;

        public void SetTurnSpeed(float degPerSec) => turnSpeedDegPerSec = Mathf.Max(1f, degPerSec);

        public void SetSmooth(bool isSmoothing, float newSmoothTime = -1f)
        {
            useSmoothDamp = isSmoothing;
            if (newSmoothTime > 0f) smoothTime = newSmoothTime;
        }
    }
    
    // comments to remove:
    // Awake() : // UnityEngine.Assertions.Assert.IsNotNull(rotateTarget, "rotateTarget is null");
    // FaceTarget() : // UnityEngine.Assertions.Assert.IsNotNull(_target, "_target is null");
}