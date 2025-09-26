using UnityEngine;

namespace Runtime
{
    [RequireComponent(typeof(CharacterController))]
    public abstract class Movement : MonoBehaviour
    {
        private CharacterController _controller;
        protected Rotation playerRotation;
        protected Vector3 direction;

        protected virtual void InitMovement()
        {
            _controller ??= GetComponent<CharacterController>();
            MyAssertions.EnsureIsNotNull(_controller);
            
            playerRotation ??= GetComponent<Rotation>();
            MyAssertions.EnsureIsNotNull(playerRotation);
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