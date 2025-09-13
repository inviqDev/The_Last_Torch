using UnityEngine;

namespace Runtime
{
    public class Ability
    {
        public enum AbilityState
        {
            None,
            Ready,
            InProgress,
            OnCooldown,
        }

        private AbilityState state;
        
        private string _abilityName;
        private Sprite _abilityIcon;
        
        private readonly AbilitySlot _abilitySlot;
        private readonly Timer _timer;
        
        private float _minAttackDistance;
        private AbilityVFX _abilityVFX;
        
        private float _progressTime;
        private float _cooldownTime;
        private float _damage;
        
        public AbilityState State => state;
        public float MinAttackDistance => _minAttackDistance;
        public AbilityVFX AbilityVFX => _abilityVFX;
        public float Damage => _damage;
        
        public Ability(MonoBehaviour owner, AbilityConfig config, AbilitySlot abilitySlot)
        {
            state = AbilityState.OnCooldown;
            
            _abilityName = config.abilityName;
            _abilityIcon = config.abilityIcon;

            _abilityVFX = config.abilityVFX;
            _minAttackDistance = config.minAttackDistance;
            
            _progressTime = config.progressTime;
            _cooldownTime = config.cooldownTime;
            _damage = config.damage;
            
            _abilitySlot = abilitySlot;
            InitAbilityUI(config);
            
            _timer = new Timer(owner);
            _timer.OnAnyValueChanged += OnCooldownValueChanged;
            
            _timer.TimerIsOver += () =>
            {
                if (state == AbilityState.OnCooldown)
                    state = AbilityState.Ready;
            };

            SetAbilityState(AbilityState.OnCooldown);
        }

        private void InitAbilityUI(AbilityConfig config)
        {
            _abilitySlot.SetUpAbilityUI(config, true);
        }
        
        private void OnCooldownValueChanged(float currentCooldown)
        {
            _abilitySlot.ShowCooldownProgress(currentCooldown);
        }

        public void SetAbilityState(AbilityState newState)
        {
            state = newState;

            switch (state)
            {
                case AbilityState.InProgress:
                    _timer.StopTimer();
                    break;
                case AbilityState.OnCooldown:
                    _timer.StartFromToTimer(0f, _cooldownTime, TimerType.Increasing);
                    break;
            }
        }

        public void ChangeDamageValue(float value)
        {
            _damage += value;
        }
    }
}