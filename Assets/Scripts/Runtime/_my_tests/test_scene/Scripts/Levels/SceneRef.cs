using System;
using UnityEngine;

namespace Runtime._my_tests.test_scene.Scripts
{
    [Serializable]
    public struct SceneRef
    {
        [SerializeField, HideInInspector] private string _path; // Runtime-путь к сцене
        public string Path => _path;

#if UNITY_EDITOR
        [SerializeField] private UnityEditor.SceneAsset _scene; // только в редакторе
        public UnityEditor.SceneAsset EditorScene => _scene;

        public void OnValidate()
        {
            if (!_scene)
            {
                _path = string.Empty;
                return;
            }

            _path = UnityEditor.AssetDatabase.GetAssetPath(_scene);
        }
#endif
    }
}