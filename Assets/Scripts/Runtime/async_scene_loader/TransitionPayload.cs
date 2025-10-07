using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime
{
    [CreateAssetMenu(menuName = "My Scriptable Objects/Async Scene Loading/Payload", order = 0, fileName = "payload")]
    public abstract class TransitionPayload : ScriptableObject
    {
        // Переопредели при необходимости
        public virtual void OnWillLoad() { }

        public virtual void OnDidLoad(Scene newScene) { }
    }
}