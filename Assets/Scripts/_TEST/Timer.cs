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

    public event Action<float> OnAnyValueChanged;
    public event Action<int> OnTicked;
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
            OnAnyValueChanged?.Invoke(from);
            yield return null;
        }
        
        TimerIsOver?.Invoke();
    }

    private IEnumerator DecreasingTimerRoutine(float from, float to)
    {
        while (from >= to)
        {
            from -= Time.deltaTime;
            OnAnyValueChanged?.Invoke(from);
            yield return null;
        }
        
        TimerIsOver?.Invoke();
    }

    public void StartTimerTicker(float intervalInSeconds, int repeatsAmount)
    {
        StopTimer();
        _timerRoutine = TimerTickerRoutine(intervalInSeconds, repeatsAmount);
        _timerActivator.StartCoroutine(_timerRoutine);
    }
    
    private IEnumerator TimerTickerRoutine(float interval, int repeats)
    {
        var wait = new WaitForSeconds(interval);
        var counter = repeats;

        while (counter > 0)
        {
            yield return wait;
            OnTicked?.Invoke(counter);
            counter--;
        }
        
        TimerIsOver?.Invoke();
    }
    
    public void StopTimer()
    {
        if (_timerRoutine == null) return;
        _timerActivator.StopCoroutine(_timerRoutine);
    }
}