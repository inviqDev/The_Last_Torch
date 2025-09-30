using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(menuName = "My Scriptable Objects/Async Scene Loading/Transition", fileName = "Transition")]
    public class TransitionAsset : ScriptableObject
    {
        [Header("Target")]
        [SerializeField] private SceneReference targetScene;
        public SceneReference TargetScene => targetScene;

        
        [Header("Behavior")]
        [SerializeField] private bool setActiveAfterLoad = true;
        public bool SetActiveAfterLoad => setActiveAfterLoad;


        [Header("Overlay")] 
        [SerializeField] private bool showProgress = true;
        public bool ShowProgress => showProgress;

        [SerializeField, Min(0f)] private float minOverlayShowTime = 0.6f;
        public float MinOverlayShowTime => minOverlayShowTime;

        
        [Header("Data (optional)")] 
        [SerializeField] private TransitionPayload payload;
        public TransitionPayload Payload => payload;
    }
}