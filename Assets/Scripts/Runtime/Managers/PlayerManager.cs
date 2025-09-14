using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    [DefaultExecutionOrder(-999)]
    public class PlayerManager : Singleton<PlayerManager>
    {
        [SerializeField] private PlayerModel playerPrefab;
        [SerializeField] private PlayerConfig playerConfig;
        
        private PlayerModel _player;
        private int _nextAbilityIndex;
        
        public PlayerModel SpawnPlayer(Vector3 spawnPoint)
        {
            return _player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity, null);
        }

        public void LoadPlayerDefaultConfig()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_player, "player is null");
            
            _player.OnNextAbilityIsAvailable += OnNextAbilityAvailable;
            _player.OnCharacterDeath += OnPlayerDeath;
            
            _nextAbilityIndex = 0;
            _player.SetUpPlayerConfig(playerConfig);
        }

        private void OnNextAbilityAvailable()
        {
            var nextAbility = playerConfig.abilities[_nextAbilityIndex];
            var availableSlot = UIManager.Instance?.GetAvailableAbilitySlot();
            var newAbility = new Ability(_player, nextAbility, availableSlot);
            
            _player?.PlayerAttack.AddNewAbility(newAbility);
            _nextAbilityIndex++;
        }
        
        private void OnPlayerDeath(Character player)
        {
            _player.OnCharacterDeath -= OnPlayerDeath;
            _player.OnNextAbilityIsAvailable -= OnNextAbilityAvailable;
        }
    }
}