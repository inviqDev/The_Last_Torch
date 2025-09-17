using UnityEngine;

namespace Runtime
{
    [System.Serializable]
    public struct SceneReference : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        [SerializeField] private UnityEditor.SceneAsset sceneAsset;
#endif
        [SerializeField] private string scenePath; // "Assets/Scenes/StartMenu.unity"
        public string Path => scenePath;

        [SerializeField] private string name;
        public string Name => name;
        public bool IsAssigned => !string.IsNullOrEmpty(scenePath);

#if UNITY_EDITOR
        public UnityEditor.SceneAsset SceneAsset
        {
            get => sceneAsset;
            set
            {
                sceneAsset = value;
                scenePath = value ? UnityEditor.AssetDatabase.GetAssetPath(value) : null;
            }
        }
#endif

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (sceneAsset)
            {
                scenePath = UnityEditor.AssetDatabase.GetAssetPath(sceneAsset);
            }
#endif
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize() { }
    }
}