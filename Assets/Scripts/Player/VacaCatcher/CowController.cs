using UnityEngine;

public class CowController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float changeDirTime;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    [Header("Animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float timer;
    private float direction;
    private bool isCaptured = false;

    void Start()
    {
        ChangeDirection();
    }

    void Update()
    {
        if (isCaptured) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
            ChangeDirection();

        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);

        // Limitar a los bordes
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;

        // Cambiar animación y flip
        UpdateAnimation();
    }

    private void ChangeDirection()
    {
        direction = Random.Range(-1f, 1f);
        timer = changeDirTime;
    }
    private void UpdateAnimation()
    {
        bool isMoving = Mathf.Abs(direction) > 0.1f;
        animator.SetBool("isRunning", isMoving);

        if (isMoving)
            spriteRenderer.flipX = direction > 0;
    }

    public void OnCaptured()
    {
        isCaptured = true;
        animator.SetTrigger("captured");
    }
}
