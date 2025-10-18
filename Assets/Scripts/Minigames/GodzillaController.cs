using UnityEngine;
using DG.Tweening;
using System.Collections;

/// <summary>
/// Controlador principal para el minijuego de Godzilla
/// Maneja la rotación, animaciones y disparo del rayo láser sincronizado con audio
/// </summary>
public class GodzillaController : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    [Tooltip("Ángulo mínimo de rotación en Y")]
    [SerializeField] private float minRotationAngle = -40f;
    
    [Tooltip("Ángulo máximo de rotación en Y")]
    [SerializeField] private float maxRotationAngle = 40f;
    
    [Tooltip("Velocidad de rotación (grados por segundo)")]
    [SerializeField] private float rotationSpeed = 30f;

    [Header("Referencias")]
    [Tooltip("Animator del modelo de Godzilla")]
    [SerializeField] private Animator animator;
    
    [Tooltip("LineRenderer para el rayo láser")]
    [SerializeField] private LineRenderer laserBeam;
    
    [Tooltip("Transform desde donde sale el rayo (boca de Godzilla)")]
    [SerializeField] private Transform laserOrigin;

    [Header("Audio Configuration")]
    [Tooltip("AudioClipSO del rayo de Godzilla (25 segundos)")]
    [SerializeField] private AudioClipSO godzillaRayoDisparoAudio;

    [Header("Timing Configuration")]
    [Tooltip("Tiempo en el que termina la transición y empieza el Shoot (segundo 16)")]
    [SerializeField] private float shootStartTime = 16f;
    
    [Tooltip("Duración total del audio (25 segundos)")]
    [SerializeField] private float totalAudioDuration = 25f;

    [Header("Configuración del Láser")]
    [Tooltip("Distancia máxima del rayo")]
    [SerializeField] private float laserMaxDistance = 100f;
    
    [Tooltip("Grosor del rayo al inicio")]
    [SerializeField] private float laserStartWidth = 0.5f;
    
    [Tooltip("Grosor del rayo al final")]
    [SerializeField] private float laserEndWidth = 0.3f;

    [Header("Nombres de Animaciones")]
    [Tooltip("Nombre del trigger para iniciar la secuencia")]
    [SerializeField] private string attackTrigger = "Attack";
    
    [Tooltip("Nombre del estado Shoot")]
    [SerializeField] private string shootAnimationState = "Shoot";

    [Header("Efectos Visuales (Opcional)")]
    [SerializeField] private ParticleSystem laserChargeEffect;
    [SerializeField] private ParticleSystem laserImpactEffect;
    [SerializeField] private Light laserLight;

    [Header("Game Manager")]
    [Tooltip("Referencia al GameManager (opcional, se busca automáticamente)")]
    [SerializeField] private GodzillaGameManager gameManager;

    // Estado del juego
    private bool isRotating = true;
    private bool isAttacking = false;
    private float currentRotation = 0f;
    private int rotationDirection = 1; // 1 = derecha, -1 = izquierda
    private Vector3 lockedDirection;
    private float attackTimer = 0f;
    private bool laserFired = false;

    public bool IsAttacking => isAttacking;

    private void Start()
    {
        InitializeLaser();
        
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (laserOrigin == null)
        {
            Debug.LogWarning("No se asignó laserOrigin. Usando la posición del objeto.");
            laserOrigin = transform;
        }

        // Buscar el GameManager si no está asignado
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GodzillaGameManager>();
        }

        // Verificar que el SoundsController esté disponible
        if (SoundsController.Instance == null)
        {
            Debug.LogError("¡IMPORTANTE! No se encontró SoundsController en la escena.");
        }
        else
        {
            Debug.Log($"SoundsController encontrado. Listo para reproducir audio.");
        }
    }

    private void Update()
    {
        if (isRotating)
        {
            RotateModel();
        }

        // Detectar input para iniciar ataque
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking && isRotating)
        {
            StartAttackSequence();
        }

        // Manejar la secuencia de ataque sincronizada con el audio
        if (isAttacking)
        {
            HandleAttackSequence();
        }
    }

    /// <summary>
    /// Rota el modelo entre los ángulos mínimo y máximo
    /// </summary>
    private void RotateModel()
    {
        // Incrementar rotación
        currentRotation += rotationSpeed * rotationDirection * Time.deltaTime;

        // Cambiar dirección si alcanza los límites
        if (currentRotation >= maxRotationAngle)
        {
            currentRotation = maxRotationAngle;
            rotationDirection = -1;
        }
        else if (currentRotation <= minRotationAngle)
        {
            currentRotation = minRotationAngle;
            rotationDirection = 1;
        }

        // Aplicar rotación
        transform.rotation = Quaternion.Euler(0f, currentRotation, 0f);
    }

    /// <summary>
    /// Inicia la secuencia de ataque sincronizada con el audio de 25 segundos
    /// </summary>
    private void StartAttackSequence()
    {
        isRotating = false;
        isAttacking = true;
        attackTimer = 0f;
        laserFired = false;
        
        // Guardar la dirección actual
        lockedDirection = transform.forward;

        Debug.Log($"Ataque iniciado en dirección: {lockedDirection}");

        // Reproducir el audio de 25 segundos
        if (godzillaRayoDisparoAudio != null)
        {
            godzillaRayoDisparoAudio.PlayOneShoot();
            Debug.Log("Audio 'GodzillaRayoDisparo' reproducido (25 segundos)");
        }

        // Activar animación de ataque
        if (animator != null)
        {
            animator.SetTrigger(attackTrigger);
        }

        // Efecto de carga (opcional)
        if (laserChargeEffect != null)
        {
            laserChargeEffect.Play();
        }
    }

    /// <summary>
    /// Maneja la secuencia de ataque basada en el tiempo del audio
    /// 0-16s: Transiciones de animación (Idle → Init → Shoot_Previous)
    /// 16-25s: Estado Shoot con láser activo
    /// 25s: Volver a Idle
    /// </summary>
    private void HandleAttackSequence()
    {
        attackTimer += Time.deltaTime;

        // Del segundo 16 al 25: Disparar y mantener el láser
        if (attackTimer >= shootStartTime && !laserFired)
        {
            laserFired = true;
            FireLaser();
        }

        // Al terminar el audio (25 segundos): Finalizar secuencia
        if (attackTimer >= totalAudioDuration)
        {
            FinishAttackSequence();
        }
    }

    /// <summary>
    /// Dispara el rayo láser
    /// </summary>
    private void FireLaser()
    {
        Debug.Log("¡Disparando rayo láser!");

        // Calcular punto final del rayo
        Vector3 startPoint = laserOrigin.position;
        Vector3 endPoint = startPoint + lockedDirection * laserMaxDistance;

        // Raycast para detectar colisiones (detecta TODOS los objetos en el camino)
        RaycastHit[] hits = Physics.RaycastAll(startPoint, lockedDirection, laserMaxDistance);
        
        foreach (RaycastHit hit in hits)
        {
            // Verificar si es un enemigo
            GodzillaEnemy enemy = hit.collider.GetComponent<GodzillaEnemy>();
            if (enemy != null && !enemy.IsDestroyed)
            {
                Debug.Log($"¡Láser impactó al enemigo: {enemy.gameObject.name}!");
                enemy.DestroyEnemy();
            }

            // Actualizar punto final si hay colisión física
            if (hit.distance < Vector3.Distance(startPoint, endPoint))
            {
                endPoint = hit.point;
                
                // Efecto de impacto
                if (laserImpactEffect != null)
                {
                    laserImpactEffect.transform.position = hit.point;
                    laserImpactEffect.transform.rotation = Quaternion.LookRotation(hit.normal);
                    laserImpactEffect.Play();
                }

                Debug.Log($"Rayo impactó en: {hit.collider.gameObject.name}");
            }
        }

        // Activar el LineRenderer
        StartCoroutine(AnimateLaser(startPoint, endPoint));
    }

    /// <summary>
    /// Anima el efecto del láser
    /// Duración: Del segundo 16 al 25 (9 segundos de láser activo)
    /// </summary>
    private IEnumerator AnimateLaser(Vector3 start, Vector3 end)
    {
        laserBeam.enabled = true;
        laserBeam.SetPosition(0, start);
        laserBeam.SetPosition(1, start); // Empieza desde el origen

        // Activar luz si existe
        if (laserLight != null)
        {
            laserLight.enabled = true;
        }

        float elapsed = 0f;
        float growDuration = 0.3f; // Duración del crecimiento del rayo

        // FASE 1: Crecimiento del rayo (0.3 segundos)
        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / growDuration;
            
            Vector3 currentEnd = Vector3.Lerp(start, end, t);
            laserBeam.SetPosition(1, currentEnd);
            
            // Actualizar posición de la luz
            if (laserLight != null)
            {
                laserLight.transform.position = currentEnd;
            }

            yield return null;
        }

        laserBeam.SetPosition(1, end);

        // FASE 2: Mantener el rayo activo durante el resto del audio
        // Del segundo 16 al 25 = 9 segundos totales
        // Ya usamos 0.3s en crecimiento, quedan ~8.4s para mantener
        // Usaremos 0.3s para el fade, así que mantenemos por 8.4s
        float laserDuration = totalAudioDuration - shootStartTime; // 25 - 16 = 9 segundos
        float maintainDuration = laserDuration - growDuration - 0.3f;
        
        yield return new WaitForSeconds(maintainDuration);

        // FASE 3: Desvanecimiento (0.3 segundos)
        float fadeDuration = 0.3f;
        elapsed = 0f;

        AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            float alpha = fadeCurve.Evaluate(t);

            // Reducir ancho del rayo
            laserBeam.startWidth = laserStartWidth * alpha;
            laserBeam.endWidth = laserEndWidth * alpha;

            // Reducir intensidad de luz
            if (laserLight != null)
            {
                laserLight.intensity *= alpha;
            }

            yield return null;
        }

        // Desactivar todo
        laserBeam.enabled = false;
        
        if (laserLight != null)
        {
            laserLight.enabled = false;
        }

        // Resetear anchos
        laserBeam.startWidth = laserStartWidth;
        laserBeam.endWidth = laserEndWidth;

        Debug.Log("Láser desactivado. Esperando fin del audio...");
    }

    /// <summary>
    /// Finaliza la secuencia de ataque y vuelve al estado normal
    /// Se llama automáticamente cuando termina el audio (25 segundos)
    /// </summary>
    private void FinishAttackSequence()
    {
        isAttacking = false;
        isRotating = true;
        attackTimer = 0f;
        laserFired = false;

        // Detener el audio usando el SoundsController
        if (SoundsController.Instance != null)
        {
            SoundsController.Instance.StopCurrentClip();
            Debug.Log("Audio detenido.");
        }

        Debug.Log("Secuencia de ataque completada (audio terminado). Volviendo a rotar.");

        // Notificar al GameManager que la secuencia terminó
        if (gameManager != null)
        {
            gameManager.OnAttackSequenceComplete();
        }
    }

    /// <summary>
    /// Inicializa el LineRenderer del láser
    /// </summary>
    private void InitializeLaser()
    {
        if (laserBeam == null)
        {
            Debug.LogError("No se asignó LineRenderer para el láser!");
            return;
        }

        laserBeam.enabled = false;
        laserBeam.positionCount = 2;
        laserBeam.startWidth = laserStartWidth;
        laserBeam.endWidth = laserEndWidth;
        laserBeam.useWorldSpace = true;

        // Configurar material si no tiene
        if (laserBeam.material == null)
        {
            laserBeam.material = new Material(Shader.Find("Sprites/Default"));
        }

        // Color azul brillante para el rayo de Godzilla
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(Color.cyan, 0.0f), 
                new GradientColorKey(Color.blue, 1.0f) 
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1.0f, 0.0f), 
                new GradientAlphaKey(0.8f, 1.0f) 
            }
        );
        laserBeam.colorGradient = gradient;
    }

    /// <summary>
    /// Método público para forzar el disparo (para testing)
    /// </summary>
    [ContextMenu("Test Fire Laser")]
    public void TestFireLaser()
    {
        if (!isAttacking)
        {
            StartAttackSequence();
        }
    }

    private void OnDrawGizmos()
    {
        // Visualizar la dirección del disparo en el editor
        if (Application.isPlaying && !isRotating)
        {
            Gizmos.color = Color.red;
            Vector3 origin = laserOrigin != null ? laserOrigin.position : transform.position;
            Gizmos.DrawRay(origin, lockedDirection * 10f);
        }
    }
}
