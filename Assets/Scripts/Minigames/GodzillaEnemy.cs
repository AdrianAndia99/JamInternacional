using UnityEngine;
using DG.Tweening;
public class GodzillaEnemy : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Tooltip("Punto A de movimiento")]
    [SerializeField] private Transform pointA;
    
    [Tooltip("Punto B de movimiento")]
    [SerializeField] private Transform pointB;
    
    [Tooltip("Velocidad de movimiento")]
    [SerializeField] private float moveSpeed = 2f;
    
    [Tooltip("Tipo de movimiento")]
    [SerializeField] private MovementType movementType = MovementType.PingPong;
    
    [Tooltip("Duración del efecto de destrucción")]
    [SerializeField] private float destructionDuration = 1f;

    // Estado
    private bool isMoving = true;
    private bool isDestroyed = false;
    private Tween movementTween;
    private GodzillaGameManager gameManager;

    public enum MovementType
    {
        PingPong,    // Va y viene
        Loop         // Ciclo continuo A→B→A→B...
    }

    private void Start()
    {
        // Buscar el GameManager
        gameManager = FindFirstObjectByType<GodzillaGameManager>();

        if (gameManager != null)
        {
            gameManager.RegisterEnemy(this);
        }

        // Iniciar movimiento
        if (pointA != null && pointB != null)
        {
            StartMovement();
        }
        else
        {
            Debug.LogError("¡Asigna los puntos A y B en el Inspector!");
        }
    }

    /// <summary>
    /// Inicia el movimiento entre los dos puntos
    /// </summary>
    private void StartMovement()
    {
        // Posicionar en el punto A al inicio
        transform.position = pointA.position;

        float distance = Vector3.Distance(pointA.position, pointB.position);
        float duration = distance / moveSpeed;

        if (movementType == MovementType.PingPong)
        {
            // Movimiento de ida y vuelta
            movementTween = transform.DOMove(pointB.position, duration)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            // Movimiento en ciclo continuo
            movementTween = transform.DOMove(pointB.position, duration)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .OnStepComplete(() => transform.position = pointA.position);
        }
    }

    /// <summary>
    /// Detiene el movimiento del enemigo
    /// </summary>
    public void StopMovement()
    {
        isMoving = false;
        if (movementTween != null)
        {
            movementTween.Kill();
        }
    }

    /// <summary>
    /// Destruye el enemigo
    /// </summary>
    public void DestroyEnemy()
    {
        if (isDestroyed) return;

        isDestroyed = true;
        StopMovement();

        Debug.Log($"Enemigo {gameObject.name} destruido por el láser!");

        // Notificar al GameManager
        if (gameManager != null)
        {
            gameManager.OnEnemyDestroyed(this);
        }

        // Animar destrucción y destruir el objeto
        transform.DOScale(Vector3.zero, destructionDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }
    private void OnTriggerEnter(Collider other)
    {
        // Si colisiona con algo que tenga el tag "Laser" o layer "Laser"
        if (other.CompareTag("Laser") || other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            DestroyEnemy();
        }
    }

    /// <summary>
    /// Verifica si el enemigo está dentro del rayo usando Raycast
    /// Llamado desde GodzillaController
    /// </summary>
    public bool IsHitByLaser(Vector3 laserStart, Vector3 laserDirection, float laserDistance)
    {
        if (isDestroyed) return false;

        // Raycast desde el origen del láser
        RaycastHit[] hits = Physics.RaycastAll(laserStart, laserDirection, laserDistance);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject == gameObject)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsDestroyed => isDestroyed;

    private void OnDestroy()
    {
        if (movementTween != null)
        {
            movementTween.Kill();
        }
    }
}