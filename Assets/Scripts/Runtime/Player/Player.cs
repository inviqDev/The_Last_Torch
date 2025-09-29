using System;
using UnityEngine;

namespace Runtime
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : Character
    {
        public Action<float, float> OnPlayerMaxHealthChanged;
        public Action<float> OnPlayerMoveSpeedChanged;
        public Action<Player> OnPlayerLevelChanged;
        public Action<float> OnPlayerExpChanged;

        [SerializeField] private PlayerMovement movementComponent;
        [SerializeField] private PlayerDash playerDashComponent;
        [SerializeField] private DashUI dashUIComponent;
        
        [SerializeField] private PlayerAttack playerAttack;

        [SerializeField] private int levelsAmount;
        [SerializeField] private float firstLevelExp;
        [SerializeField] private float multiplier = 1.5f;
        private float _currentExp;

        public PlayerAttack PlayerAttack => playerAttack;

        public int CurrentLevel { get; private set; } = 1;
        public float CurrentLevelMaxExp { get; private set; }
        
        public float[] levelsExp;

        private int GetCurrentLevelExpIndex()
        {
            return CurrentLevel - 1;
        }

        private void SetUpLevelsExpArray()
        {
            levelsExp = new float[levelsAmount];
            levelsExp[GetCurrentLevelExpIndex()] = firstLevelExp;

            _currentExp = 0f;
            // _totalCollectedExp = 0f;
            CurrentLevelMaxExp = levelsExp[GetCurrentLevelExpIndex()];

            var currentValue = firstLevelExp;
            for (var i = 1; i < levelsAmount; i++)
            {
                levelsExp[i] = currentValue * multiplier;
                currentValue = levelsExp[i];
            }
            
            OnPlayerLevelChanged?.Invoke(this);
        }

        public void SetUpPlayerConfig(PlayerConfig config)
        {
            SetUpLevelsExpArray();
            OnPlayerExpChanged?.Invoke(_currentExp);
            
            maxHealth = config.maxHealth;
            currentHealth = maxHealth;
            
            moveSpeed = config.moveSpeed;
            movementComponent.SetMoveSettingsFromConfig(config);
            playerDashComponent.SetDashSettingsFromConfig(config, dashUIComponent);
            
            healthBar.Init(this);
            
            OnPlayerMaxHealthChanged?.Invoke(maxHealth, currentHealth);
            OnPlayerMoveSpeedChanged?.Invoke(moveSpeed);
        }

        public void CollectExp(float pickedExpAmount)
        {
            _currentExp += pickedExpAmount;

            if (_currentExp >= CurrentLevelMaxExp)
            {
                _currentExp -= CurrentLevelMaxExp;
                
                ++CurrentLevel;
                CurrentLevelMaxExp = levelsExp[GetCurrentLevelExpIndex()];
                
                ChangeStats(20f, 1f, 10f);
                
                OnPlayerLevelChanged?.Invoke(this);
            }

            OnPlayerExpChanged?.Invoke(_currentExp);
        }

        public override void LaunchOnCharacterDeathLogic()
        {
            // player "unique player" anim, sound, etc
            print(gameObject.name + " IS DEAD");
            
            base.LaunchOnCharacterDeathLogic();
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
                playerAttack.UpdateAbilitiesStats(damageBoost);
            }
        }
    }
}