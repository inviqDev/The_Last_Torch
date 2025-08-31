#if UNITY_EDITOR
using Runtime._my_tests.test_scene.Scripts;
using UnityEditor;
using UnityEngine;

namespace Runtime._my_tests.test_scene.Editor
{
    [CustomPropertyDrawer(typeof(SceneRef))]
    public class SceneRefDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var sceneProp = property.FindPropertyRelative("_scene");
            var pathProp = property.FindPropertyRelative("_path");

            // Заголовок
            position = EditorGUI.PrefixLabel(position, label);

            // Поле выбора SceneAsset
            EditorGUI.BeginChangeCheck();
            var newScene = (SceneAsset)EditorGUI.ObjectField(
                new Rect(position.x, position.y, position.width * 0.6f, position.height),
                sceneProp.objectReferenceValue, typeof(SceneAsset), false);

            // Dropdown по Build Settings
            var buildScenes = EditorBuildSettings.scenes;
            string[] names = new string[buildScenes.Length];
            int currentIndex = -1;
            for (int i = 0; i < buildScenes.Length; i++)
            {
                names[i] = System.IO.Path.GetFileNameWithoutExtension(buildScenes[i].path);
                if (!string.IsNullOrEmpty(pathProp.stringValue) && buildScenes[i].path == pathProp.stringValue)
                    currentIndex = i;
            }

            int newIndex = EditorGUI.Popup(
                new Rect(position.x + position.width * 0.62f, position.y, position.width * 0.38f, position.height),
                Mathf.Max(currentIndex, 0), names);

            if (newIndex >= 0 && newIndex < buildScenes.Length && buildScenes.Length > 0)
            {
                var path = buildScenes[newIndex].path;
                var picked = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                if (picked != null)
                {
                    sceneProp.objectReferenceValue = picked;
                    pathProp.stringValue = path;
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                // синхронизируем путь с объектом
                if (newScene)
                    pathProp.stringValue = AssetDatabase.GetAssetPath(newScene);
                sceneProp.objectReferenceValue = newScene;
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif