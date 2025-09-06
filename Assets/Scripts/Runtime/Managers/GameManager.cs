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

        public PlayerModel player { get; private set; }

        protected override void Awake()
        {
            var playerSpawnPoint = levelEnv.PlayerSpawnPoint;
            player = pm.SpawnPlayer(playerSpawnPoint);
            
            MyAsserts.IsNotNull(player, "player is null");
            /////////////////////////////////////////////////////////////
            player.GetComponent<CameraMover>().Init(Camera.main, player.transform);
        }
    }
}
