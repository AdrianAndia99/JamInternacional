using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }

    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(this.gameObject); // Changed from 'this' to 'this.gameObject'
        }
        else
        {
            Destroy(gameObject);
        }
    }
}