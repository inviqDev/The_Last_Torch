using UnityEngine;

namespace Runtime
{
    public class BillboardPlayerUI : MonoBehaviour
    {
        private Transform _cameraMainTransform;

        private void Start()
        {
            UnityEngine.Assertions.Assert.IsNotNull(GameManager.Instance, "GameManager is null");
            _cameraMainTransform = GameManager.Instance?.CameraMain.transform;
        }

        private void LateUpdate()
        {
            transform.LookAt(_cameraMainTransform);
        }
    }
}
