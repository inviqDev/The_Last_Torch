using System;
using System.Collections;
using UnityEngine;

namespace Runtime
{
    public enum DashState
    {
        Ready,
        InProgress,
        OnCooldown,
    }
    public class PlayerDash : Movement
    {
        public event Action OnDashFinished;
        
        private float _speed;
        private float _duration;
        private float _cooldown;

        private DashUI _dashUI;
        private Timer _timer;
        private IEnumerator _routine;
        
        public DashState dashState { get; private set; }

        protected override void Initialize()
        {
            base.Initialize();
            enabled = false;
        }

        public void SetDashSettingsFromConfig(PlayerConfig config, DashUI dashUI)
        {
            _speed = config.dashSpeed;
            _duration = config.dashDuration;
            _cooldown = config.dashCooldown;
            
            
            _timer = new Timer(this);
            _timer.TimerIsOver += ChangeDashState;
            
            Initialize();
            _dashUI = dashUI;
            _dashUI.InitializeAbilityUI(_timer, _cooldown, _duration);
            
            dashState = DashState.OnCooldown;
            _dashUI.UpdateDashState(dashState);
            _timer.StartFromToTimer(0f, _cooldown, TimerType.Increasing);
        }

        private void ChangeDashState()
        {
            switch (dashState)
            {
                case DashState.InProgress:
                    StopMovement();
                    
                    dashState = DashState.OnCooldown;
                    _dashUI.UpdateDashState(dashState);
                    
                    _timer.StartFromToTimer(0, _cooldown, TimerType.Increasing);
                    OnDashFinished?.Invoke();
                    break;
                
                case DashState.OnCooldown: 
                    dashState = DashState.Ready;
                    break;
            }
        }

        public override void StartMovement(Vector3 dir)
        {
            if (dashState is DashState.InProgress or DashState.OnCooldown) return;
            
            direction = dir.normalized * _speed;
            playerRotation.StopFacing();
            
            _timer.TimerIsOver -= ChangeDashState;
            _timer.TimerIsOver += ChangeDashState;
            
            dashState = DashState.InProgress;
            _dashUI.UpdateDashState(dashState);
            _timer.StartFromToTimer(_duration, 0f, TimerType.Decreasing);
            
            enabled = true;
        }

        public override void Move(Vector3 dir)
        {
        }

        public override void StopMovement()
        {
            direction = Vector3.zero;
            dashState = DashState.OnCooldown;
            
            _timer.TimerIsOver -= ChangeDashState;
            _timer.TimerIsOver += ChangeDashState;
            _timer.StartFromToTimer(0, _cooldown, TimerType.Increasing);
            
            enabled = false;
        }
    }
}