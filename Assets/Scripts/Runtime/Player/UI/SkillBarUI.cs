using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class SkillBarUI : MonoBehaviour
    {
        [SerializeField] private SkillConfig config;
        [SerializeField] private Slider _cooldownSlider;
        [SerializeField] private Image _image;
        
        private readonly float minValue = 0f;
        private float maxValue;
        
        private Timer skillTimer;

        public void Init()
        {
            _image.sprite = config.logo;
            
            _cooldownSlider.minValue = minValue;
            _cooldownSlider.maxValue = config.cooldownTime;
            
            maxValue = config.cooldownTime;
            
            _cooldownSlider.value = minValue;

            skillTimer = new Timer(this);
            skillTimer.OnAnyValueChanged += ShowSkillCooldown;
            skillTimer.TimerIsOver += OnTimerIsOver;
        }

        private void SetStartValues()
        {
            _cooldownSlider.value = minValue;
            skillTimer.StartFromToTimer(minValue, maxValue, TimerType.Increasing);
        }

        private void OnTimerIsOver()
        {
            print("USE AGAIN !");
            
            skillTimer.StopTimer();
            SetStartValues();
        }

        private void Start()
        {
            Init();
            SetStartValues();
        }
        
        private void ShowSkillCooldown(float currentTimerValue)
        {
            _cooldownSlider.value = currentTimerValue;
        }
        
        
        // on attack => set value to 0 => start timer => onCooldownIsOver => set "skill active"
    }
}