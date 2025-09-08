#region usings

using UnityEngine;
using UnityEngine.UI;

using MyAssert = UnityEngine.Assertions.Assert;

#endregion

namespace Runtime
{
    public class PlayerHealthBar : MonoBehaviour
    {
        private PlayerModel _playerModel;
        private Slider _healthBar;
        private float _minValue = 0f;
        
        public void Init(PlayerModel player)
        {
            _playerModel = player;
            MyAssert.IsNotNull(player, "Player is null");
            if (!_playerModel) return;
            
            _healthBar = GetComponent<Slider>();
            _healthBar.maxValue = _playerModel.CurrentHealth;
            _healthBar.value = _playerModel.CurrentHealth;

            _playerModel.OnHealthChanged += OnHealthChanged;
        }

        private void OnHealthChanged(float currentHealth)
        {
            if (currentHealth < _minValue) return;
            _healthBar.value = currentHealth;
        }


        private void OnDisable()
        {
            if (!_playerModel) return;
            _playerModel.OnHealthChanged += OnHealthChanged;
        }
    }
}