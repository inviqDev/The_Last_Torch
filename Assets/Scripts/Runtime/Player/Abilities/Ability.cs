using System;
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
        
        public Action<Ability> OnDamageValueChanged;

        private AbilityState state;
        
        private AbilitySlot _abilitySlot;
        private Timer _timer;
        
        private float _progressTime;
        
        public AbilityState State => state;
        
        public string Name { get; private set; }
        public Sprite Icon { get; private set; }
        
        public string SoundPoolKey { get; private set; }
        public string ParticlesPoolKey { get; private set; }
        
        public float MinAttackDistance { get; private set; }
        public float Cooldown { get; private set; }
        public float Damage { get; private set; }
        
        public AbilityVFX AbilityVFX { get; private set; }
        
        public Ability(AbilityConfig config) 
        {
            Name = config.abilityName;
            Icon = config.abilityIcon;

            SoundPoolKey = config.SoundUniquePoolKey;
            ParticlesPoolKey = config.ParticlesUniquePoolKey;
            
            MinAttackDistance = config.maxAttackDistance;
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
                {
                    state = AbilityState.Ready;
                }
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
                    break;
                case AbilityState.InProgress:
                    _abilitySlot.SetInProgressUIState(true, false);
                    // _timer.StartFromToTimer(0f, _progressTime, TimerType.Increasing);
                    break;
                
                case AbilityState.OnCooldown:
                    _abilitySlot.SetInProgressUIState(false, true);
                    _timer?.StartFromToTimer(0f, Cooldown, TimerType.Increasing);
                    break;
            }
        }
        
        public void UpdateAbility(float increment, float decrement)
        {
            Damage += increment;
            Cooldown /= decrement;
            
            OnDamageValueChanged?.Invoke(this);
        }
    }
}