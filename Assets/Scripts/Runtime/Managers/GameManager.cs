#region usings

using UnityEngine;
using MyAsserts = UnityEngine.Assertions.Assert;

#endregion

namespace Runtime
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private EnemySpawner spawner;
        [SerializeField] private UIManager UIManager;
        
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private ParticlesManager particlesManager;
        
        [SerializeField] private LevelEnvironment levelEnv;

        public Camera CameraMain { get; private set; }
        public PlayerModel Player { get; private set; }
        
        private SuperBoss _superBoss;

        // protected override void Awake()
        // {
        //     CameraMain = Camera.main;
        //     
        //     var playerSpawnPoint = levelEnv.PlayerSpawnPoint;
        //     Player = playerManager.SpawnPlayer(playerSpawnPoint);
        //     MyAsserts.IsNotNull(Player, "Player is null");
        //     
        //     UIManager.InitLevelUI(Player);
        //     playerManager.LoadPlayerDefaultConfig();
        //     Player.PlayerAttack.Init(Player);
        //     
        //     Player.GetComponent<CameraMover>().Init(CameraMain, Player.transform);
        //     
        //     soundManager.Init();
        //     particlesManager.Init();
        //     
        //     // spawner.OnSuperBossSpawned += OnSuperBossSpawned;
        //     spawner.SpawnNextWave();
        //     
        //     Player.OnCharacterDeath += OnPlayerDeath;
        // }

        public void Init()
        {
            playerManager = Instantiate(playerManager, null);
            spawner = Instantiate(spawner, null);
            UIManager = Instantiate(UIManager, null);
            soundManager = Instantiate(soundManager, null);
            particlesManager = Instantiate(particlesManager, null);
            levelEnv = Instantiate(levelEnv, Vector3.zero, Quaternion.identity, null);
            
            CameraMain = Camera.main;
            
            var playerSpawnPoint = levelEnv.PlayerSpawnPoint;
            Player = playerManager.SpawnPlayer(playerSpawnPoint);
            MyAsserts.IsNotNull(Player, "Player is null");
            
            UIManager.InitLevelUI(Player);
            playerManager.LoadPlayerDefaultConfig();
            Player.PlayerAttack.Init(Player);
            
            Player.GetComponent<CameraMover>().Init(CameraMain, Player.transform);
            
            soundManager.Init();
            particlesManager.Init();
            
            // spawner.OnSuperBossSpawned += OnSuperBossSpawned;
            spawner.SpawnNextWave();
            
            Player.OnCharacterDeath += OnPlayerDeath;
        }
        
        // private void OnSuperBossSpawned(SuperBoss boss)
        // {
        //     _superBoss = boss;
        //     boss.OnSuperBossDeath += ShowWinUI;
        //     print("SINGED UP");
        // }
        //
        // private void ShowWinUI()
        // {
        //     print("CALLED EVENT");
        //     UIManager.Instance?.ShowWinGamePanel();
        //     _superBoss.OnSuperBossDeath -= ShowWinUI;
        // }

        private void OnPlayerDeath(Character player)    
        {
            print("PLAYER IS DEAD !");
            
            UIManager.TurnOnGameOverPanel();
            gameObject.SetActive(false);
            
            Time.timeScale = 0f;
        }
    }
}
