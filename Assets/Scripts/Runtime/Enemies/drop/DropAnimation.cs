using UnityEngine;

namespace Runtime
{
    public class DropAnimation : MonoBehaviour
    {
        [SerializeField] private bool isRotating;
        [SerializeField] private Vector3 rotationAxis;
        [SerializeField] private float rotationSpeed = 90f; // Degrees per second

        [SerializeField] private bool isScaling;
        [SerializeField] private Vector3 startScale;
        [SerializeField] private Vector3 endScale;
        [SerializeField] private bool useEasingForScaling; // Separate toggle for scaling ease
        [SerializeField] private float scaleLerpSpeed = 1f; // Speed of scaling transition
        
        [SerializeField] private bool isFloating;
        [SerializeField] private bool useEasingForFloating; // Separate toggle for floating ease
        [SerializeField] private float floatHeight = 1f; // Max height displacement
        [SerializeField] private float floatSpeed = 1f;

        private Vector3 _initialPosition;
        private float _floatTimer;
        private float _scaleTimer;

        private void Update()
        {
            if (isRotating)
            {
                transform.Rotate(rotationAxis * (rotationSpeed * Time.deltaTime));
            }

            if (isFloating)
            {
                _floatTimer += Time.deltaTime * floatSpeed;
                var t = Mathf.PingPong(_floatTimer, 1f);
                
                if (useEasingForFloating) 
                    t = EaseInOutQuad(t);

                transform.position = _initialPosition + new Vector3(0, t * floatHeight, 0);
            }

            if (isScaling)
            {
                _scaleTimer += Time.deltaTime * scaleLerpSpeed;
                var t = Mathf.PingPong(_scaleTimer, 1f); // Oscillates between 0 and 1

                if (useEasingForScaling)
                    t = EaseInOutQuad(t);
                
                transform.localScale = Vector3.Lerp(startScale, endScale, t);
            }
        }

        public void StartAnimation(bool rotating = true, bool floating = true, bool scaling = true)
        {
            _initialPosition = transform.position;
            
            isRotating = rotating;
            isFloating = floating;
            isScaling = scaling;
        }

        private float EaseInOutQuad(float t)
        {
            return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
        }
    }
}