using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CinemachineCameraSwitcher : MonoBehaviour
{
    [SerializeField] private List<CinemachineCamera> _cameras;
    [SerializeField] private int _activeCameraIndex = 0;
    [SerializeField] private int _activePriority = 20;
    [SerializeField] private int _inactivePriority = 10;

    private void Start()
    {
        UpdateCameraPriorities();
    }

    public void SwitchToCamera(int index)
    {
        if (index < 0 || index >= _cameras.Count)
        {
            Debug.LogWarning($"CinemachineCameraSwitcher: índice inválido {index}");
            return;
        }

        if (_activeCameraIndex == index)
            return;

        _activeCameraIndex = index;
        UpdateCameraPriorities();
    }

    public void NextCamera()
    {
        int nextIndex = (_activeCameraIndex + 1) % _cameras.Count;
        SwitchToCamera(nextIndex);
    }

    public void PreviousCamera()
    {
        int prevIndex = (_activeCameraIndex - 1 + _cameras.Count) % _cameras.Count;
        SwitchToCamera(prevIndex);
    }

    private void UpdateCameraPriorities()
    {
        for (int i = 0; i < _cameras.Count; i++)
        {
            if (_cameras[i] != null)
                _cameras[i].Priority = (i == _activeCameraIndex) ? _activePriority : _inactivePriority;
        }
    }

    public CinemachineCamera GetCurrentCamera()
    {
        if (_activeCameraIndex >= 0 && _activeCameraIndex < _cameras.Count)
            return _cameras[_activeCameraIndex];
        return null;
    }
}