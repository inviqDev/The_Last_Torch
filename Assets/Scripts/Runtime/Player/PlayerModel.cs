#region usings

using System;
using UnityEngine;
using MyAssert = UnityEngine.Assertions.Assert;

#endregion

namespace Runtime
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerModel : CharacterBase
    {
        public Action OnPlayerDeath;

        // [SerializeField] private HealthBar _healthBar;
        [SerializeField] private PlayerMovement movementComponent;
        [SerializeField] private Dash dashComponent;

        [SerializeField] private PlayerAttack playerAttack;
        public PlayerAttack PlayerAttack => playerAttack;

        private PlayerManager pm;

#region Player_Stats
        
        private float _dashSpeed;
        private float _dashDuration;

#endregion

        private void OnEnable()
        {
            MyAssert.IsNotNull(PlayerManager.Instance, "PlayerManager is null");

            pm = PlayerManager.Instance;
            pm.OnConfigChanged += ChangeConfig;
        }

        public void ChangeConfig(PlayerConfig config)
        {
            health = config.maxHealth;
            currentHealth = health;

            moveSpeed = config.moveSpeed;
            _dashSpeed = config.dashSpeed;
            _dashDuration = config.dashDuration;
            
            healthBar.Init(this);

            movementComponent.SetMoveSettingsFromConfig(config);
            dashComponent.SetDashSettingsFromConfig(config);

            print($"moveSpeed: {moveSpeed} // _dashSpeed: {_dashSpeed} // _dashDuration: {_dashDuration}");
        }

        public void ChangeAllModifiers()
        {
            movementComponent.IncreaseMoveSpeedModifier();
            dashComponent.IncreaseDashSpeedModifier();
            dashComponent.IncreaseDashDurationModifier();
        }

        public override void TakeDamage(float incomingDamage)
        {
            base.TakeDamage(incomingDamage);
            if (!(currentHealth <= 0)) return;

            LaunchOnPlayerDeathLogic();
        }

        private void LaunchOnPlayerDeathLogic()
        {
            print("PLAYER IS DEAD !");
            gameObject.SetActive(false);
            OnPlayerDeath?.Invoke();
        }

        private void OnDisable()
        {
            if (!pm) return;

            pm.OnConfigChanged -= ChangeConfig;
        }
    }
}