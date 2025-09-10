using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class AbilityUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI abilityName;
        [SerializeField] private Image abilityIcon;
        [SerializeField] private Button abilityButton;
        [SerializeField] private Slider abilitySlider;
        
        public Slider abilityCooldownSlider => abilitySlider;
        
        public bool IsActive { get; private set; }

        public void SetUpAbilityUI(AbilityConfig config, bool isActive)
        {
            abilityName.text = config.abilityName;
            abilityIcon.sprite = config.abilityIcon;

            abilitySlider.minValue = 0f;
            abilitySlider.maxValue = config.cooldown;
            abilitySlider.value = abilitySlider.minValue;
            
            abilityButton.interactable = isActive;
            IsActive = isActive;
        }

        public void ShowCooldownProgress(float progress)
        {
            abilitySlider.value = progress;
        }
    }
}