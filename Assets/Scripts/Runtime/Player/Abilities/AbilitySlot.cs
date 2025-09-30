using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class AbilitySlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI damage;
        
        [SerializeField] private Image iconImage;
        [SerializeField] private Button activationButton;
        [SerializeField] private Slider cooldownProgress;
        [SerializeField] private TextMeshProUGUI cooldownText;

        private readonly float _minSliderValue = 0f;
        private float _maxSliderValue;

        public bool IsActive { get; private set; }

        public void ActivateAbilityUI(Ability ability)
        {
            IsActive = ability.State != AbilityState.None;

            iconImage.sprite = ability.Icon;

            _maxSliderValue = ability.Cooldown;
            cooldownProgress.minValue = _minSliderValue;
            cooldownProgress.maxValue = _maxSliderValue;

            activationButton.interactable = IsActive;
            
            ability.OnAbilityStatsChanged += UpdateAbilityUI;
            
            if (IsActive)
            {
                UpdateAbilityUI(ability);
            }
        }

        private void UpdateAbilityUI(Ability ability)
        {
            damage.text = $"{ability.Damage:F0} DMG";
            
            cooldownProgress.maxValue = ability.Cooldown;
            cooldownText.text = $"{ability.Cooldown:F2} SEC";
        }

        public void ShowCooldownProgress(float progress)
        {
            cooldownProgress.value = progress;
        }

        public void SetInProgressUIState(bool isInteractable, bool isActive)
        {
            activationButton.interactable = isInteractable;
            cooldownProgress.gameObject.SetActive(isActive);
        }
    }
}