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
        private float _cooldown;
        private GameObject _abilityVFX;
        
        private readonly AbilityUI _abilityUI;
        private readonly Timer _timer;
        
        public AbilityState State => state;
        public GameObject AbilityVFX => _abilityVFX;
        public float Damage => _damage;
        
        public Ability(MonoBehaviour owner, AbilityConfig config, AbilityUI abilityUI)
        {
            state = AbilityState.None;
            
            _abilityName = config.abilityName;
            _abilityIcon = config.abilityIcon;

            _damage = config.damage;
            _cooldown = config.cooldown;
            _abilityVFX = config.abilityPrefab;
            
            _abilityUI = abilityUI;
            InitAbilityUI(config);
            
            _timer = new Timer(owner);
            _timer.OnAnyValueChanged += OnCooldownValueChanged;
            _timer.TimerIsOver += SetAbilityIsReadyState;

            _timer.StartFromToTimer(0f, config.cooldown, TimerType.Increasing);
        }

        private void InitAbilityUI(AbilityConfig config)
        {
            _abilityUI.SetUpAbilityUI(config, true);
        }
        
        private void OnCooldownValueChanged(float currentCooldown)
        {
            _abilityUI.ShowCooldownProgress(currentCooldown);
        }

        private void SetAbilityIsReadyState()
        {
            state = AbilityState.Ready;
        }

        public void SetAbilityState(AbilityState newState)
        {
            state = newState;

            if (state == AbilityState.InProgress)
            {
                
            }
            else if (state == AbilityState.OnCooldown)
            {
                _timer.StartFromToTimer(0f, _cooldown, TimerType.Increasing);
            }
        }
    }
}