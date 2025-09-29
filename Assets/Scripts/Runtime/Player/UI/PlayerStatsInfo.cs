using TMPro;
using UnityEngine;

namespace Runtime
{
    public class PlayerStatsInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI health;
        [SerializeField] private TextMeshProUGUI moveSpeed;

        private Player _player;
        
        public void Init(Player player)
        {
            _player = player;
            
            _player.OnPlayerMaxHealthChanged += OnPlayerMaxHealthChanged;
            _player.OnPlayerMoveSpeedChanged += OnPlayerMoveSpeedChanged;
        }

        private void OnPlayerMoveSpeedChanged(float newMoveSpeed)
        {
            moveSpeed.text = $"{newMoveSpeed :F2} SPEED";
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