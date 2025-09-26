using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class DashUI : MonoBehaviour
    {
        [SerializeField] private Image fillingIcon;
        [SerializeField] private Slider progressBar;

        private Timer _timer;
        private float _abilityCooldown;

        public void InitDashAbilityUI(Timer timer, float cooldown)
        {
            fillingIcon.fillAmount = 0f;
            progressBar.value = 0f;
            
            _timer = timer;
            _abilityCooldown = cooldown;
            
            _timer.OnAnyValueChanged += ShowCooldownAbilityProgress;
        }

        private void ShowCooldownAbilityProgress(float value)
        {
            fillingIcon.fillAmount = value / _abilityCooldown;
            progressBar.value = value / _abilityCooldown;
        }

        private void OnDisable()
        {
            _timer.OnAnyValueChanged -= ShowCooldownAbilityProgress;
        }
    }
}