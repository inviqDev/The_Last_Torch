using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime
{
    public abstract class TransitionPayload : ScriptableObject
    {
        // Переопредели при необходимости
        public virtual void OnWillLoad() { }

        public virtual void OnDidLoad(Scene newScene) { }
    }
}