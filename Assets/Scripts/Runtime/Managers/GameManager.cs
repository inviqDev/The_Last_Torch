#region usings

using UnityEngine;
using MyAsserts = UnityEngine.Assertions.Assert;

#endregion

namespace Runtime
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private LevelEnvironment levelEnv;
        [SerializeField] private PlayerManager pm;
        // [SerializeField] private 

        public Camera CameraMain { get; private set; }
        
        public PlayerModel Player { get; private set; }

        protected override void Awake()
        {
            var playerSpawnPoint = levelEnv.PlayerSpawnPoint;
            
            Player = pm.SpawnPlayer(playerSpawnPoint);
            MyAsserts.IsNotNull(Player, "Player is null");
            
            /////////////////////////////////////////////////////////////
            CameraMain = Camera.main;
            Player.GetComponent<CameraMover>().Init(CameraMain, Player.transform);
            Player.OnPlayerDeath += OnPlayerDeath;
        }

        private void OnPlayerDeath()
        {
            
        }
    }
}
