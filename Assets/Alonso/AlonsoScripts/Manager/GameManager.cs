using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityEvent OnStartScene;
    public UnityEvent OnFinishScene;

    public void DebugSMT(string smt)
    {
        Debug.Log(smt);
    }
}