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
    [Tooltip("Tiempo antes de disparar automáticamente si no se presiona SPACE")]
    [SerializeField] private float autoShootTime = 16f;

    [Header("Configuración del Láser")]
    [Tooltip("Distancia máxima del rayo")]
    [SerializeField] private float laserMaxDistance = 1000f;
    
    [Tooltip("Grosor del rayo al inicio")]
    [SerializeField] private float laserStartWidth = 0.5f;
    
    [Tooltip("Grosor del rayo al final")]
    [SerializeField] private float laserEndWidth = 0.3f;
    
    [Tooltip("Duración del rayo láser en segundos (cuánto tiempo permanece activo)")]
    [SerializeField] private float laserDuration = 9f;
    
    [Tooltip("Layer de los enemigos (crea un layer 'GodzillaEnemy' y asígnalo aquí)")]
    [SerializeField] private LayerMask enemyLayer;

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
    private bool playerLockedDirection = false; // Si el jugador presionó SPACE
    private float currentRotation = 0f;
    private int rotationDirection = 1; // 1 = derecha, -1 = izquierda
    private Vector3 lockedDirection;
    private float gameTimer = 0f; // Temporizador desde el inicio del juego
    private bool laserFired = false;

    public bool IsRotating => isRotating;

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
        laserChargeEffect.Play();
        // Buscar el GameManager si no está asignado
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GodzillaGameManager>();
        }

        // Iniciar la animación de ataque desde el principio
        if (animator != null)
        {
            animator.SetTrigger(attackTrigger);
            Debug.Log("🎬 Animación iniciada al comenzar la escena");
        }

        // Reproducir el audio desde el inicio
        if (godzillaRayoDisparoAudio != null)
        {
            godzillaRayoDisparoAudio.PlayOneShoot();
            Debug.Log("🔊 Audio reproducido al inicio");
        }
    }

    private void Update()
    {
        // Incrementar el temporizador del juego
        gameTimer += Time.deltaTime;

        // Rotar mientras no se haya disparado
        if (isRotating)
        {
            RotateModel();
            
            // Debug: Presiona Q para ver si apuntas al enemigo
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CheckIfAimingAtEnemy();
            }
            
            // Si el jugador presiona SPACE, congelar la dirección
            if (Input.GetKeyDown(KeyCode.Space) && !playerLockedDirection)
            {
                LockDirection();
            }
        }

        // Disparo automático cuando se acaba el tiempo
        if (!laserFired && gameTimer >= autoShootTime)
        {
            // Si el jugador no congeló la dirección, congelar ahora
            if (!playerLockedDirection)
            {
                LockDirection();
                Debug.Log("⏰ Tiempo agotado. Disparando automáticamente en la última dirección.");
            }
            
            FireLaser();
        }
    }

    /// <summary>
    /// Congela la dirección actual de Godzilla
    /// </summary>
    private void LockDirection()
    {
        isRotating = false;
        playerLockedDirection = true;
        
        // Calcular y guardar la dirección
        float angleInRadians = currentRotation * Mathf.Deg2Rad;
        lockedDirection = new Vector3(Mathf.Sin(angleInRadians), 0f, Mathf.Cos(angleInRadians));
        
        Debug.Log($"🔒 Dirección congelada en: {currentRotation:F1}° - Dirección: {lockedDirection}");
        Debug.Log($"⏱️ Tiempo restante para disparo: {(autoShootTime - gameTimer):F1}s");
    }

    /// <summary>
    /// Verifica si estamos apuntando a un enemigo (ayuda visual en consola)
    /// Presiona Q para verificar si apuntas al enemigo
    /// </summary>
    private void CheckIfAimingAtEnemy()
    {
        float angleInRadians = currentRotation * Mathf.Deg2Rad;
        Vector3 currentDirection = new Vector3(Mathf.Sin(angleInRadians), 0f, Mathf.Cos(angleInRadians));
        
        Debug.Log($"🎯 Verificando dirección actual:");
        Debug.Log($"   Ángulo: {currentRotation:F1}°");
        Debug.Log($"   Dirección: {currentDirection}");
        
        // Raycast para ver si apuntamos a un enemigo
        RaycastHit[] hits = Physics.RaycastAll(laserOrigin.position, currentDirection, laserMaxDistance, enemyLayer);
        
        if (hits.Length > 0)
        {
            Debug.Log($"   ✅ ¡APUNTANDO A {hits.Length} ENEMIGO(S)! Presiona SPACE ahora para atacar");
            foreach (var hit in hits)
            {
                Debug.Log($"      - {hit.collider.name} (distancia: {hit.distance:F1})");
            }
        }
        else
        {
            Debug.LogWarning($"   ❌ NO apuntas a ningún enemigo en este ángulo");
            Debug.LogWarning($"      Continúa rotando y vuelve a presionar Q");
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
    /// Dispara el rayo láser (solo una vez)
    /// </summary>
    private void FireLaser()
    {
        laserFired = true;
        
        Debug.Log("⚡ ¡Disparando rayo láser!");

        // Calcular punto final del rayo
        Vector3 startPoint = laserOrigin.position;
        Vector3 endPoint = startPoint + lockedDirection * laserMaxDistance;

        Debug.Log($"🎯 Origen del rayo: {startPoint}");
        Debug.Log($"🎯 Dirección del rayo: {lockedDirection}");
        Debug.Log($"🎯 Distancia máxima: {laserMaxDistance}");

        // Raycast para detectar colisiones SOLO en el layer de enemigos
        RaycastHit[] hits = Physics.RaycastAll(startPoint, lockedDirection, laserMaxDistance, enemyLayer);
        
        Debug.Log($"🔍 Raycast detectó {hits.Length} enemigo(s)");

        bool hitEnemy = false;
        
        foreach (RaycastHit hit in hits)
        {
            Debug.Log($"🎯 Raycast impactó: {hit.collider.gameObject.name} - Distancia: {hit.distance:F1}m");

            // Verificar si es un enemigo
            GodzillaEnemy enemy = hit.collider.GetComponent<GodzillaEnemy>();
            if (enemy != null && !enemy.IsDestroyed)
            {
                Debug.Log($"💥 ¡IMPACTO! Enemigo {enemy.gameObject.name} destruido!");
                enemy.DestroyEnemy();
                hitEnemy = true;
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
            }
        }

        // Activar el LineRenderer para mostrar el láser
        StartCoroutine(AnimateLaser(startPoint, endPoint));
        
        // Resultado final después de un delay (esperar a que el láser se vea)
        StartCoroutine(ShowResult(hitEnemy, 2f));
    }

    /// <summary>
    /// Muestra el resultado del disparo (Victoria o Derrota)
    /// </summary>
    private IEnumerator ShowResult(bool hitEnemy, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (hitEnemy)
        {
            Debug.Log("🎉 ¡VICTORIA! Enemigo eliminado");
            if (gameManager != null)
            {
                gameManager.TriggerVictory();
            }
        }
        else
        {
            Debug.Log("💀 ¡DERROTA! Fallaste el disparo");
            if (gameManager != null)
            {
                gameManager.TriggerDefeat();
            }
        }
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

        // FASE 2: Mantener el rayo activo durante la duración configurada
        float fadeDuration = 0.3f;
        float maintainDuration = laserDuration - growDuration - fadeDuration;
        
        Debug.Log($"⚡ Manteniendo rayo activo por {maintainDuration:F1} segundos");
        yield return new WaitForSeconds(maintainDuration);

        // FASE 3: Desvanecimiento (0.3 segundos)
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

    private void OnDrawGizmos()
    {
        // Visualizar la dirección del disparo en el editor
        if (Application.isPlaying && !isRotating && laserOrigin != null)
        {
            Gizmos.color = Color.red;
            Vector3 origin = laserOrigin.position;
            Gizmos.DrawRay(origin, lockedDirection * laserMaxDistance);
            Gizmos.DrawWireSphere(origin, 0.5f);

            // Dibujar texto en la escena
            UnityEngine.GUI.color = Color.yellow;
        }

        // Visualizar el rayo activo durante el disparo
        if (Application.isPlaying && laserBeam != null && laserBeam.enabled)
        {
            Gizmos.color = Color.cyan;
            if (laserOrigin != null)
            {
                Gizmos.DrawRay(laserOrigin.position, lockedDirection * 10f);
            }
        }
    }
}
