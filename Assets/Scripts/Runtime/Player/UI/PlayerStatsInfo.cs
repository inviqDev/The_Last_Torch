using TMPro;
using UnityEngine;

namespace Runtime
{
    public class PlayerStatsInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI health;
        [SerializeField] private TextMeshProUGUI moveSpeed;

        private PlayerModel _player;
        
        public void Init(PlayerModel player)
        {
            _player = player;
            
            _player.OnPlayerMaxHealthChanged += OnPlayerMaxHealthChanged;
            _player.OnPlayerMoveSpeedChanged += OnPlayerMoveSpeedChanged;
        }

        private void OnPlayerMoveSpeedChanged(float newMoveSpeed)
        {
            moveSpeed.text = $"{newMoveSpeed} UNITS SPEED";
        }

        private void OnPlayerMaxHealthChanged(float newMaxHealth, float newCurrentHealth)
        {
            health.text = $"{newMaxHealth} MAX HP";
        }

        private void OnDisable()
        {
            if (!_player) return;
            
            _player.OnPlayerMaxHealthChanged -= OnPlayerMaxHealthChanged;
            _player.OnPlayerMoveSpeedChanged -= OnPlayerMoveSpeedChanged;
        }
    }
}