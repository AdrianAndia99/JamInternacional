using UnityEngine;
using UnityEngine.Events;

public abstract class MinigameManager : MonoBehaviour
{
    public UnityEvent OnStart;
    public UnityEvent OnWin;
    public UnityEvent OnDefeat;

    private void Start()
    {
        OnStart?.Invoke();
    }
    public void Win()
    {
        OnWin?.Invoke();
    }
    public void Defeat()
    {
        OnDefeat?.Invoke();
    }
}