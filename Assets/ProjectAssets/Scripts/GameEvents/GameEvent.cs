using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEvents
{
    [CreateAssetMenu(fileName = "VoidEvent", menuName = "GameEvents/VoidEvent", order = 0)]
    public class GameEvent : ScriptableObject
    {
        private List<GameEventListener> listeners;

        private void OnEnable()
        {
            listeners = new List<GameEventListener>();
        }

        private void OnDisable()
        {
            listeners.Clear();
        }

        public void SetUp()
        {
            listeners = new List<GameEventListener>();
        }

        public void Raise()
        {
            foreach (GameEventListener sub in listeners)
            {
                sub.OnEventRaised();
            }
        }

        public void Register(GameEventListener newListener)
        {
            if (listeners.Contains(newListener)) return;

            listeners.Add(newListener);
        }

        public void Unregister(GameEventListener newListener)
        {
            if (!listeners.Contains(newListener)) return;

            listeners.Remove(newListener);
        }
    }
    public abstract class GameEvent<T> : ScriptableObject
    {
        private List<GameEventListener<T>> listeners; 

        private void OnEnable()
        {
            listeners = new List<GameEventListener<T>>();
        }

        private void OnDisable()
        {
            listeners.Clear();
        }

        public void SetUp()
        {
            listeners = new List<GameEventListener<T>>();
        }

        public void Raise(T value)
        {
            foreach (GameEventListener<T> sub in listeners)
            {
                sub.OnEventRaised(value);
            }
        }

        public void Register(GameEventListener<T> newListener)
        {
            if (listeners.Contains(newListener)) return;

            listeners.Add(newListener);
        }

        public void Unregister(GameEventListener<T> newListener)
        {
            if (!listeners.Contains(newListener)) return;

            listeners.Remove(newListener);
        }
    }
}