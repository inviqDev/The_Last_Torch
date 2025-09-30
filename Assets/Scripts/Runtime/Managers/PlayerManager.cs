using UnityEngine;

namespace Runtime
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private Player playerPrefab;
        [SerializeField] private PlayerConfig playerConfig;

        private Player _player;

        public Player SpawnPlayer(Vector3 spawnPoint)
        {
            _player = Instantiate(playerPrefab, null);
            _player.transform.SetPositionAndRotation(spawnPoint, Quaternion.identity);

            return _player;
        }

        public void LoadPlayerConfig()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_player, "player is null");
            _player.SetUpPlayerConfig(playerConfig);
        }
    }
}