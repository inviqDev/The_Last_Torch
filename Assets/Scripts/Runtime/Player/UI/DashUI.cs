using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class DashUI : MonoBehaviour
    {
        [SerializeField] private Image fillingIcon;
        [SerializeField] private Slider progressBar;

        private DashState _dashState;
        
        private Timer _timer;
        private float _dashCooldown;
        private float _dashDuration;

        public void InitDashAbilityUI(Timer timer, float cooldown, float duration)
        {
            fillingIcon.fillAmount = 0f;
            progressBar.value = 0f;

            _timer = timer;
            _dashCooldown = cooldown;
            _dashDuration = duration;
            
            _timer.OnAnyValueChanged += ShowCooldownProgress;
        }

        private void ShowCooldownProgress(float value)
        {
            switch (_dashState)
            {
                case DashState.InProgress:
                    fillingIcon.fillAmount = value / _dashDuration;
                    progressBar.value = value / _dashDuration;
                    break;
                case DashState.OnCooldown:
                    fillingIcon.fillAmount = value / _dashCooldown;
                    progressBar.value = value / _dashCooldown;
                    break;
            }
        }

        public void UpdateDashState(DashState dashState) => _dashState = dashState;

        private void OnDisable()
        {
            _timer.OnAnyValueChanged -= ShowCooldownProgress;
        }
    }
}