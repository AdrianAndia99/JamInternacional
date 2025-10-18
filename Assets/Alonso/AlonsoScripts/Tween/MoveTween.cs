using UnityEngine;
using DG.Tweening;

public class MoveTween : BaseTween
{
    [SerializeField] private Transform _targetTransform;

    public void Move()
    {
        if (_targetTransform == null) return;

        KillCurrent();
        onPlay?.Invoke();

        _currentTween = transform.DOMove(
            _targetTransform.position, 
            _duration)
        .SetEase(_ease)
        .OnComplete(() => onComplete?.Invoke());
    }
    public void SetTargetTransform(Transform targetTransform) => _targetTransform = targetTransform;
}