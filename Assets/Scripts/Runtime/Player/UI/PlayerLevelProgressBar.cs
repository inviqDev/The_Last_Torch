using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class PlayerLevelProgressBar : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelInfo;
        [SerializeField] private Slider expSlider;
        [SerializeField] private TextMeshProUGUI info;
        
        private Player _player;
        
        public void Initialize(Player player)
        {
            _player = player;
            UnityEngine.Assertions.Assert.IsNotNull(
                _player, "Player is not set");

            UpdateLevelExpProgressBar(_player);
            
            _player.OnPlayerLevelChanged += UpdateLevelExpProgressBar;
            _player.OnPlayerExpChanged += OnPlayerExpChanged;
            _player.OnCharacterDeath += OnPlayerDeath;
        }

        private void UpdateLevelExpProgressBar(Player player)
        {
            expSlider.minValue = 0f;
            expSlider.maxValue = player.CurrentLevelMaxExp;
            
            levelInfo.text = $"LEVEL {player.CurrentLevel.ToString()}";
        }

        private void OnPlayerExpChanged(float currentExp)
        {
            expSlider.value = currentExp;
            info.text = $"{currentExp} / {expSlider.maxValue}";
        }
        
        private void OnPlayerDeath(Character player)
        {
            _player.OnCharacterDeath -= OnPlayerDeath;
            
            _player.OnPlayerLevelChanged -= UpdateLevelExpProgressBar;
            _player.OnPlayerExpChanged -= OnPlayerExpChanged;
        }
    }
}
