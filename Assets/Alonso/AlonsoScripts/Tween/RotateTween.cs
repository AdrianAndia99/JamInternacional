using UnityEngine;
using DG.Tweening;
using System;

public class RotateTween : BaseTween
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private RotateMode _rotateMode;

    public void Rotate()
    {
        KillCurrent();
        onPlay?.Invoke();

        _currentTween = transform.DORotate(
            _targetTransform.rotation.eulerAngles,
            _duration,
            _rotateMode
        )
        .SetEase(_ease)
        .OnComplete(() => onComplete?.Invoke());
    }
    public void SetTargetTransform(Transform targetTransform) => _targetTransform = targetTransform;
    public void SetRotateMode(int rotateMode)
    {
        int max = Enum.GetValues(typeof(RotateMode)).Length - 1;

        rotateMode = Mathf.Clamp(rotateMode, 0, max);

        _rotateMode = (RotateMode)rotateMode;
    }
}