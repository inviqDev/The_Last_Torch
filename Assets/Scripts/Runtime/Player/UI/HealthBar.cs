#region usings

using UnityEngine;
using UnityEngine.UI;

using MyAssert = UnityEngine.Assertions.Assert;

#endregion

namespace Runtime
{
    public class HealthBar : MonoBehaviour
    {
        private Character _character;
        private Slider _healthBar;
        private readonly float _minValue = 0f;
        
        public void Init(Character character)
        {
            _character = character;
            MyAssert.IsNotNull(character, "character is null");
            if (!_character) return;
            
            _healthBar = GetComponent<Slider>();
            _healthBar.maxValue = _character.CurrentHealth;
            _healthBar.value = _character.CurrentHealth;

            _character.OnHealthChanged += OnHealthChanged;
        }

        private void OnHealthChanged(float currentHealth)
        {
            if (currentHealth < _minValue) return;
            _healthBar.value = currentHealth;
        }

        private void OnDisable()
        {
            if (!_character) return;
            _character.OnHealthChanged += OnHealthChanged;
        }
    }
}