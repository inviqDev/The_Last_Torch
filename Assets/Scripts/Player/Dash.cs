using System;
using System.Collections;
using UnityEngine;

public class Dash : MonoBehaviour
{
    public event Action OnDashFinished;
    
    [SerializeField] private CharacterController controller;
    
    private float _dashSpeed;
    private float _dashSpeedModifier = 1.0f;
    
    private float _dashDuration;
    private float _dashDurationModifier = 1.0f;
    
    private IEnumerator _dashCoroutine;
    private bool _isDashing;

    private void Start()
    {
        _isDashing = false;
    }

    public void SetDashSettingsFromConfig(PlayerConfig config)
    {
        _dashSpeed = config.DashSpeed * _dashSpeedModifier;
        _dashDuration = config.DashDuration * _dashDurationModifier;
    }

    public void IncreaseDashSpeedModifier()
    {
        print($"dash speed modifier BEFORE : {_dashSpeedModifier}");
        print($"dash speed BEFORE: {_dashSpeed}");
        
        _dashSpeedModifier += 0.2f;
        _dashSpeed *= _dashSpeedModifier;
        
        print($"dash speed modifier AFTER : {_dashSpeedModifier}");
        print($"dash speed AFTER: {_dashSpeed}");
    }

    public void IncreaseDashDurationModifier()
    {
        print($"dash duration modifier BEFORE : {_dashDurationModifier}");
        print($"dash duration BEFORE: {_dashDuration}");
        
        _dashDurationModifier += 0.2f;
        _dashDuration *= _dashDurationModifier;
        
        print($"dash duration modifier AFTER : {_dashDurationModifier}");
        print($"dash duration AFTER: {_dashDuration}");
    }

    public void PerformDash(Vector3 direction)
    {
        if (_isDashing) return;
        if (_dashCoroutine != null)
        {
            StopCoroutine(_dashCoroutine);
        }
        
        var dashDirection = direction.normalized * _dashSpeed;
        _dashCoroutine = DashCoroutine(dashDirection);
        StartCoroutine(_dashCoroutine);
    }

    private IEnumerator DashCoroutine(Vector3 direction)
    {
        _isDashing = true;
        var time = 0.0f;
        
        while (time < _dashDuration)
        {
            time += Time.deltaTime;
            controller.Move(direction * Time.deltaTime);
            yield return null;
        }
        
        _isDashing = false;
        OnDashFinished?.Invoke();
    }
}