using UnityEngine;
using DG.Tweening;

public class MoveTween : BaseTween
{
    [SerializeField] private Transform _targetTransform;
    
    [SerializeField] private Vector3 _targetVector;

    public bool _vectorMove;
    public void Move()
    {
        if (_targetTransform == null && _vectorMove == false) return;

        KillCurrent();
        onPlay?.Invoke();

        if(_vectorMove == false)
        {
            _currentTween = transform.DOMove(
                _targetTransform.position, 
                _duration)
            .SetEase(_ease)
            .OnComplete(() => onComplete?.Invoke());
        }
        else
        {
            _currentTween = transform.DOMove(
                _targetVector,
                _duration)
            .SetEase(_ease)
            .OnComplete(() => onComplete?.Invoke());
        }
    }
    public void LocalMove()
    {
        if (_targetTransform == null && _vectorMove == false) return;

        KillCurrent();
        onPlay?.Invoke();

        if (_vectorMove == false)
        {
            _currentTween = transform.DOLocalMove(
                _targetTransform.position,
                _duration)
            .SetEase(_ease)
            .OnComplete(() => onComplete?.Invoke());
        }
        else
        {
            _currentTween = transform.DOLocalMove(
                _targetVector,
                _duration)
            .SetEase(_ease)
            .OnComplete(() => onComplete?.Invoke());
        }
    }
    public void SetTargetTransform(Transform targetTransform) => _targetTransform = targetTransform;    
    public void SetTargetVector(Vector3 targetVector) => _targetVector = targetVector;
}