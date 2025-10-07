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

        [SerializeField] private EnemiesDetector detector;

        [SerializeField] private int levelsAmount;
        [SerializeField] private float firstLevelExp;
        [SerializeField] private float A;
        [SerializeField] private float B;

        private PlayerMovement _movementComponent;
        private PlayerDash _playerDashComponent;
        private DashUI _dashUIComponent;
        private PlayerAttack _playerAttack;
        private CameraMover _cameraMover;

        private float _currentExp;

        public EnemiesDetector Detector => detector;
        public int CurrentLevel { get; private set; }
        public float CurrentLevelMaxExp { get; private set; }

        public float[] levelsExp;

        public void InitializePlayerComponents()
        {
            _movementComponent = GetComponent<PlayerMovement>();
            UnityEngine.Assertions.Assert.IsNotNull(_movementComponent,
                "player movement component is missing");

            _playerDashComponent = GetComponent<PlayerDash>();
            UnityEngine.Assertions.Assert.IsNotNull(_playerDashComponent,
                "player dash component is missing");

            _dashUIComponent = GetComponent<DashUI>();
            UnityEngine.Assertions.Assert.IsNotNull(_dashUIComponent,
                "player dash component UI [slider] is missing");

            _cameraMover = GetComponent<CameraMover>();
            UnityEngine.Assertions.Assert.IsNotNull(_cameraMover,
                "player camera mover component is missing");
            _cameraMover?.Initialize(GameManager.Instance.CameraMain, transform);

            UnityEngine.Assertions.Assert.IsNotNull(detector,
                "enemies detector component is missing");
            detector?.Initialize();

            _playerAttack = GetComponent<PlayerAttack>();
            UnityEngine.Assertions.Assert.IsNotNull(_playerAttack,
                "player attack component is missing");
            _playerAttack?.Initialize(this);
            
            SetUpLevelsExpArray();
        }

        private int GetCurrentLevelExpIndex()
        {
            return Mathf.Clamp(CurrentLevel - 1, 0, Mathf.Max(0, levelsAmount - 1));
        }

        private void SetUpLevelsExpArray()
        {
            levelsAmount = Mathf.Max(1, levelsAmount);
            levelsExp = new float[levelsAmount];

            levelsExp[0] = firstLevelExp;
            for (var n = 1; n < levelsAmount; n++)
            {
                var d = firstLevelExp + A * n + B * n * n;
                levelsExp[n] = Mathf.Round(d);
            }
            
            CurrentLevel = 1;
            _currentExp = 0f;
            CurrentLevelMaxExp = levelsExp[GetCurrentLevelExpIndex()];

            OnPlayerLevelChanged?.Invoke(this);
        }

        public void SetUpPlayerConfig(PlayerConfig config)
        {
            maxHealth = config.maxHealth;
            currentHealth = maxHealth;

            moveSpeed = config.moveSpeed;
            _movementComponent.SetMoveSettingsFromConfig(config);
            _playerDashComponent.SetDashSettingsFromConfig(config, _dashUIComponent);

            healthBar.Initialize(this);

            OnPlayerMaxHealthChanged?.Invoke(maxHealth, currentHealth);
            OnPlayerMoveSpeedChanged?.Invoke(moveSpeed);
            OnPlayerExpChanged?.Invoke(_currentExp);
        }

        public void CollectExp(float pickedExpAmount)
        {
            _currentExp += pickedExpAmount;

            if (CurrentLevel >= levelsAmount)
            {
                _currentExp = Mathf.Min(_currentExp, CurrentLevelMaxExp);
                OnPlayerExpChanged?.Invoke(_currentExp);
                return;
            }

            while (CurrentLevel < levelsAmount && _currentExp >= CurrentLevelMaxExp)
            {
                _currentExp -= CurrentLevelMaxExp;
                ++CurrentLevel;

                if (CurrentLevel >= levelsAmount)
                {
                    // add extra credits or something
                    CurrentLevelMaxExp = levelsExp[GetCurrentLevelExpIndex()];
                    _currentExp = Mathf.Min(_currentExp, CurrentLevelMaxExp);

                    OnPlayerLevelChanged?.Invoke(this);
                    OnPlayerExpChanged?.Invoke(_currentExp);

                    return;
                }

                CurrentLevelMaxExp = levelsExp[GetCurrentLevelExpIndex()];
                OnPlayerLevelChanged?.Invoke(this);
            }

            OnPlayerExpChanged?.Invoke(_currentExp);
        }

        protected override void LaunchOnCharacterDeathLogic()
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
                _movementComponent.SetNewMoveSpeed(moveSpeed);

                OnPlayerMoveSpeedChanged?.Invoke(moveSpeed);
            }

            if (damageBoost != 0f)
            {
                _playerAttack.ChangeAbilitiesStats(damageBoost);
            }
        }
    }
}