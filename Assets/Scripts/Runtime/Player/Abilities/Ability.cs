using System;
using UnityEngine;

namespace Runtime
{
    public enum AbilityState
    {
        Disable,
        InProgress,
        OnCooldown,
        Ready,
    }
    
    public class Ability
    {
        public Action<AbilityState> OnAbilityStateChange;
        public Action<Ability> OnAbilityStatsChanged;

        private AbilityState state;
        
        // private AbilitySlot _abilitySlot;
        
        
        private float _progressTime;
        
        public AbilityState State => state;
        
        public string Name { get; private set; }
        public Sprite Icon { get; private set; }
        
        public string SoundPoolKey { get; private set; }
        public string ParticlesPoolKey { get; private set; }
        
        public float MinAttackDistance { get; private set; }
        public float Cooldown { get; private set; }
        public float Damage { get; private set; }

        public Timer Timer { get; private set; }
        public AbilityVFX AbilityVFX { get; private set; }
        
        public Ability(AbilityConfig config) 
        {
            SetAbilityState(AbilityState.Disable);
            
            Name = config.abilityName;
            Icon = config.abilityIcon;

            SoundPoolKey = config.SoundUniquePoolKey;
            ParticlesPoolKey = config.ParticlesUniquePoolKey;
            
            MinAttackDistance = config.maxAttackDistance;
            Cooldown = config.cooldownTime;
            Damage = config.damage;

            AbilityVFX = config.abilityVFX;
            _progressTime = config.progressTime;
        }

        public void ActivateAbility(MonoBehaviour owner)
        {
            Timer = new Timer(owner);
            
            Timer.TimerIsOver += () =>
            {
                if (state != AbilityState.OnCooldown) return;
                SetAbilityState(AbilityState.Ready);
            };

            SetAbilityState(AbilityState.OnCooldown);
        }

        public void SetAbilityState(AbilityState newState)
        {
            state = newState;

            switch (state)
            {
                case AbilityState.Disable:
                    break;
                
                case AbilityState.InProgress:
                    OnAbilityStateChange?.Invoke(AbilityState.InProgress);
                    break;
                
                case AbilityState.OnCooldown:
                    OnAbilityStateChange?.Invoke(AbilityState.OnCooldown);
                    Timer?.StartFromToTimer(0f, Cooldown, TimerType.Increasing);
                    break;
                
                case AbilityState.Ready:
                    OnAbilityStateChange?.Invoke(AbilityState.Ready);
                    break;
            }
        }
        
        public void UpdateAbility(float increment, float decrement)
        {
            Damage += increment;
            Cooldown /= decrement;
            
            OnAbilityStatsChanged?.Invoke(this);
        }
    }
}