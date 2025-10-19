using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DinoRunner : MonoBehaviour
{
    [Header("Referencias del Jugador")]
    [Tooltip("Transform del personaje jugador")]
    [SerializeField] private Transform player;
    
    [Tooltip("Animator del personaje (para animaciones de piernas)")]
    [SerializeField] private Animator playerAnimator;

    [Header("Referencias del Dinosaurio")]
    [Tooltip("Transform del dinosaurio perseguidor")]
    [SerializeField] private Transform dinosaur;
    
    [Tooltip("Animator del dinosaurio")]
    [SerializeField] private Animator dinosaurAnimator;

    [Header("Configuración de Distancia")]
    [Tooltip("Distancia inicial entre jugador y dinosaurio")]
    [SerializeField] private float initialDistance = 10f;
    
    [Tooltip("Distancia mínima (si llega aquí, el dinosaurio te atrapa)")]
    [SerializeField] private float minDistance = 2f;
    
    [Tooltip("Distancia máxima (victoria si llegas aquí)")]
    [SerializeField] private float maxDistance = 20f;

    [Header("Configuración de Mecánica")]
    [Tooltip("Cuánto se aleja el dinosaurio al presionar correctamente")]
    [SerializeField] private float correctInputDistance = 0.5f;
    
    [Tooltip("Cuánto se acerca el dinosaurio al cometer error")]
    [SerializeField] private float wrongInputDistance = 1f;
    
    [Tooltip("Velocidad a la que el dinosaurio se acerca automáticamente")]
    [SerializeField] private float autoApproachSpeed = 0.5f;
    
    [Tooltip("Tiempo máximo entre inputs (si tardas más, es error)")]
    [SerializeField] private float maxInputDelay = 1.5f;

    [Header("Configuración de Obstáculos")]
    [Tooltip("Prefab del obstáculo")]
    [SerializeField] private GameObject obstaclePrefab;
    
    [Tooltip("Punto de spawn de obstáculos (adelante del jugador)")]
    [SerializeField] private Transform obstacleSpawnPoint;
    
    [Tooltip("Velocidad de movimiento de obstáculos hacia el jugador")]
    [SerializeField] private float obstacleSpeed = 5f;
    
    [Tooltip("Intervalo entre spawns de obstáculos")]
    [SerializeField] private float obstacleSpawnInterval = 3f;

    [Header("Sistema de Salto")]
    [Tooltip("Altura del salto")]
    [SerializeField] private float jumpHeight = 2f;
    
    [Tooltip("Duración del salto")]
    [SerializeField] private float jumpDuration = 0.5f;


    [Header("UI - Indicadores")]
    [Tooltip("Texto que muestra qué tecla presionar (A o D)")]
    [SerializeField] private TextMeshProUGUI nextKeyText;
    
    [Tooltip("Texto que muestra la distancia actual")]
    [SerializeField] private TextMeshProUGUI distanceText;
    
    [Tooltip("Imagen o indicador visual para la tecla A")]
    [SerializeField] private Image keyAIndicator;
    
    [Tooltip("Imagen o indicador visual para la tecla D")]
    [SerializeField] private Image keyDIndicator;
    
    [Tooltip("Color cuando la tecla es correcta")]
    [SerializeField] private Color correctKeyColor = Color.green;
    
    [Tooltip("Color cuando la tecla es incorrecta")]
    [SerializeField] private Color incorrectKeyColor = Color.red;
    
    [Tooltip("Color neutral")]
    [SerializeField] private Color neutralKeyColor = Color.white;

    [Header("UI - Paneles de Resultado")]
    [Tooltip("Panel de victoria (escapaste del dinosaurio)")]
    [SerializeField] private GameObject victoryPanel;
    
    [Tooltip("Panel de derrota (el dinosaurio te atrapó)")]
    [SerializeField] private GameObject defeatPanel;

    [Header("Audio")]
    [Tooltip("Audio de paso correcto")]
    [SerializeField] private AudioClipSO correctStepAudio;
    
    [Tooltip("Audio de error")]
    [SerializeField] private AudioClipSO wrongStepAudio;
    
    [Tooltip("Audio de victoria")]
    [SerializeField] private AudioClipSO victoryAudio;
    
    [Tooltip("Audio de derrota")]
    [SerializeField] private AudioClipSO defeatAudio;

    [Header("Nombres de Animaciones")]
    [SerializeField] private string playerRunAnimation = "Run";
    [SerializeField] private string playerIdleAnimation = "Idle";
    [SerializeField] private string dinosaurChaseAnimation = "Run";

    // Estado del juego
    private float currentDistance;
    private bool expectingA = true; // Alterna entre A y D
    private bool gameActive = false;
    private bool gameEnded = false;
    private float lastInputTime;
    private float obstacleTimer;
    private int correctInputsCount = 0;
    private int wrongInputsCount = 0;
    
    // Estado del salto
    private bool isJumping = false;
    private float playerStartY;
    [SerializeField] private float dinosaurStartY; // Posición Y inicial del dinosaurio

    private void Start()
    {
        // Configurar distancia inicial
        currentDistance = initialDistance;
        UpdateDinosaurPosition();
        
        // Guardar posición Y inicial del jugador y dinosaurio
        if (player != null)
        {
            playerStartY = player.position.y;
        }
        
        if (dinosaur != null)
        {
            dinosaurStartY = dinosaur.position.y;
        }

        // Ocultar paneles
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);

        // Iniciar el juego
        StartGame();
    }

    private void Update()
    {
        if (!gameActive || gameEnded) return;

        // Verificar timeout de input
        if (Time.time - lastInputTime > maxInputDelay)
        {
            ProcessWrongInput();
            lastInputTime = Time.time;
        }

        // El dinosaurio se acerca automáticamente con el tiempo
        currentDistance -= autoApproachSpeed * Time.deltaTime;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        // Actualizar posición del dinosaurio
        UpdateDinosaurPosition();

        // Actualizar UI
        UpdateUI();

        // Detectar inputs de correr (A/D)
        DetectInput();
        
        // Detectar input de salto (ESPACIO)
        DetectJump();

        // Sistema de obstáculos
        HandleObstacles();

        // Verificar condiciones de victoria/derrota
        CheckWinLoseConditions();
    }

    /// <summary>
    /// Inicia el juego
    /// </summary>
    private void StartGame()
    {
        gameActive = true;
        gameEnded = false;
        currentDistance = initialDistance;
        expectingA = true;
        lastInputTime = Time.time;
        obstacleTimer = 0f;
        correctInputsCount = 0;
        wrongInputsCount = 0;

        // Iniciar animaciones
        if (playerAnimator != null)
        {
            playerAnimator.Play(playerRunAnimation);
        }

        if (dinosaurAnimator != null)
        {
            dinosaurAnimator.Play(dinosaurChaseAnimation);
        }

        Debug.Log("🦖 ¡Juego iniciado! Presiona A y D alternadamente para correr.");
        UpdateUI();
    }

    /// <summary>
    /// Detecta el input del jugador
    /// </summary>
    private void DetectInput()
    {
        bool pressedA = Input.GetKeyDown(KeyCode.A);
        bool pressedD = Input.GetKeyDown(KeyCode.D);

        if (pressedA || pressedD)
        {
            bool isCorrect = (expectingA && pressedA) || (!expectingA && pressedD);

            if (isCorrect)
            {
                ProcessCorrectInput();
            }
            else
            {
                ProcessWrongInput();
            }

            // Alternar la tecla esperada
            expectingA = !expectingA;
            lastInputTime = Time.time;
        }
    }

    /// <summary>
    /// Procesa un input correcto
    /// </summary>
    private void ProcessCorrectInput()
    {
        correctInputsCount++;
        
        // Alejar al dinosaurio
        currentDistance += correctInputDistance;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        Debug.Log($"✅ ¡Correcto! Distancia: {currentDistance:F1}m");

        // Audio de paso correcto
        if (correctStepAudio != null)
        {
            correctStepAudio.PlayOneShoot();
        }

        // Feedback visual
        StartCoroutine(FlashKeyIndicator(true));
    }

    /// <summary>
    /// Procesa un input incorrecto o timeout
    /// </summary>
    private void ProcessWrongInput()
    {
        wrongInputsCount++;
        
        // Acercar al dinosaurio
        currentDistance -= wrongInputDistance;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        Debug.Log($"❌ ¡Error! El dinosaurio se acerca. Distancia: {currentDistance:F1}m");

        // Audio de error
        if (wrongStepAudio != null)
        {
            wrongStepAudio.PlayOneShoot();
        }

        // Feedback visual
        StartCoroutine(FlashKeyIndicator(false));
    }

    /// <summary>
    /// Detecta el input de salto
    /// </summary>
    private void DetectJump()
    {
        // Solo permitir saltar si no está ya saltando
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            StartCoroutine(JumpCoroutine());
        }
    }

    /// <summary>
    /// Corrutina que ejecuta el salto del jugador
    /// </summary>
    private IEnumerator JumpCoroutine()
    {
        isJumping = true;

        float elapsedTime = 0f;
        float halfDuration = jumpDuration / 2f;

        // Fase de subida
        while (elapsedTime < halfDuration)
        {
            float progress = elapsedTime / halfDuration;
            float newY = Mathf.Lerp(playerStartY, playerStartY + jumpHeight, progress);
            player.position = new Vector3(player.position.x, newY, player.position.z);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegurar que llegó al punto máximo
        player.position = new Vector3(player.position.x, playerStartY + jumpHeight, player.position.z);

        // Fase de bajada
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            float progress = elapsedTime / halfDuration;
            float newY = Mathf.Lerp(playerStartY + jumpHeight, playerStartY, progress);
            player.position = new Vector3(player.position.x, newY, player.position.z);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegurar que regresó al suelo
        player.position = new Vector3(player.position.x, playerStartY, player.position.z);
        
        isJumping = false;
        Debug.Log("🦘 Salto completado");
    }

    /// <summary>
    /// Actualiza la posición visual del dinosaurio según la distancia
    /// VERSIÓN PARA SPRITES 2D EN ENTORNO 3D
    /// </summary>
    private void UpdateDinosaurPosition()
    {
        if (dinosaur == null || player == null) return;

        // Para sprites 2D, trabajamos en el plano XY o XZ
        // Asumiendo que los sprites están en el plano XY (vista lateral)
        // El dinosaurio se posiciona a la IZQUIERDA del jugador
        
        // IMPORTANTE: Solo actualizar la posición X (horizontal), mantener Y fija
        // Esto evita que el dinosaurio salte cuando el jugador salta
        Vector3 offset = new Vector3(-currentDistance, 0f, 0f); // A la izquierda
        
        // Crear nueva posición manteniendo la Y original del dinosaurio
        Vector3 newPosition = player.position + offset;
        newPosition.y = dinosaurStartY; // Mantener al dinosaurio siempre en su altura inicial
        
        dinosaur.position = newPosition;
        
        // IMPORTANTE: Hacer que el sprite siempre mire a la cámara (billboard)
        // Esto evita que se vea de lado en un entorno 3D
        if (Camera.main != null)
        {
            dinosaur.rotation = Camera.main.transform.rotation;
        }
    }

    /// <summary>
    /// Actualiza todos los elementos de la UI
    /// </summary>
    private void UpdateUI()
    {
        // Mostrar qué tecla presionar
        if (nextKeyText != null)
        {
            nextKeyText.text = expectingA ? "Presiona: A" : "Presiona: D";
        }

        // Mostrar distancia
        if (distanceText != null)
        {
            distanceText.text = $"Distancia: {currentDistance:F1}m\nCorrectos: {correctInputsCount} | Errores: {wrongInputsCount}";
        }

        // Actualizar indicadores de teclas
        UpdateKeyIndicators();
    }

    /// <summary>
    /// Actualiza los indicadores visuales de las teclas
    /// </summary>
    private void UpdateKeyIndicators()
    {
        if (keyAIndicator != null)
        {
            keyAIndicator.color = expectingA ? correctKeyColor : neutralKeyColor;
        }

        if (keyDIndicator != null)
        {
            keyDIndicator.color = !expectingA ? correctKeyColor : neutralKeyColor;
        }
    }

    /// <summary>
    /// Efecto visual de flash en los indicadores
    /// </summary>
    private System.Collections.IEnumerator FlashKeyIndicator(bool correct)
    {
        Color flashColor = correct ? correctKeyColor : incorrectKeyColor;
        Image indicator = expectingA ? keyDIndicator : keyAIndicator; // La que acabas de presionar

        if (indicator != null)
        {
            indicator.color = flashColor;
            yield return new WaitForSeconds(0.2f);
            indicator.color = neutralKeyColor;
        }
    }

    /// <summary>
    /// Maneja el sistema de obstáculos
    /// </summary>
    private void HandleObstacles()
    {
        obstacleTimer += Time.deltaTime;

        if (obstacleTimer >= obstacleSpawnInterval)
        {
            SpawnObstacle();
            obstacleTimer = 0f;
        }
    }

    /// <summary>
    /// Spawna un obstáculo
    /// </summary>
    private void SpawnObstacle()
    {
        if (obstaclePrefab == null || obstacleSpawnPoint == null) return;

        GameObject obstacle = Instantiate(obstaclePrefab, obstacleSpawnPoint.position, Quaternion.identity);
        
        // Agregar componente de movimiento al obstáculo
        ObstacleMovement obstacleScript = obstacle.AddComponent<ObstacleMovement>();
        obstacleScript.Initialize(player, obstacleSpeed, this);

        Debug.Log("🚧 Obstáculo spawneado");
    }

    /// <summary>
    /// Llamado por el obstáculo cuando el jugador falla en esquivarlo
    /// </summary>
    public void OnObstacleHit()
    {
        Debug.Log("💥 ¡Chocaste con un obstáculo!");
        ProcessWrongInput();
    }

    /// <summary>
    /// Verifica las condiciones de victoria y derrota
    /// </summary>
    private void CheckWinLoseConditions()
    {
        // Derrota: El dinosaurio te alcanzó
        if (currentDistance <= minDistance)
        {
            TriggerDefeat();
        }

        // Victoria: Escapaste lo suficiente
        if (currentDistance >= maxDistance)
        {
            TriggerVictory();
        }
    }

    /// <summary>
    /// Activa la secuencia de victoria
    /// </summary>
    private void TriggerVictory()
    {
        if (gameEnded) return;

        gameEnded = true;
        gameActive = false;

        Debug.Log("🎉 ¡VICTORIA! ¡Escapaste del dinosaurio!");

        // Audio
        if (victoryAudio != null)
        {
            victoryAudio.PlayOneShoot();
        }

        // Panel
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        // Detener animaciones
        if (playerAnimator != null)
        {
            playerAnimator.Play(playerIdleAnimation);
        }
    }

    /// <summary>
    /// Activa la secuencia de derrota
    /// </summary>
    private void TriggerDefeat()
    {
        if (gameEnded) return;

        gameEnded = true;
        gameActive = false;

        Debug.Log("💀 ¡DERROTA! ¡El dinosaurio te atrapó!");

        // Audio
        if (defeatAudio != null)
        {
            defeatAudio.PlayOneShoot();
        }

        // Panel
        if (defeatPanel != null)
        {
            defeatPanel.SetActive(true);
        }

        // Detener animaciones
        if (playerAnimator != null)
        {
            playerAnimator.Play(playerIdleAnimation);
        }
    }

    /// <summary>
    /// Reinicia el juego
    /// </summary>
    public void RestartGame()
    {
        // Ocultar paneles
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);

        // Limpiar obstáculos
        ObstacleMovement[] obstacles = FindObjectsByType<ObstacleMovement>(FindObjectsSortMode.None);
        foreach (var obs in obstacles)
        {
            Destroy(obs.gameObject);
        }

        // Reiniciar
        StartGame();
    }

    /// <summary>
    /// Vuelve al menú
    /// </summary>
    public void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuTest");
    }
}
