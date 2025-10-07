using System.Collections;
using UnityEngine;

namespace Runtime._my_tests
{
    public enum FireplaceStatus
    {
        Deactivated,
        Activated,
    }
    public class FireplaceProgressBar : ProgressBar
    {
        [SerializeField, Range(1, 10)] private int totalActivationTime;
        
        private Timer _timer;
        private float _currentTime;
        private FireplaceStatus _status;

        private Coroutine _timerRoutine;

        protected override void Start()
        {
            base.Start();
        
            _status = FireplaceStatus.Deactivated;
        
            _timer ??= new Timer(this);
            _currentTime = 0.0f;
        
            _timer.OnAnyValueChanged += UpdateProgressBar;
            _timer.TimerIsOver += FinishActivation;
        }

        private void OnEnable()
        {
            _currentTime = 0.0f;
            
            _timer ??= new Timer(this);
            _timer.StopTimer();
            
            _timer.OnAnyValueChanged -= UpdateProgressBar;
            _timer.OnAnyValueChanged += UpdateProgressBar;
                
            _timer.TimerIsOver -= FinishActivation;
            _timer.TimerIsOver += FinishActivation;
        }

        private void OnTriggerEnter(Collider other)
        {
            var isPlayer = other.CompareTag(GameManager.Instance?.Player.tag);
            var alreadyActive = _status == FireplaceStatus.Activated;
            if (!isPlayer && alreadyActive) return;
        
            if (_timerRoutine != null)
            {
                StopCoroutine(_timerRoutine);
            }

            UnityEngine.Assertions.Assert.IsNotNull(_timer, "Timer is missing");
            if (_timer == null) return;
            
            _timer.StartFromToTimer(_currentTime, totalActivationTime, TimerType.Increasing);
            ShowProgressBar(_currentTime);
        }

        private void UpdateProgressBar(float newValue)
        {
            _currentTime = newValue;
            FillProgressBar(_currentTime / totalActivationTime);
        }

        private void FinishActivation()
        {
            // launch specific fireplace logic => able to upgrade player ??
            _status = FireplaceStatus.Activated;
            HideProgressBar();
        }

        private void OnTriggerExit(Collider other)
        {
            UnityEngine.Assertions.Assert.IsNotNull(
                GameManager.Instance?.Player, "Player is missing");
            if (!other.CompareTag(GameManager.Instance?.Player?.tag)) return;

            UnityEngine.Assertions.Assert.IsNotNull(_timer, "Timer is missing");
            if (_timer == null) return;
            
            _timer.StopTimer();
            _timerRoutine = StartCoroutine(DecreaseProgressBarRoutine());
        }

        private IEnumerator DecreaseProgressBarRoutine()
        {
            var current = _currentTime;
            while (current > 0.0f)
            {
                current -= Time.deltaTime;
                UpdateProgressBar(_currentTime);
                yield return null;
            }

            _currentTime = 0.0f;
            _timerRoutine = null;
            HideProgressBar();
        }

        private void OnDisable()
        {
            if (_timer == null) return;
            
            _timer.OnAnyValueChanged -= UpdateProgressBar;
            _timer.TimerIsOver -= FinishActivation;
            
            _timer.Dispose();
            _timer = null;
        }
    }
}