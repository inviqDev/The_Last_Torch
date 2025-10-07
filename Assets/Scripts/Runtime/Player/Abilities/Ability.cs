using System;
using UnityEngine;

namespace Runtime
{
    public enum AbilityState
    {
        Deactivated,
        InProgress,
        OnCooldown,
        Ready,
    }

    public enum StatChangeMode
    {
        /// <summary>Simple additive change: new = current + value.</summary>
        SimpleAdd,
        /// <summary>Percentage change. E.G: Pass +10 for +10%, -10 for -10%.</summary>
        Percent,
        /// <summary>Set exact value: new = value.</summary>
        SetValue
    }

    public class Ability : IDisposable
    {
        public Action<Ability> OnAbilityReady;
        public Action<AbilityState> OnAbilityStateChange;
        public Action<Ability> OnAbilityStatsChanged;

        private AbilityState state;
        private readonly float _baseDamage;
        private readonly float _baseCooldown;
        // private readonly float _progressTime;

        private bool _disposed;

        public AbilityState State => state;

        public string Name { get; private set; }
        public Sprite Icon { get; private set; }

        public string HitSfxPoolKey { get; private set; }
        public string HitVfxPoolKey { get; private set; }

        public float AttackRange { get; private set; }
        public float Damage { get; private set; }
        public float Cooldown { get; private set; }

        public Timer Timer { get; private set; }
        public AbilityVfx AbilityVfx { get; private set; }

        public Ability(AbilityConfig config)
        {
            SetAbilityState(AbilityState.Deactivated);

            Name = config.abilityName;
            Icon = config.abilityIcon;

            _baseDamage = config.damage;
            Damage = ResetDamageToDefault();
            AttackRange = config.maxAttackDistance;

            _baseCooldown = config.cooldownTime;
            Cooldown = ResetCooldownToDefault();
            
            // _progressTime = config.progressTime;
            
            AbilityVfx = config.abilityVfx;
            HitSfxPoolKey = config.HitSoundPoolKey;
            HitVfxPoolKey = config.HitParticlePoolKey;
        }

        public void ActivateAbility(MonoBehaviour owner)
        {
            ThrowIfDisposed();
            
            Timer?.Dispose();
            Timer = new Timer(owner);
            
            Timer.TimerIsOver += SetReadyAbilityState;
            SetAbilityState(AbilityState.OnCooldown);
        }

        public void DeactivateAbility()
        {
            ThrowIfDisposed();
            
            if (Timer == null) return;
            Timer.TimerIsOver -= SetReadyAbilityState;
            Timer.Dispose();
            Timer = null;
        }

        private void SetReadyAbilityState()
        {
            SetAbilityState(AbilityState.Ready);
        }

        public void SetAbilityState(AbilityState newState)
        {
            ThrowIfDisposed();
            state = newState;

            switch (state)
            {
                case AbilityState.Deactivated:
                    DeactivateAbility();
                    break;

                case AbilityState.InProgress:
                    OnAbilityStateChange?.Invoke(AbilityState.InProgress);
                    // set this state very time an ability is launched =>
                    // set on cooldown state after progress time is passed
                    break;

                case AbilityState.OnCooldown:
                    OnAbilityStateChange?.Invoke(AbilityState.OnCooldown);
                    Timer?.StartFromToTimer(0f, Cooldown, TimerType.Increasing);
                    break;

                case AbilityState.Ready:
                    OnAbilityStateChange?.Invoke(AbilityState.Ready);
                    OnAbilityReady?.Invoke(this);
                    break;
            }
        }

        /// <summary>
        /// Changes cooldown using one of three modes:
        /// <list type="bullet">
        /// <item><description><b>SimpleAdd</b>: new = current + value (value in seconds; negative to reduce).</description></item>
        /// <item><description><b>Percent</b>:  new = current * (1 + value/100). E.G: +10 => +10%, -20 => -20%.</description></item>
        /// <item><description><b>SetValue</b>: new = value (seconds).</description></item>
        /// </list>
        /// Notes:
        /// - Cooldown is clamped to be non-negative (min 0). No hidden hard caps.
        /// - Returns the new cooldown.
        /// </summary>
        public float ChangeAbilityCooldown(StatChangeMode mode, float value)
        {
            var newValue = mode switch
            {
                StatChangeMode.SimpleAdd => Cooldown + value,
                StatChangeMode.Percent => Cooldown * (1f + value / 100f),
                StatChangeMode.SetValue => value,
                _ => Cooldown
            };

            if (float.IsNaN(newValue) || newValue < 0f) newValue = 0f;
            if (Mathf.Approximately(newValue, Cooldown)) return Cooldown;

            Cooldown = newValue;
            OnAbilityStatsChanged?.Invoke(this);

            return Cooldown;
        }

        /// <summary>
        /// Changes damage using one of three modes:
        /// <list type="bullet">
        /// <item><description><b>SimpleAdd</b>: new = current + value (raw points; negative allowed).</description></item>
        /// <item><description><b>Percent</b>:  new = current * (1 + value/100). For example, +25 => +25%, -10 => -10%.</description></item>
        /// <item><description><b>SetValue</b>: new = value.</description></item>
        /// </list>
        /// Notes:
        /// - Damage is clamped to be non-negative (min 0). No hidden hard caps.
        /// - Returns the new damage.
        /// </summary>
        public float ChangeAbilityDamage(StatChangeMode mode, float value)
        {
            var newValue = mode switch
            {
                StatChangeMode.SimpleAdd => Damage + value,
                StatChangeMode.Percent => Damage * (1f + value / 100f),
                StatChangeMode.SetValue => value,
                _ => Damage
            };

            if (float.IsNaN(newValue) || newValue < 0f) newValue = 0f;
            if (Mathf.Approximately(newValue, Damage)) return Damage;

            Damage = newValue;
            OnAbilityStatsChanged?.Invoke(this);

            return Damage;
        }

        /// <summary>
        /// Resets cooldown back to the value from config.
        /// Returns the new cooldown.
        /// </summary>
        public float ResetCooldownToDefault() => ChangeAbilityCooldown(StatChangeMode.SetValue, _baseCooldown);

        /// <summary>
        /// Resets damage back to the value from config.
        /// Returns the new damage.
        /// </summary>
        public float ResetDamageToDefault() => ChangeAbilityDamage(StatChangeMode.SetValue, _baseDamage);

        private void ThrowIfDisposed()
        {
            if (_disposed) 
                throw new ObjectDisposedException(nameof(Ability));
        }
        public void Dispose()
        {
            if (!_disposed) return;
            
            Timer?.Dispose();
            Timer = null;
            
            OnAbilityReady = null;
            OnAbilityStateChange = null;
            OnAbilityStatsChanged = null;
            
            _disposed = true;
        }
    }
}