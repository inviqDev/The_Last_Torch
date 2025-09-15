using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class AbilitySlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI abilityName;
        [SerializeField] private Image abilityIcon;
        [SerializeField] private Button abilityButton;
        [SerializeField] private Slider abilitySlider;

        private readonly float _minSliderValue = 0f;
        private float _maxSliderValue;
        
        public bool IsActive { get; private set; }

        public void SetUpAbilityUI(Ability ability, bool isActive)
        {
            abilityName.text = ability.Name;
            abilityIcon.sprite = ability.Icon;

            abilitySlider.minValue = _minSliderValue;
            _maxSliderValue = ability.Cooldown;
            abilitySlider.maxValue = _maxSliderValue;
            abilitySlider.value = abilitySlider.minValue;
            
            IsActive = isActive;
            abilityButton.interactable = IsActive;
        }

        public void ShowCooldownProgress(float progress)
        {
            abilitySlider.value = progress;
        }
    }
}