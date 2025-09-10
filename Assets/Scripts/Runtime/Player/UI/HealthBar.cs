#region usings

using UnityEngine;
using UnityEngine.UI;

using MyAssert = UnityEngine.Assertions.Assert;

#endregion

namespace Runtime
{
    public class HealthBar : MonoBehaviour
    {
        private CharacterBase _characterBase;
        private Slider _healthBar;
        private readonly float _minValue = 0f;
        
        public void Init(CharacterBase character)
        {
            _characterBase = character;
            MyAssert.IsNotNull(character, "character is null");
            if (!_characterBase) return;
            
            _healthBar = GetComponent<Slider>();
            _healthBar.maxValue = _characterBase.CurrentHealth;
            _healthBar.value = _characterBase.CurrentHealth;

            _characterBase.OnHealthChanged += OnHealthChanged;
        }

        private void OnHealthChanged(float currentHealth)
        {
            if (currentHealth < _minValue) return;
            _healthBar.value = currentHealth;
        }

        private void OnDisable()
        {
            if (!_characterBase) return;
            _characterBase.OnHealthChanged += OnHealthChanged;
        }
    }
}