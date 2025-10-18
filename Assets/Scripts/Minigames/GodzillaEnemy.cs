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

        // Rotar hacia el punto B al inicio
        Vector3 directionToB = (pointB.position - pointA.position).normalized;
        if (directionToB != Vector3.zero)
        {
            Quaternion rotationToB = Quaternion.LookRotation(directionToB);
            transform.rotation = rotationToB;
        }

        float distance = Vector3.Distance(pointA.position, pointB.position);
        float duration = distance / moveSpeed;

        if (movementType == MovementType.PingPong)
        {
            // Movimiento de ida y vuelta con rotación
            Sequence sequence = DOTween.Sequence();
            
            // Ir de A a B
            sequence.Append(transform.DOMove(pointB.position, duration).SetEase(Ease.Linear));
            
            // Rotar hacia A cuando llegue a B
            sequence.AppendCallback(() => {
                Vector3 directionToA = (pointA.position - pointB.position).normalized;
                if (directionToA != Vector3.zero)
                {
                    transform.DORotateQuaternion(Quaternion.LookRotation(directionToA), 0.3f);
                }
            });
            
            // Ir de B a A
            sequence.Append(transform.DOMove(pointA.position, duration).SetEase(Ease.Linear));
            
            // Rotar hacia B cuando llegue a A
            sequence.AppendCallback(() => {
                Vector3 directionToB2 = (pointB.position - pointA.position).normalized;
                if (directionToB2 != Vector3.zero)
                {
                    transform.DORotateQuaternion(Quaternion.LookRotation(directionToB2), 0.3f);
                }
            });
            
            // Loop infinito
            sequence.SetLoops(-1);
            movementTween = sequence;
        }
        else
        {
            // Movimiento en ciclo continuo con rotación
            Sequence sequence = DOTween.Sequence();
            
            // Ir de A a B
            sequence.Append(transform.DOMove(pointB.position, duration).SetEase(Ease.Linear));
            
            // Al llegar a B, teleportarse a A y rotar hacia B
            sequence.AppendCallback(() => {
                transform.position = pointA.position;
                Vector3 directionToB2 = (pointB.position - pointA.position).normalized;
                if (directionToB2 != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(directionToB2);
                }
            });
            
            // Loop infinito
            sequence.SetLoops(-1);
            movementTween = sequence;
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
        if (isDestroyed)
        {
            Debug.LogWarning($"⚠️ {gameObject.name} ya estaba destruido!");
            return;
        }

        isDestroyed = true;
        StopMovement();

        Debug.Log($"💥 Enemigo {gameObject.name} destruido por el láser!");

        // Notificar al GameManager
        if (gameManager != null)
        {
            gameManager.OnEnemyDestroyed(this);
            Debug.Log($"✅ GameManager notificado de la destrucción de {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"⚠️ GameManager es null! No se puede notificar la destrucción.");
        }

        // Animar destrucción y destruir el objeto
        transform.DOScale(Vector3.zero, destructionDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                Debug.Log($"🗑️ GameObject {gameObject.name} destruido completamente");
                Destroy(gameObject);
            });
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