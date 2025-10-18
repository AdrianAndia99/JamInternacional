using UnityEngine;
using DG.Tweening;
using System;

public class RotateTween : BaseTween
{
    [Header("Target Settings")]
    [SerializeField] private bool _useVectorRotation = false;
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Vector3 _targetRotation;
    [SerializeField] private RotateMode _rotateMode = RotateMode.FastBeyond360;

    public void Rotate()
    {
        KillCurrent();
        onPlay?.Invoke();

        Vector3 targetEuler = _useVectorRotation
            ? _targetRotation
            : _targetTransform != null
                ? _targetTransform.rotation.eulerAngles
                : transform.rotation.eulerAngles; // fallback por seguridad

        _currentTween = transform
            .DORotate(targetEuler, _duration, _rotateMode)
            .SetEase(_ease)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void SetTargetTransform(Transform targetTransform) => _targetTransform = targetTransform;
    public void SetTargetRotation(Vector3 targetRotation) => _targetRotation = targetRotation;
    public void SetUseVectorRotation(bool useVector) => _useVectorRotation = useVector;

    public void SetRotateMode(int rotateMode)
    {
        int max = Enum.GetValues(typeof(RotateMode)).Length - 1;
        rotateMode = Mathf.Clamp(rotateMode, 0, max);
        _rotateMode = (RotateMode)rotateMode;
    }
}