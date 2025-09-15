using UnityEngine;

namespace Runtime
{
    [DefaultExecutionOrder(-999)]
    public class PlayerManager : Singleton<PlayerManager>
    {
        [SerializeField] private PlayerModel playerPrefab;
        [SerializeField] private PlayerConfig playerConfig;
        
        private PlayerModel _player;
        
        public PlayerModel SpawnPlayer(Vector3 spawnPoint)
        {
            _player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity, null);
            // _player.PlayerAttack.Init(_player);
            
            return _player;
        }

        public void LoadPlayerDefaultConfig()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_player, "player is null");
            _player.SetUpPlayerConfig(playerConfig);
        }
    }
}