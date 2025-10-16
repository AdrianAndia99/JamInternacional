using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System;

public abstract class BaseTween : MonoBehaviour
{
    [Header("Base Parameters")]
    [SerializeField] protected float _duration;
    [SerializeField] protected Ease _ease;

    [Header("Events")]
    public UnityEvent onPlay;
    public UnityEvent onComplete;

    protected Tween _currentTween;
    protected void KillCurrent()
    {
        if (_currentTween != null && _currentTween.IsActive())
        {
            _currentTween.Kill();
            _currentTween = null;
        }
    }
    public void SetDuration(float duration) => _duration = duration;
    public void SetEaseFromInt(int easeValue)
    {
        int max = Enum.GetValues(typeof(Ease)).Length - 1;

        easeValue = Mathf.Clamp(easeValue, 0, max);

        _ease = (Ease)easeValue;
    }
}