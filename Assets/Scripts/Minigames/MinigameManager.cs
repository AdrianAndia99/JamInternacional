using UnityEngine;
using UnityEngine.Events;

public abstract class MinigameManager : MonoBehaviour
{
    public GameObject MiniGamePrefab;

    [Header("Events")]
    public UnityEvent OnStart;

    public UnityEvent OnFinish;

    public virtual void OnStartGame()
    {
        OnStart?.Invoke();
    }
    public virtual void OnFinishGame()
    {
        OnFinish?.Invoke();
    }
}