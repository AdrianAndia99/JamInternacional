using UnityEngine;
using UnityEngine.Events;

public abstract class MinigameManager : MonoBehaviour
{
    public GameObject MiniGamePrefab;

    public UnityEvent OnWin;
    public UnityEvent OnDefeat;
}