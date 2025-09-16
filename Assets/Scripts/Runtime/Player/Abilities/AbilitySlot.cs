using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class AbilitySlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI abilityName;
        [SerializeField] private Image iconImage;
        [SerializeField] private Button activationButton;
        [SerializeField] private Slider cooldownProgress;
        [SerializeField] private TextMeshProUGUI damage;

        private readonly float _minSliderValue = 0f;
        private float _maxSliderValue;

        public bool IsActive { get; private set; }

        public void SetUpAbilityUI(Ability ability) //, bool isActive)
        {
            IsActive = ability.State != Ability.AbilityState.None;

            ability.OnDamageValueChanged += OnDamageValueChanged;

            abilityName.text = ability.Name;
            iconImage.sprite = ability.Icon;

            _maxSliderValue = ability.Cooldown;
            cooldownProgress.minValue = _minSliderValue;
            cooldownProgress.maxValue = _maxSliderValue;

            activationButton.interactable = IsActive;

            if (IsActive)
            {
                OnDamageValueChanged(ability.Damage);
                // ShowCooldownProgress(_minSliderValue);
            }
        }

        private void OnDamageValueChanged(float newDamageValue)
        {
            damage.text = $"{newDamageValue} DMG";
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