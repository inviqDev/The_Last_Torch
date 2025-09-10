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
        [SerializeField] private UIManager UIManager;

        public Camera CameraMain { get; private set; }
        public PlayerModel Player { get; private set; }

        protected override void Awake()
        {
            var playerSpawnPoint = levelEnv.PlayerSpawnPoint;
            
            
            Player = playerManager.SpawnPlayer(playerSpawnPoint);
            MyAsserts.IsNotNull(Player, "Player is null");
            
            UIManager.ResetAllAbilitiesUI();
            playerManager.LoadDefaultConfig(UIManager.GetAbilityUI());
            
            /////////////////////////////////////////////////////////////
            CameraMain = Camera.main;
            Player.GetComponent<CameraMover>().Init(CameraMain, Player.transform);
            Player.OnPlayerDeath += OnPlayerDeath;
        }

        private void OnPlayerDeath()
        {
            print("PLAYER IS DEAD !");
            
            UIManager.TurnOnGameplayUI();
            gameObject.SetActive(false);
            
            Time.timeScale = 0f;
        }
    }
}
