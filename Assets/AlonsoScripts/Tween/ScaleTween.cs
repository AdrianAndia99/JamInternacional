using UnityEngine;
using DG.Tweening;

public class ScaleTween : BaseTween
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private bool _relative = false;

    public void Scale()
    {
        KillCurrent();
        onPlay?.Invoke();

        if (_relative)
        {
            _currentTween = transform.DOScale(transform.localScale + _targetTransform.localScale, _duration);
        }
        else
        {
            _currentTween = transform.DOScale(_targetTransform.localScale, _duration);
        }

        _currentTween
            .SetEase(_ease)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void SetTargetTransform(Transform targetTransform) => _targetTransform = targetTransform;
    public void SetRelative(bool relative) => _relative = relative;
}