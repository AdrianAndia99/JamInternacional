using UnityEngine;
using UnityEngine.Events;

public class GhostController : MonoBehaviour
{
    public UnityEvent OnGhostClicked;

    private float showTime = 1.5f;
    private float hideTime = 1f;
    private float timer;
    private bool isVisible = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        HideGhost();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            if (isVisible)
                HideGhost();
            else
                ShowGhost();
        }
    }

    void ShowGhost()
    {
        transform.position = new Vector3(
            Random.Range(-3f, 3f),
            Random.Range(-2f, 2f),
            0
        );
        spriteRenderer.enabled = true;
        isVisible = true;
        timer = showTime;
    }

    void HideGhost()
    {
        spriteRenderer.enabled = false;
        isVisible = false;
        timer = hideTime;
    }

    void OnMouseDown()
    {
        if (isVisible)
        {
            OnGhostClicked?.Invoke();
            HideGhost();
        }
    }
}
