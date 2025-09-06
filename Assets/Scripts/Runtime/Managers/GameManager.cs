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

        public Camera CameraMain { get; private set; }
        
        public PlayerModel player { get; private set; }

        protected override void Awake()
        {
            var playerSpawnPoint = levelEnv.PlayerSpawnPoint;
            
            player = pm.SpawnPlayer(playerSpawnPoint);
            MyAsserts.IsNotNull(player, "player is null");
            
            /////////////////////////////////////////////////////////////
            CameraMain = Camera.main;
            player.GetComponent<CameraMover>().Init(CameraMain, player.transform);
        }
    }
}
