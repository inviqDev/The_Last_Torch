#region usings

using TMPro;
using UnityEngine;
using UnityEngine.UI;

using MyAssert = UnityEngine.Assertions.Assert;

#endregion

namespace Runtime
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI healthInfo;
        
        private Character _character;
        private readonly float _minValue = 0f;
        private float _maxValue;
        
        public void Init(Character character)
        {
            _character = character;
            MyAssert.IsNotNull(_character, "character is not set properly");
            
            if (!_character) return;
            
            _maxValue = _character.MaxHealth;
            slider.maxValue = _maxValue;
            slider.value = _character.CurrentHealth;
            
            if (_character is Player player)
            {
                player.OnPlayerMaxHealthChanged += OnPlayerMaxHealthChanged;
            }
            
            _character.OnHealthChanged += OnHealthChanged;
            _character.OnCharacterDeath += OnCharacterDeath;
            
            SetHealthInfo(_character.CurrentHealth, _character.MaxHealth);
        }

        private void OnCharacterDeath(Character character)
        {
            if (_character is Player player)
            {
                player.OnPlayerMaxHealthChanged -= OnPlayerMaxHealthChanged;
            }
            
            _character.OnHealthChanged -= OnHealthChanged;
            _character.OnCharacterDeath -= OnCharacterDeath;
        }

        private void OnPlayerMaxHealthChanged(float newMaxHealth, float newCurrentHealth)
        {
            slider.maxValue = newMaxHealth;
            slider.value = newCurrentHealth;
            
            SetHealthInfo(newCurrentHealth, newMaxHealth);
        }

        private void SetHealthInfo(float currentHealth, float maxHealth)
        {
            healthInfo.text = $"{currentHealth} / {maxHealth}";
        }

        private void OnHealthChanged(float currentHealth)
        {
            if (currentHealth < _minValue) return;
            
            slider.value = currentHealth;
            SetHealthInfo(_character.CurrentHealth, _character.MaxHealth);
        }

        private void OnDisable()
        {
            if (!_character) return;
            _character.OnHealthChanged += OnHealthChanged;
        }
    }
}