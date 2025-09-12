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

        private float _damage;
        private float _progressTime;
        private float _cooldownTime;
        
        private AbilityVFX _abilityVFX;
        
        private readonly AbilitySlot _abilitySlot;
        private readonly Timer _timer;
        
        public AbilityState State => state;
        public AbilityVFX AbilityVFX => _abilityVFX;
        public float Damage => _damage;
        
        public Ability(MonoBehaviour owner, AbilityConfig config, AbilitySlot abilitySlot)
        {
            state = AbilityState.None;
            
            _abilityName = config.abilityName;
            _abilityIcon = config.abilityIcon;

            _abilityVFX = config.abilityVFX;
            _progressTime = config.progressTime;
            _cooldownTime = config.cooldownTime;
            _damage = config.damage;
            
            _abilitySlot = abilitySlot;
            InitAbilityUI(config);
            
            _timer = new Timer(owner);
            _timer.OnAnyValueChanged += OnCooldownValueChanged;
            _timer.TimerIsOver += SetAbilityIsReadyState;

            _timer.StartFromToTimer(0f, config.cooldownTime, TimerType.Increasing);
        }

        private void InitAbilityUI(AbilityConfig config)
        {
            _abilitySlot.SetUpAbilityUI(config, true);
        }
        
        private void OnCooldownValueChanged(float currentCooldown)
        {
            _abilitySlot.ShowCooldownProgress(currentCooldown);
        }

        private void SetAbilityIsReadyState()
        {
            state = AbilityState.Ready;
        }

        public void SetAbilityState(AbilityState newState)
        {
            state = newState;

            switch (state)
            {
                case AbilityState.InProgress:
                    _timer.StartFromToTimer(0f, _progressTime, TimerType.Increasing);
                    break;
                case AbilityState.OnCooldown:
                    _timer.StartFromToTimer(0f, _cooldownTime, TimerType.Increasing);
                    break;
            }
        }
    }
}