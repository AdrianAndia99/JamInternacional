using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class CollisionNotifier2D : MonoBehaviour
{
    // Enable Collision Events
    [SerializeField] private bool collisionEnter = false;
    [SerializeField] private bool collisionExit = false;
    [SerializeField] private bool collisionStay = false;

    // Enable Trigger Events
    [SerializeField] private bool triggerEnter = false;
    [SerializeField] private bool triggerExit = false;
    [SerializeField] private bool triggerStay = false;

    // Collision Events With Parameters
    [SerializeField] private bool parameterCollisionEnterEvents = false;
    [SerializeField] private bool parameterCollisionExitEvents = false;
    [SerializeField] private bool parameterCollisionStayEvents = false;

    // Trigger Events With Parameters
    [SerializeField] private bool parameterTriggerEnterEvents = false;
    [SerializeField] private bool parameterTriggerExitEvents = false;
    [SerializeField] private bool parameterTriggerStayEvents = false;

    // Collision Events Without Parameters
    [SerializeField] private bool voidCollisionEnterEvents = false;
    [SerializeField] private bool voidCollisionExitEvents = false;
    [SerializeField] private bool voidCollisionStayEvents = false;

    // Trigger Events Without Parameters
    [SerializeField] private bool voidTriggerEnterEvents = false;
    [SerializeField] private bool voidTriggerExitEvents = false;
    [SerializeField] private bool voidTriggerStayEvents = false;

    // Layer Filtering
    [SerializeField] private bool enableLayerFilter = false;
    [SerializeField] private LayerMask allowedLayers;

    // Tag Filtering
    [SerializeField] private bool enableTagFilter = false;
    [SerializeField] private List<string> allowedTags = new List<string>();

    // Collision Events (with parameters)
    [SerializeField] private UnityEvent<Collision2D> OnCollisionEntered;
    [SerializeField] private UnityEvent<Collision2D> OnCollisionExited;
    [SerializeField] private UnityEvent<Collision2D> OnCollisionStayed;

    // Collision Events (void)
    [SerializeField] private UnityEvent OnCollisionEnteredSimple;
    [SerializeField] private UnityEvent OnCollisionExitedSimple;
    [SerializeField] private UnityEvent OnCollisionStayedSimple;

    // Trigger Events (with parameters)
    [SerializeField] private UnityEvent<Collider2D> OnTriggerEntered;
    [SerializeField] private UnityEvent<Collider2D> OnTriggerExited;
    [SerializeField] private UnityEvent<Collider2D> OnTriggerStayed;

    // Trigger Events (void)
    [SerializeField] private UnityEvent OnTriggerEnteredSimple;
    [SerializeField] private UnityEvent OnTriggerExitedSimple;
    [SerializeField] private UnityEvent OnTriggerStayedSimple;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collisionEnter || !IsAllowed(collision.gameObject)) return;

        if (parameterCollisionEnterEvents)
            OnCollisionEntered?.Invoke(collision);

        if (voidCollisionEnterEvents)
            OnCollisionEnteredSimple?.Invoke();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collisionExit || !IsAllowed(collision.gameObject)) return;

        if (parameterCollisionExitEvents)
            OnCollisionExited?.Invoke(collision);

        if (voidCollisionExitEvents)
            OnCollisionExitedSimple?.Invoke();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collisionStay || !IsAllowed(collision.gameObject)) return;

        if (parameterCollisionStayEvents)
            OnCollisionStayed?.Invoke(collision);

        if (voidCollisionStayEvents)
            OnCollisionStayedSimple?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggerEnter || !IsAllowed(other.gameObject)) return;

        if (parameterTriggerEnterEvents)
            OnTriggerEntered?.Invoke(other);

        if (voidTriggerEnterEvents)
            OnTriggerEnteredSimple?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!triggerExit || !IsAllowed(other.gameObject)) return;

        if (parameterTriggerExitEvents)
            OnTriggerExited?.Invoke(other);

        if (voidTriggerExitEvents)
            OnTriggerExitedSimple?.Invoke();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!triggerStay || !IsAllowed(other.gameObject)) return;

        if (parameterTriggerStayEvents)
            OnTriggerStayed?.Invoke(other);

        if (voidTriggerStayEvents)
            OnTriggerStayedSimple?.Invoke();
    }

    private bool IsAllowed(GameObject obj)
    {
        if (enableLayerFilter && (allowedLayers.value & (1 << obj.layer)) == 0)
            return false;

        if (enableTagFilter && !allowedTags.Contains(obj.tag))
            return false;

        return true;
    }
}