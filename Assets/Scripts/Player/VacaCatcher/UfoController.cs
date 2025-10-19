using UnityEngine;
using UnityEngine.InputSystem;

public class UfoController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    [Header("Rayo")]
    [SerializeField] private GameObject rayPrefab;
    [SerializeField] private Transform rayOrigin;

    [Header("Animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector2 moveInput;
    private bool canShoot = true;
    private bool isDead = false;

    private void Update()
    {
        if (isDead) return; // no puede moverse si está muerto

        // Movimiento horizontal
        Vector3 move = new Vector3(moveInput.x, 0f, 0f);
        transform.Translate(move * moveSpeed * Time.deltaTime);

        // Limitar movimiento dentro de (-8, 8)
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;

        // Actualizar animaciones
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        // Estado de movimiento
        bool isMoving = Mathf.Abs(moveInput.x) > 0.1f;
        animator.SetBool("isMoving", isMoving);

        // Flip sprite según dirección
        if (isMoving)
            spriteRenderer.flipX = moveInput.x > 0;
    }

    // --- CALLBACKS DEL NUEVO INPUT SYSTEM ---

    // Movimiento (Vector2)
    public void OnMove(InputAction.CallbackContext context)
    {
        if (isDead) return; // no puede moverse si está muerto

        // Leemos el vector completo (x, y)
        moveInput = context.ReadValue<Vector2>();

        // Si la acción termina, reseteamos el movimiento
        if (context.canceled)
            moveInput = Vector2.zero;
    }

    // Disparo
    public void OnFire(InputAction.CallbackContext context)
    {
        // Solo dispara cuando la acción se ejecuta (performed)
        if (context.performed && canShoot)
        {
            ShootRay();
        }
    }

    private void ShootRay()
    {
        canShoot = false;

        GameObject ray = Instantiate(rayPrefab, rayOrigin.position, Quaternion.identity);
        RayController rc = ray.GetComponent<RayController>();
        rc.onFinish += ResetShot;
        rc.onMiss += OnMissedShot; // Nuevo: evento si falla
    }

    private void ResetShot()
    {
        // Permitir disparar nuevamente si no ha muerto
        if (!isDead)
            canShoot = true;
    }
    private void OnMissedShot()
    {
        canShoot = false;
        isDead = true;
        moveInput = Vector2.zero;

        animator.SetTrigger("dead");
        Debug.Log("El OVNI falló el disparo, perdiste.");
    }
}
