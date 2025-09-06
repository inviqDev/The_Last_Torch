using System;
using UnityEngine;

namespace Runtime
{
    [DefaultExecutionOrder(-999)]
    public class PlayerManager : Singleton<PlayerManager>
    {
        public event Action<PlayerConfig> OnConfigChanged;

        [SerializeField] private PlayerConfig[] configs;
        [SerializeField] private int currentLevel;
        private PlayerConfig _currentConfig;
    
        [SerializeField] private PlayerModel playerPrefab;
        private PlayerModel _player;
        public PlayerModel Player => _player;

        public PlayerModel SpawnPlayer(Vector3 spawnPoint)
        {
            _player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity, null);
            _player.ChangeConfig(configs[currentLevel - 1]);
            
            return _player;
        }

        public void IncreasePlayerLevel()
        {
            ++currentLevel;
            LoadNextLevelConfig();
        }

        private void LoadNextLevelConfig()
        {
            if (currentLevel > configs.Length)
            {
                print("OVER LIMIT");
                currentLevel = 1;
            }
        
            _currentConfig = configs[currentLevel - 1];
            OnConfigChanged?.Invoke(_currentConfig);
        }
    }
}