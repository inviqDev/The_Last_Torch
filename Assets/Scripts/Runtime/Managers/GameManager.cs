#region usings

using UnityEngine;
using MyAsserts = UnityEngine.Assertions.Assert;

#endregion

namespace Runtime
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private LevelEnvironment levelEnv;
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private EnemySpawner spawner;
        [SerializeField] private UIManager UIManager;

        public Camera CameraMain { get; private set; }
        public PlayerModel Player { get; private set; }
        

        protected override void Awake()
        {
            CameraMain = Camera.main;
            
            var playerSpawnPoint = levelEnv.PlayerSpawnPoint;
            Player = playerManager.SpawnPlayer(playerSpawnPoint);
            
            MyAsserts.IsNotNull(Player, "Player is null");
            
            UIManager.InitLevelUI(Player);
            playerManager.LoadPlayerDefaultConfig();
            
            Player.GetComponent<CameraMover>().Init(CameraMain, Player.transform);
            
            spawner.SpawnNextWave();
            
            Player.OnPlayerDeath += OnPlayerDeath;
        }

        private void OnPlayerDeath()
        {
            print("PLAYER IS DEAD !");
            
            UIManager.TurnOnGameOverPanel();
            gameObject.SetActive(false);
            
            Time.timeScale = 0f;
        }
    }
}
