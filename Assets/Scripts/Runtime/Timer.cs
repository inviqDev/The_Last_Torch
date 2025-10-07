using System;
using System.Collections;
using UnityEngine;

namespace Runtime
{
    public enum TimerType
    {
        Increasing,
        Decreasing
    }
    public class Timer : IDisposable
    {
        public event Action<float> OnAnyValueChanged;
        public event Action<int> OnTicked;
        public event Action TimerIsOver;

        private readonly MonoBehaviour _timerHandler;
        private Coroutine _timerCoroutine;
        private bool _disposed;

        private float _totalTime;
        private float _currentTime;

        public Timer(MonoBehaviour timerHandler) => _timerHandler = timerHandler;
        public bool IsActive => _timerCoroutine != null;
        
        public void StartFromToTimer(float from, float to, TimerType timerType, bool showProgress = true)
        {
            ThrowIfDisposed();
            StopTimer();
            
            var routine = timerType switch
            {
                TimerType.Increasing =>  IncreasingTimerRoutine(from, to, showProgress),
                TimerType.Decreasing => DecreasingTimerRoutine(from, to, showProgress),
                _ => null
            };

            UnityEngine.Assertions.Assert.IsNotNull(routine, "Timer has not been started");
            _timerCoroutine = _timerHandler.StartCoroutine(routine);
        }
        
        private IEnumerator IncreasingTimerRoutine(float from, float to, bool showProgress)
        {
            var timerValue = from;
            while (timerValue <= to)
            {
                timerValue += Time.deltaTime;
                if (showProgress)
                {
                    OnAnyValueChanged?.Invoke(timerValue);
                }
                
                yield return null;
            }
        
            TimerIsOver?.Invoke();
            _timerCoroutine = null;
        }
        
        private IEnumerator DecreasingTimerRoutine(float from, float to, bool show)
        {
            var timerValue = from;
            while (timerValue >= to)
            {
                timerValue -= Time.deltaTime;
                if (show)
                {
                    OnAnyValueChanged?.Invoke(timerValue);
                }
                
                yield return null;
            }
        
            TimerIsOver?.Invoke();
            _timerCoroutine = null;
        }

        public void StartTimerTicker(float intervalInSeconds, int repeatsAmount)
        {
            ThrowIfDisposed();
            StopTimer();
            
            var routine = TimerTickerRoutine(intervalInSeconds, repeatsAmount);
            _timerCoroutine = _timerHandler.StartCoroutine(routine);
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
            _timerCoroutine = null;
        }
    
        public void StopTimer()
        {
            if (!IsActive || !_timerHandler) return;
            _timerHandler.StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
        
        private void ThrowIfDisposed()
        {
            if (_disposed) 
                throw new ObjectDisposedException(nameof(Timer));
        }

        public void Dispose()
        {
            if (_disposed) return;
            
            StopTimer();

            OnAnyValueChanged = null;
            OnTicked = null;
            TimerIsOver = null;
            
            _disposed = true;
        }
    }
}