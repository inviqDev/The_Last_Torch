using System;
using UnityEngine;

namespace Runtime
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : Character
    {
        public Action OnNextAbilityIsAvailable;
        
        public Action<float, float> OnPlayerMaxHealthChanged;
        public Action<float> OnPlayerMoveSpeedChanged;
        
        public Action<int, float, float> OnPlayerLevelChanged;
        public Action<float> OnPlayerExpChanged;

        [SerializeField] private PlayerMovement movementComponent;
        [SerializeField] private PlayerDash playerDashComponent;
        [SerializeField] private DashUI dashUIComponent;
        
        [SerializeField] private PlayerAttack playerAttack;

        [SerializeField] private int levelsAmount;
        [SerializeField] private float firstLevelExp;
        [SerializeField] private float multiplier = 2f;
        public float[] levelsExp;
        private int _currentLevel = 1;
        private float _currentLevelMaxExp;
        
        public PlayerAttack PlayerAttack => playerAttack;

        #region Player_Stats

        // total statistics?
        // private float _totalCollectedExp;
        
        private float _currentExp;
        private float _dashSpeed;
        private float _dashDuration;

        #endregion

        private int GetCurrentLevelExpIndex()
        {
            return _currentLevel - 1;
        }

        private void SetUpLevelsExpArray()
        {
            levelsExp = new float[levelsAmount];
            levelsExp[GetCurrentLevelExpIndex()] = firstLevelExp;

            _currentExp = 0f;
            // _totalCollectedExp = 0f;
            _currentLevelMaxExp = levelsExp[GetCurrentLevelExpIndex()];

            var currentValue = firstLevelExp;
            for (var i = 1; i < levelsAmount; i++)
            {
                levelsExp[i] = currentValue * multiplier;
                currentValue = levelsExp[i];
            }
            
            OnNextAbilityIsAvailable?.Invoke();
            OnPlayerLevelChanged?.Invoke(_currentLevel, 0f, _currentLevelMaxExp);
        }

        public void SetUpPlayerConfig(PlayerConfig config)
        {
            SetUpLevelsExpArray();
            OnPlayerExpChanged?.Invoke(_currentExp);
            
            maxHealth = config.maxHealth;
            currentHealth = maxHealth;

            moveSpeed = config.moveSpeed;
            _dashSpeed = config.dashSpeed;
            _dashDuration = config.dashDuration;

            healthBar.Init(this);
            
            movementComponent.SetMoveSettingsFromConfig(config);
            playerDashComponent.SetDashSettingsFromConfig(config, dashUIComponent);
            
            OnPlayerMaxHealthChanged?.Invoke(maxHealth, currentHealth);
            OnPlayerMoveSpeedChanged?.Invoke(moveSpeed);
        }

        public void CollectExp(float pickedExpAmount)
        {
            _currentExp += pickedExpAmount;

            if (_currentExp >= _currentLevelMaxExp)
            {
                // save "delta"
                _currentExp -= _currentLevelMaxExp;
                
                // level up
                ++_currentLevel;
                _currentLevelMaxExp = levelsExp[GetCurrentLevelExpIndex()];
                
                ChangeStats(20f, 1f, 10f);
                
                OnNextAbilityIsAvailable?.Invoke();
                OnPlayerLevelChanged?.Invoke(_currentLevel, 0f, _currentLevelMaxExp);
            }

            OnPlayerExpChanged?.Invoke(_currentExp);
        }

        // public override void TakeDamage(float incomingDamage)
        // {
        //     base.TakeDamage(incomingDamage);
        // }

        public override void LaunchOnCharacterDeathLogic()
        {
            base.LaunchOnCharacterDeathLogic();
            OnCharacterDeath?.Invoke(this);
        }

        public void ChangeStats(float healthBoost, float moveSpeedBoost, float damageBoost)
        {
            if (healthBoost != 0f)
            {
                maxHealth += healthBoost;
                currentHealth += healthBoost;
                
                OnPlayerMaxHealthChanged?.Invoke(maxHealth, currentHealth);
            }

            if (moveSpeedBoost != 0f)
            {
                moveSpeed += moveSpeedBoost;
                movementComponent.SetNewMoveSpeed(moveSpeed);
                
                OnPlayerMoveSpeedChanged?.Invoke(moveSpeed);
            }

            if (damageBoost != 0f)
            {
                playerAttack.ChangeAbilitiesDamage(damageBoost);
            }
        }
    }
}