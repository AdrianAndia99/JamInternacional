using UnityEngine;
using UnityEngine.Events;

public class AnimationEventRelay : MonoBehaviour
{
    public UnityEvent OnEventTriggered;

    public void TriggerEvent()
    {
        OnEventTriggered?.Invoke();
    }
}