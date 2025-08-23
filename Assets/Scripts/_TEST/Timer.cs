using System;
using System.Collections;
using UnityEngine;

public enum TimerType
{
    Increasing,
    Decreasing
}
public class Timer
{
    private IEnumerator _timerRoutine;
    private readonly MonoBehaviour _timerActivator;

    private float _totalTime;
    private float _currentTime;

    public event Action<float> OnValueChanged;
    public event Action TimerIsOver;

    public Timer(MonoBehaviour timerActivator) => _timerActivator = timerActivator;

    public void StartFromToTimer(float from, float to, TimerType timerType)
    {
        _timerRoutine = timerType switch
        {
            TimerType.Increasing => IncreasingTimerRoutine(from, to),
            TimerType.Decreasing => DecreasingTimerRoutine(from, to),
            _ => null
        };

        Debug.Assert(_timerRoutine != null, "Timer has not been started");
        _timerActivator.StartCoroutine(_timerRoutine);
    }

    private IEnumerator IncreasingTimerRoutine(float from, float to)
    {
        while (from <= to)
        {
            from += Time.deltaTime;
            OnValueChanged?.Invoke(from);
            yield return null;
        }
        
        TimerIsOver?.Invoke();
    }

    private IEnumerator DecreasingTimerRoutine(float from, float to)
    {
        while (from >= to)
        {
            from -= Time.deltaTime;
            OnValueChanged?.Invoke(from);
            yield return null;
        }
        
        TimerIsOver?.Invoke();
    }
    
    public void StopTimer()
    {
        if (_timerRoutine == null) return;
        _timerActivator.StopCoroutine(_timerRoutine);
    }
}