using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private List<Transform> _targetTransforms;
    [SerializeField] private Transform _lastTarget;
    [SerializeField] private int _currentIndex = 0;

    [SerializeField] private MoveTween _moveTween;
    [SerializeField] private RotateTween _rotateTween;

    private void Start()
    {
        NextCamera();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            NextCamera();
        }
    }
    public void NextCamera()
    {
        if(_lastTarget != null)
        {
            transform.position = _lastTarget.position;
        }
        _currentIndex++;
        if (_currentIndex >= _targetTransforms.Count)
        {
            _currentIndex = 0;
        }

        _moveTween.SetTargetTransform(_targetTransforms[_currentIndex]);
        _rotateTween.SetTargetTransform(_targetTransforms[_currentIndex]);

        _moveTween.Move();
        _rotateTween.Rotate();
        _lastTarget = _targetTransforms[_currentIndex];
    }
}