using UnityEngine;

namespace Runtime
{
    public class CameraMover : MonoBehaviour
    {
        [Header("Camera position settings")]
        [Tooltip("Offset distance => how far from target camera appears by default")]
        [SerializeField] private Vector3 defaultOffsetFromTarget;
        [SerializeField] private float offsetDistance = 20f;

        [Tooltip("Movement smoothing time")]
        [SerializeField] private float smoothTime = 0.2f;

        [Header("Zoom settings")]
        [Tooltip("Minimum and maximum camera distance.")]
        [SerializeField] private float minDistance = 10f;
        [SerializeField] private float maxDistance = 50f;

        [Tooltip("How fast the scroll wheel changes the distance.")]
        [SerializeField] private float zoomSensitivity = 1f;

        private Transform _target;
        private Camera _camera;
        private Vector3 _offsetDir;
        private Vector3 _currentOffset;
        private Vector3 _smoothVel;

        public void Init(Camera movingCamera, Transform targetToFollow)
        {
            _target = targetToFollow;
            _camera = movingCamera;
            
            InitAssertions();

            _camera.transform.position = targetToFollow.position + defaultOffsetFromTarget;
            _camera.transform.LookAt(targetToFollow.position);

            _offsetDir = (_target.position - _camera.transform.position).normalized;
            _currentOffset = _offsetDir * offsetDistance;
        }

        private void InitAssertions()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_target, "_target is null");
            UnityEngine.Assertions.Assert.IsNotNull(_camera, "_camera is null");
            UnityEngine.Assertions.Assert.IsTrue(offsetDistance > 0f, "_currentOffset is < 0f");
            UnityEngine.Assertions.Assert.IsTrue(minDistance < maxDistance, "minDistance > maxDistance");
            UnityEngine.Assertions.Assert.IsTrue(minDistance > 0f, "minDistance is < 0");
            UnityEngine.Assertions.Assert.IsTrue(zoomSensitivity > 0f, "zoomSensitivity is < 0");
        }

        private void LateUpdate()
        {
            if (!_target || !_camera) return;

            var desiredPos = _target.position - _currentOffset;
            var updatedPos = Vector3.SmoothDamp(_camera.transform.position, desiredPos, ref _smoothVel, smoothTime);
            _camera.transform.position = updatedPos;
        }

        /// <summary>
        /// Update zoom distance. Positive 'scrollDelta' zooms in (make closer) if your input is positive for forward scroll.
        /// Pass the raw scroll delta on Y (do not normalize).
        /// </summary>
        public void SetOffset(float scrollDelta)
        {
            if (Mathf.Abs(scrollDelta) < 0.0001f) return;

            var scrollDirection = scrollDelta > 0 ? 1f : -1f;
            var desiredDistance = offsetDistance - scrollDirection * zoomSensitivity;
            offsetDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
            _currentOffset = _offsetDir * offsetDistance;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (maxDistance < minDistance) maxDistance = minDistance;
            offsetDistance = Mathf.Clamp(offsetDistance, minDistance, maxDistance);
        }
#endif
    }
}