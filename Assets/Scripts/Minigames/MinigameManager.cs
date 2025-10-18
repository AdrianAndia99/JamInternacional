using UnityEngine;
using UnityEngine.Events;

public abstract class MinigameManager : MonoBehaviour
{
    public GameObject MiniGamePrefab;

    public UnityEvent OnStart;
    public UnityEvent OnWin;
    public UnityEvent OnDefeat;

    private void Start()
    {
        OnStart?.Invoke();
    }
}