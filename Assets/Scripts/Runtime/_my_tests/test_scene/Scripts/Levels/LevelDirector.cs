using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime._my_tests.test_scene.Scripts.Levels
{
    public class LevelDirector : MonoBehaviour
    {
        [SerializeField] private LevelDefinition level;
        
        private PlayerManager _playerManager;
        public PlayerManager PlayerManager => _playerManager;

        private EnemySpawner _enemySpawner;
        public EnemySpawner EnemySpawner => _enemySpawner;

        private async void Start()
        {
            DontDestroyOnLoad(gameObject);
            await LoadLevel(level);
        }

        public async Task LoadLevel(LevelDefinition def)
        {
            // 1) Загружаем контентную сцену Additive
            if (string.IsNullOrEmpty(def.AdditiveScene.Path))
            {
                UnityEngine.Assertions.Assert.IsNotNull(def.AdditiveScene.Path, "AdditiveScene Path is null");
                return;
            }

            // ????
            var load = SceneManager.LoadSceneAsync(def.AdditiveScene.Path, LoadSceneMode.Additive);
            while (!load.isDone) await Task.Yield();

            var scene = SceneManager.GetSceneByPath(def.AdditiveScene.Path);
            SceneManager.SetActiveScene(scene);

            // 2) Спавним PlayerManager и создаём игрока (подставь свой API)
            var playerManagerGO = Instantiate(def.PlayerManagerPrefab, gameObject.transform);
            if (playerManagerGO.TryGetComponent(out PlayerManager playerManager))
            {
                _playerManager = playerManager;
                _playerManager.SpawnPlayer();
            }
            else
            {
                UnityEngine.Assertions.Assert.IsNotNull(playerManager, "PlayerManager is not found");
                Debug.Break();
            }

            // 3) Спавним спаунер (если нужно)
            if (def.SpawnerPrefab)
            {
                var spawnerGO = Instantiate(def.SpawnerPrefab, gameObject.transform);
                var enemySpawner = spawnerGO.GetComponent<EnemySpawner>();
                if (spawnerGO.TryGetComponent(out EnemySpawner spawner))
                {
                    _enemySpawner = spawner;
                }
            }

            Debug.Log("Level ready → Gameplay");
        }

        public async Task LoadNextLevel()
        {
            // пример перехода
            if (string.IsNullOrEmpty(level.NextLevel.Path))
            {
                Debug.LogWarning("NextLevel is not set");
                return;
            }

            // Загружаем новый
            var load = SceneManager.LoadSceneAsync(level.NextLevel.Path, LoadSceneMode.Additive);
            while (!load.isDone) await Task.Yield();
            var newScene = SceneManager.GetSceneByPath(level.NextLevel.Path);
            SceneManager.SetActiveScene(newScene);

            // Выгружаем все старые контентные, кроме Bootstrap
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var sc = SceneManager.GetSceneAt(i);
                if (sc.path != newScene.path && sc.name != "Bootstrap")
                    await SceneManager.UnloadSceneAsync(sc);
            }

            // Можно повторить шаги спавна под новый def…
        }
    }
}