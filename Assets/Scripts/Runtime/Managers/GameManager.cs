using UnityEngine;

namespace Runtime
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private Spawner spawner;
        [SerializeField] private Pool pool;
        [SerializeField] private UIManager UI_Manager;
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private ParticlesManager particlesManager;
        
        [SerializeField] private LevelEnvironment levelEnv;

        private PlayerManager _playerManager;
        private Spawner _spawner;
        private Pool _pool;
        private UIManager _uiManager;
        private SoundManager _soundManager;
        private ParticlesManager _particlesManager;
        private LevelEnvironment _levelEnv;

        public PlayerManager PlayerManager => _playerManager;
        public Spawner Spawner => _spawner;
        public Pool Pool => _pool;
        public UIManager UIManager => _uiManager;
        public SoundManager SoundManager => _soundManager;
        public ParticlesManager ParticlesManager => _particlesManager;
        public LevelEnvironment LevelEnv => _levelEnv;
        
        public Camera CameraMain { get; private set; }
        public Player Player { get; private set; }
        
        private SuperBoss _superBoss;
        private Timer _timer;

        protected override void Awake()
        {
            _playerManager ??= Instantiate(playerManager, transform);
            _spawner ??= Instantiate(spawner, transform);
            _pool = Instantiate(pool, null);
            _uiManager ??= Instantiate(UI_Manager, transform);
            _soundManager ??= Instantiate(soundManager, transform);
            _particlesManager ??= Instantiate(particlesManager, transform);
            
            playerManager.gameObject.SetActive(true);
            spawner.gameObject.SetActive(true);
            UI_Manager.gameObject.SetActive(true);
            soundManager.gameObject.SetActive(true);
            particlesManager.gameObject.SetActive(true);
        }
        
        public void Init()
        {
            CameraMain = Camera.main;
            
            _levelEnv = Instantiate(levelEnv, Vector3.zero, Quaternion.identity, transform);
            _levelEnv.RebakeNavMeshSurface();
            _levelEnv.gameObject.SetActive(true);
            
            if (Player)
            {
                print("Player exists before the start");
                Destroy(Player.gameObject);
            }
            
            var playerSpawnPoint = levelEnv.PlayerSpawnPoint;
            Player = _playerManager.SpawnPlayer(playerSpawnPoint);
            UnityEngine.Assertions.Assert.IsNotNull(Player, "Player is null");
            
            _uiManager.InitLevelUI(Player);
            
            Player.InitPlayerComponents();
            _playerManager.LoadPlayerConfig();
            
            _soundManager.Init();
            _particlesManager.Init();
            _spawner.Init();
            
            Player.OnCharacterDeath += OnPlayerDeath;
        }
        
        private void OnPlayerDeath(Character player)    
        {
            Spawner.StopSpawningEnemies();
            UIManager.TurnOnGameOverPanel();
        }
    }
}
