using UnityEngine;
using DG.Tweening;

public class JumpTween : BaseTween
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private float _jumpPower;
    [SerializeField] private int _numJumps;

    public void Jump()
    {
        if (_targetTransform == null) return;

        KillCurrent();
        onPlay?.Invoke();

        _currentTween = transform.DOJump(
            _targetTransform.position,
            _jumpPower,
            _numJumps,
            _duration
        )
        .SetEase(_ease)
        .OnComplete(() => onComplete?.Invoke());
    }
    public void SetTargetTransform(Transform targetTransform) => _targetTransform = targetTransform;
    public void SetJumpPower(float jumpPower) => _jumpPower = jumpPower;
    public void SetNumJumps(int numJumps) => _numJumps = numJumps;
}