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
        
        private AbilitySlot _abilitySlot;
        private Timer _timer;
        
        private float _progressTime;
        
        public AbilityState State => state;
        
        public string Name { get; private set; }
        public Sprite Icon { get; private set; }
        
        public float MinAttackDistance { get; private set; }
        public float Cooldown { get; private set; }
        public float Damage { get; private set; }
        
        public AbilityVFX AbilityVFX { get; private set; }
        
        public Ability(AbilityConfig config) //, AbilitySlot abilitySlot)
        {
            Name = config.abilityName;
            Icon = config.abilityIcon;
            
            MinAttackDistance = config.minAttackDistance;
            Cooldown = config.cooldownTime;
            Damage = config.damage;

            AbilityVFX = config.abilityVFX;
            _progressTime = config.progressTime;
            
            SetAbilityState(AbilityState.None);
        }

        public void ActivateAbility(MonoBehaviour owner, AbilitySlot slot)
        {
            _abilitySlot = slot;
            
            _timer = new Timer(owner);
            _timer.OnAnyValueChanged += OnCooldownValueChanged;
            
            _timer.TimerIsOver += () =>
            {
                if (state == AbilityState.OnCooldown)
                    state = AbilityState.Ready;
            };

            SetAbilityState(AbilityState.OnCooldown);
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
                case AbilityState.None:
                case AbilityState.InProgress:
                    _timer?.StopTimer();
                    break;
                case AbilityState.OnCooldown:
                    _timer?.StartFromToTimer(0f, Cooldown, TimerType.Increasing);
                    break;
            }
        }
        
        public void ChangeDamageValue(float increment)
        {
            Damage += increment;
        }
    }
}