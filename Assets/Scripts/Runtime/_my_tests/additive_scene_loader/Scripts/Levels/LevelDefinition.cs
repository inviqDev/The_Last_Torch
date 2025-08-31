using UnityEngine;

namespace Runtime._my_tests
{
    [CreateAssetMenu(menuName = "Game/Level Definition", fileName = "LevelDefinition")]
    public sealed class LevelDefinition : ScriptableObject
    {
        [Header("Environment (Additive)")]
        public SceneRef AdditiveScene;            // dropdown вместо строки

        [Header("Player")]
        public GameObject PlayerManagerPrefab;    // пустышка с твоим PlayerManager
        public ScriptableObject PlayerConfig;     // твой конфиг игрока (SO)

        [Header("Enemies/Spawners (optional)")]
        public GameObject SpawnerPrefab;          // префаб спаунера (если не лежит в сцене)
        public ScriptableObject WavesConfig;      // конфиг волн (SO)

        [Header("Flow (optional)")]
        public SceneRef NextLevel;                // на будущее, если делаешь переходы
    }
}