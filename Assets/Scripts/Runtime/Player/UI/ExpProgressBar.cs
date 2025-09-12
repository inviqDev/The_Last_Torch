using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class ExpProgressBar : MonoBehaviour
    {
        [SerializeField] private Slider expSlider;
        [SerializeField] private TextMeshProUGUI info;
        
        // TO KNOW: CURRENT MIN VALUE => MAX VALUE =>
        public void Init(PlayerModel player)
        {
            player.OnPlayerLevelChanged += OnPlayerLevelChanged;
            player.OnPlayerExpChanged += OnPlayerExpChanged;
        }

        private void OnPlayerLevelChanged(float levelMinExpValue, float levelMaxExpValue)
        {
            expSlider.minValue = levelMinExpValue;
            expSlider.maxValue = levelMaxExpValue;
        }

        private void OnPlayerExpChanged(float currentExp)
        {
            expSlider.value = currentExp;
            info.text = $"{currentExp} / {expSlider.maxValue}";
        }
    }
}
