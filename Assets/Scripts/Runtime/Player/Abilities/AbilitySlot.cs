using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class AbilitySlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI damageInfo;

        [SerializeField] private Image iconImage;
        [SerializeField] private Button activationButton;
        [SerializeField] private Slider cooldownProgress;
        [SerializeField] private TextMeshProUGUI cooldownInfo;

        private readonly float _minSliderValue = 0f;

        private Ability _ability;
        private float _maxSliderValue;

        public bool IsActive { get; private set; }

        public void SetAbilitySlotUI(Ability ability)
        {
            _ability = ability;

            IsActive = ability.State != AbilityState.Deactivated;
            iconImage.sprite = ability.Icon;
            activationButton.interactable = false;
            cooldownProgress.gameObject.SetActive(false);
            
            damageInfo.text = string.Empty;
            cooldownInfo.text = string.Empty;

            // _maxSliderValue = ability.Cooldown;
            // cooldownProgress.minValue = _minSliderValue;
            // cooldownProgress.maxValue = _maxSliderValue;

            if (IsActive)
            {
                _maxSliderValue = ability.Cooldown;
                cooldownProgress.minValue = _minSliderValue;
                cooldownProgress.maxValue = _maxSliderValue;
                
                _ability.Timer.OnAnyValueChanged += UpdateAbilityProgressBar;
                _ability.OnAbilityStateChange += OnAbilityStateChange;
                _ability.OnAbilityStatsChanged += UpdateAbilityUI;
                
                UpdateAbilityUI(_ability);
            }
        }

        private void OnAbilityStateChange(AbilityState state)
        {
            activationButton.interactable = state == AbilityState.Ready;
            cooldownProgress.gameObject.SetActive(state != AbilityState.InProgress);
        }

        private void UpdateAbilityProgressBar(float timerValue)
        {
            cooldownProgress.value = timerValue;
        }

        private void UpdateAbilityUI(Ability ability)
        {
            damageInfo.text = $"{ability.Damage:F2} DMG";

            cooldownProgress.maxValue = ability.Cooldown;
            cooldownInfo.text = $"{ability.Cooldown:F2} SEC";
        }

        private void OnDestroy()
        {
            if (_ability == null) return;

            if (_ability.Timer != null)
            {
                _ability.Timer.OnAnyValueChanged -= UpdateAbilityProgressBar;
            }

            _ability.OnAbilityStateChange -= OnAbilityStateChange;
            _ability.OnAbilityStatsChanged -= UpdateAbilityUI;

            _ability = null;
        }
    }
}