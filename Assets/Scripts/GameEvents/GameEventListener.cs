using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.GameEvents
{
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] private GameEvent gameEvent;

        [SerializeField] private UnityEvent response;

        private void OnEnable()
        {
            gameEvent.Register(this);
        }

        private void OnDisable()
        {
            gameEvent.Unregister(this);
        }

        public void OnEventRaised()
        {
            response?.Invoke();
        }
    }
    public abstract class GameEventListener<T> : MonoBehaviour
    {
        [SerializeField] private GameEvent<T> gameEvent;

        [SerializeField] private UnityEvent<T> response;

        private void OnEnable()
        {
            gameEvent.Register(this);
        }

        private void OnDisable()
        {
            gameEvent.Unregister(this);
        }

        public void OnEventRaised(T value)
        {
            response?.Invoke(value);
        }
    }
}