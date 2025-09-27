using UnityEngine;

namespace Runtime
{
    [DefaultExecutionOrder(-999)]
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private Player playerPrefab;
        [SerializeField] private PlayerConfig playerConfig;
        
        private Player _player;
        
        public Player SpawnPlayer(Vector3 spawnPoint)
        {
            var cameraMain = GameManager.Instance?.CameraMain;
            _player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity, null);
            _player.GetComponent<CameraMover>().Init(cameraMain, _player.transform);
            _player.PlayerAttack.Init(_player);
            
            return _player;
        }

        public void LoadPlayerConfig()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_player, "player is null");
            _player.SetUpPlayerConfig(playerConfig);
        }
    }
}