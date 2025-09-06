#region usings

using System;
using UnityEngine;
using MyAssert = UnityEngine.Assertions.Assert;

#endregion

namespace Runtime
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerModel : MonoBehaviour
    {
        public Action<float> OnHealthChanged;
        public Action OnPlayerDeath;

        
        [SerializeField] private PlayerHealthBar _healthBar;

        [SerializeField] private BoxCollider _collisionDetector;

        [SerializeField] private PlayerMovement movementComponent;
        [SerializeField] private Dash dashComponent;

        private PlayerManager pm;

        #region Player_Stats

        private float _health;
        private float _currentHealth;
        public float CurrentHealth => _currentHealth;

        private float _moveSpeed;

        private float _dashSpeed;
        private float _dashDuration;

        private float _damage;
        public float Damage => _damage;

        #endregion


        private void OnEnable()
        {
            MyAssert.IsNotNull(PlayerManager.Instance, "PlayerManager is null");

            pm = PlayerManager.Instance;
            pm.OnConfigChanged += ChangeConfig;
        }

        public void ChangeConfig(PlayerConfig config)
        {
            _health = config.Health;
            _currentHealth = _health;

            _healthBar.Init(this);

            _moveSpeed = config.MoveSpeed;
            _dashSpeed = config.DashSpeed;
            _dashDuration = config.DashDuration;
            _damage = config.Damage;

            movementComponent.SetMoveSettingsFromConfig(config);
            dashComponent.SetDashSettingsFromConfig(config);

            print($"moveSpeed: {_moveSpeed} // _dashSpeed: {_dashSpeed} // _dashDuration: {_dashDuration}");
        }

        public void ChangeAllModifiers()
        {
            movementComponent.IncreaseMoveSpeedModifier();
            dashComponent.IncreaseDashSpeedModifier();
            dashComponent.IncreaseDashDurationModifier();
        }

        public void TakeDamage(float incomingDamage)
        {
            _currentHealth -= incomingDamage;

            if (Mathf.Clamp(_currentHealth, 0, 100) <= 0)
            {
                OnPlayerDeath?.Invoke();
                return;
            }

            OnHealthChanged?.Invoke(_currentHealth);
        }

        private void OnDisable()
        {
            if (!pm) return;

            pm.OnConfigChanged -= ChangeConfig;
        }
    }
}