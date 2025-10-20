using System.Collections.Generic;
using UnityEngine;

public class CinemachineCameraSwitcher : MonoBehaviour
{
    [SerializeField] private List<Transform> _cameras;
    [SerializeField] private int _currentIndex;

    [SerializeField] private MoveTween _moveTween;
    [SerializeField] private RotateTween _rotateTween;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            NextCamera();
        }
    }
    public void NextCamera()
    {
        _currentIndex++;
        if (_currentIndex >= _cameras.Count)
        {

        }

        _moveTween.SetTargetTransform(_cameras[_currentIndex]);
        _rotateTween.SetTargetTransform(_cameras[_currentIndex]);

        _moveTween.Move();
        _rotateTween.Rotate();
    }
}