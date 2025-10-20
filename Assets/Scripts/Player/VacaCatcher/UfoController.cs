using UnityEngine;
using UnityEngine.InputSystem;

public class UfoController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;

    [Header("Rayo")]
    [SerializeField] private GameObject rayPrefab;
    [SerializeField] private Transform rayOrigin;

    [Header("Animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector2 moveInput;
    private bool canShoot = true;
    private bool isDead = false;
    private bool isAbducting = false; // cuando abduce a la vaca
    private bool gameEnded = false;

    private RayController currentRay;

    private void OnEnable()
    {
        // Reset de estado por si se reusa el prefab
        moveInput = Vector2.zero;
        canShoot = true;
        isDead = false;
        isAbducting = false;
        gameEnded = false;

        animator.Rebind();
        animator.Update(0f);
    }

    private void OnDisable()
    {
        // Limpieza: si hay rayo activo, desvincular eventos
        if (currentRay != null)
        {
            currentRay.onFinish -= OnRayFinished;
            currentRay.onMiss -= OnMissedShot;
            currentRay.onAbductionSuccess -= OnAbductionSuccess;
        }
    }

    private void Update()
    {
        if (isDead || isAbducting) return;

        Vector3 move = new Vector3(moveInput.x, 0f, 0f);
        transform.Translate(move * moveSpeed * Time.deltaTime);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        bool isMoving = Mathf.Abs(moveInput.x) > 0.1f;
        animator.SetBool("isMoving", isMoving);

        if (isMoving)
            spriteRenderer.flipX = moveInput.x < 0;
    }

    // --- INPUT SYSTEM CALLBACKS ---

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isDead || isAbducting) return;

        moveInput = context.ReadValue<Vector2>();
        if (context.canceled)
            moveInput = Vector2.zero;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed && canShoot && !isDead && !isAbducting)
        {
            ShootRay();
        }
    }

    private void ShootRay()
    {
        canShoot = false;
        animator.SetTrigger("shoot");

        GameObject ray = Instantiate(rayPrefab, rayOrigin.position, Quaternion.identity);
        currentRay = ray.GetComponent<RayController>();

        // Subscribir eventos
        currentRay.onFinish += OnRayFinished;
        currentRay.onMiss += OnMissedShot;
        currentRay.onAbductionSuccess += OnAbductionSuccess;
    }

    private void OnAbductionSuccess()
    {
        if (gameEnded) return;

        gameEnded = true; // bloquea todo
        // El alien deja de moverse mientras abduce
        isAbducting = true;
        moveInput = Vector2.zero;
        animator.SetBool("isMoving", false);
        Debug.Log("Alien abduciendo vaca...");
    }

    private void OnRayFinished()
    {
        if (gameEnded) return;

        // Solo vuelve a poder disparar si el juego no terminó
        if (!isDead)
            canShoot = true;
    }

    private void OnMissedShot()
    {
        isDead = true;
        canShoot = false;
        moveInput = Vector2.zero;
        animator.SetTrigger("dead");
        Debug.Log("OVNI falló el tiro. GAME OVER.");
    }
}
