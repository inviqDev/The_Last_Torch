using UnityEngine;

namespace Runtime
{
    [RequireComponent(typeof(CharacterController))]
    public abstract class Movement : MonoBehaviour
    {
        private CharacterController _controller;
        protected Rotation playerRotation;
        protected Vector3 direction;

        protected virtual void Initialize()
        {
            _controller ??= GetComponent<CharacterController>();
            UnityEngine.Assertions.Assert.IsNotNull(
                _controller, "character controller component is missing");
            
            playerRotation ??= GetComponent<Rotation>();
            UnityEngine.Assertions.Assert.IsNotNull(
                playerRotation, "player rotation component is missing");
        }

        private void Update()
        {
            _controller.Move(direction * Time.deltaTime);
        }

        public abstract void StartMovement(Vector3 dir);
        public abstract void Move(Vector3 dir);
        public abstract void StopMovement();
    }
}