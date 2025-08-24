using System.Collections;
using UnityEngine;

public enum FireplaceStatus
{
    Deactivated,
    Activated,
}
public class FireplaceProgressBar : ProgressBar
{
    [SerializeField, Range(1, 10)] private int totalActivationTime;
    
    private Timer _timer;
    private float _currentActivationTime;
    private FireplaceStatus _status;

    private IEnumerator _backToZeroRoutine;


    protected override void Start()
    {
        base.Start();
        
        _status = FireplaceStatus.Deactivated;
        
        _timer ??= new Timer(this);
        _currentActivationTime = 0.0f;
        
        _timer.OnAnyValueChanged += OnAnyTimerValueChanged;
        _timer.TimerIsOver += OnActivationIsComplete;
    }

    private void OnEnable()
    {
        if (_timer != null)
        {
            _timer ??= new Timer(this);
            _currentActivationTime = 0.0f;
            
            _timer.OnAnyValueChanged += OnAnyTimerValueChanged;
            _timer.TimerIsOver += OnActivationIsComplete;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var isPlayer = other.CompareTag(PlayerManager.Instance.CurrentPlayer.tag);
        var alreadyActive = _status == FireplaceStatus.Activated;
        if (!isPlayer && alreadyActive) return;
        
        if (_backToZeroRoutine != null)
        {
            StopCoroutine(_backToZeroRoutine);
        }

        if (_timer != null)
        {
            _timer.StartFromToTimer(_currentActivationTime, totalActivationTime, TimerType.Increasing);
            ShowProgressBar(_currentActivationTime);
        }
    }

    private void OnAnyTimerValueChanged(float currentTimerValue)
    {
        _currentActivationTime = currentTimerValue;
        FillProgressBar(_currentActivationTime / totalActivationTime);
    }

    private void OnActivationIsComplete()
    {
        _status = FireplaceStatus.Activated;
        HideProgressBar();
    }

    private void OnTriggerExit(Collider other)
    {
        var isPlayer = other.CompareTag(PlayerManager.Instance.CurrentPlayer.tag);
        // var alreadyActive = _status == FireplaceStatus.Activated;
        if (!isPlayer) return; //&& alreadyActive) return;

        if (_timer != null)
        {
            _timer.StopTimer();

            _backToZeroRoutine = DecreaseProgressBarRoutine();
            StartCoroutine(_backToZeroRoutine);
            print($"Current status : {_status} ^^ _backToZeroRoutine is active : {_backToZeroRoutine != null}");
        }
    }

    private IEnumerator DecreaseProgressBarRoutine()
    {
        while (_currentActivationTime > 0.0f)
        {
            _currentActivationTime -= Time.deltaTime;
            OnAnyTimerValueChanged(_currentActivationTime);
            yield return null;
        }

        _currentActivationTime = 0.0f;
        HideProgressBar();
    }

    private void OnDisable()
    {
        if (_timer != null)
        {
            _timer.OnAnyValueChanged -= OnAnyTimerValueChanged;
            _timer.TimerIsOver -= OnActivationIsComplete;
        }
    }
}