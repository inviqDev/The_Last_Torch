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
        
        // TO KNOW: CURRENT MIN VALUE => MAX VALUE =>
        public void Init(Player player)
        {
            _player = player;
            UnityEngine.Assertions.Assert.IsNotNull(_player, "Player is not set");
            
            // _player.OnPlayerStatsChanged += OnPlayerStatsChanged;
            _player.OnPlayerLevelChanged += OnPlayerLevelChanged;
            _player.OnPlayerExpChanged += OnPlayerExpChanged;
            
            _player.OnCharacterDeath += OnPlayerDeath;
        }

        private void OnPlayerStatsChanged(float arg1, float arg2)
        {
            throw new System.NotImplementedException();
        }

        private void OnPlayerDeath(Character player)
        {
            _player.OnCharacterDeath -= OnPlayerDeath;
            
            _player.OnPlayerLevelChanged -= OnPlayerLevelChanged;
            _player.OnPlayerExpChanged -= OnPlayerExpChanged;
        }

        private void OnPlayerLevelChanged(int newLevel, float levelMinExpValue, float levelMaxExpValue)
        {
            expSlider.minValue = levelMinExpValue;
            expSlider.maxValue = levelMaxExpValue;
            
            levelInfo.text = $"LEVEL {newLevel.ToString()}";
        }

        private void OnPlayerExpChanged(float currentExp)
        {
            expSlider.value = currentExp;
            info.text = $"{currentExp} / {expSlider.maxValue}";
        }
    }
}
