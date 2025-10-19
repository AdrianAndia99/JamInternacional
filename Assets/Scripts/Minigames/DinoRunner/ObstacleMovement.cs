using UnityEngine;

/// <summary>
/// Script para los obstáculos que se mueven hacia el jugador
/// VERSIÓN PARA SPRITES 2D EN ENTORNO 3D
/// </summary>
public class ObstacleMovement : MonoBehaviour
{
    private Transform player;
    private float speed;
    private DinoRunner gameManager;
    private bool hasHit = false;

    /// <summary>
    /// Inicializa el obstáculo
    /// </summary>
    public void Initialize(Transform playerTransform, float moveSpeed, DinoRunner manager)
    {
        player = playerTransform;
        speed = moveSpeed;
        gameManager = manager;
        
        // Hacer que el obstáculo mire a la cámara (billboard para sprites 2D)
        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }

    private void Update()
    {
        if (player == null) return;

        // PARA SPRITES 2D: Mover en el eje X (horizontal)
        // Si tus sprites están en el plano XY (vista lateral)
        transform.position += Vector3.right * speed * Time.deltaTime; // Mover hacia la derecha (hacia el jugador)
        
        // Alternativa si tu setup es diferente:
        // transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);

        // Verificar si pasó al jugador (destruir si está muy a la derecha)
        if (transform.position.x > player.position.x + 2f)
        {
            // El jugador esquivó el obstáculo
            Debug.Log("✅ Obstáculo esquivado");
            Destroy(gameObject);
        }
        
        // Mantener el sprite mirando a la cámara
        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        // Si choca con el jugador
        if (other.CompareTag("Player"))
        {
            // Verificar si el jugador está lo suficientemente alto (saltando sobre el obstáculo)
            float playerY = other.transform.position.y;
            float obstacleY = transform.position.y;
            float clearanceHeight = 1f; // Altura mínima para esquivar el obstáculo
            
            if (playerY > obstacleY + clearanceHeight)
            {
                // El jugador saltó sobre el obstáculo exitosamente
                Debug.Log("🦘 ¡Obstáculo esquivado con salto!");
                hasHit = true; // Marcar como procesado para que no vuelva a detectar
                Destroy(gameObject);
                return;
            }
            
            // Si no saltó lo suficiente, recibe el golpe
            hasHit = true;
            Debug.Log("💥 Obstáculo golpeó al jugador");
            
            if (gameManager != null)
            {
                gameManager.OnObstacleHit();
            }

            Destroy(gameObject);
        }
    }
    
    // Para colisiones 2D si usas Collider2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            // Verificar si el jugador está lo suficientemente alto (saltando sobre el obstáculo)
            float playerY = other.transform.position.y;
            float obstacleY = transform.position.y;
            float clearanceHeight = 1f; // Altura mínima para esquivar el obstáculo
            
            if (playerY > obstacleY + clearanceHeight)
            {
                // El jugador saltó sobre el obstáculo exitosamente
                Debug.Log("🦘 ¡Obstáculo esquivado con salto! (2D)");
                hasHit = true; // Marcar como procesado para que no vuelva a detectar
                Destroy(gameObject);
                return;
            }
            
            // Si no saltó lo suficiente, recibe el golpe
            hasHit = true;
            Debug.Log("💥 Obstáculo golpeó al jugador (2D)");
            
            if (gameManager != null)
            {
                gameManager.OnObstacleHit();
            }

            Destroy(gameObject);
        }
    }
}
