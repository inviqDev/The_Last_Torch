using System;
using UnityEngine;

namespace Runtime
{
    [DefaultExecutionOrder(-999)]
    public class PlayerManager : Singleton<PlayerManager>
    {
        public event Action<PlayerConfig> OnConfigChanged;

        //////////////////////////////////////////////////
        [SerializeField] private AbilityConfig lightning;
        [SerializeField] private AbilityConfig frost;
        //////////////////////////////////////////////////
        
        [SerializeField] private AbilityConfig defaultAbilityConfig;
        [SerializeField] private PlayerConfig[] configs;
        [SerializeField] private int currentLevel;
        private PlayerConfig _currentConfig;
    
        [SerializeField] private PlayerModel playerPrefab;
        private PlayerModel _player;
        public PlayerModel Player => _player;

        public PlayerModel SpawnPlayer(Vector3 spawnPoint)
        {
            return _player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity, null);
        }

        public void LoadDefaultConfig(AbilityUI abilityUI)
        {
            UnityEngine.Assertions.Assert.IsNotNull(_player, "player is null");

            var defaultConfig = configs[currentLevel - 1];
            _player?.ChangeConfig(defaultConfig);
            
            var newAbility = new Ability(_player, defaultAbilityConfig, abilityUI);
            _player?.PlayerAttack.AddNewAbility(newAbility);
            
            // ========================================== //
            _player?.PlayerAttack.AddNewAbility(new Ability(_player, lightning, UIManager.Instance?.GetAbilityUI()));
            _player?.PlayerAttack.AddNewAbility(new Ability(_player, frost, UIManager.Instance?.GetAbilityUI()));
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