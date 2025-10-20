using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DoorFight : MonoBehaviour
{
    [Header("Referencias de la Puerta")]
    [Tooltip("Transform de la puerta que va a rotar")]
    [SerializeField] private Transform door;
    
    [Tooltip("Ángulo inicial de la puerta (0)")]
    [SerializeField] private float initialAngle = 0f;
    
    [Tooltip("Ángulo de derrota (cuando la IA gana)")]
    [SerializeField] private float defeatAngle = -90f;

    [Header("Configuración del Juego")]
    [Tooltip("Duración del juego en segundos")]
    [SerializeField] private float gameDuration = 10f;
    
    [Tooltip("Velocidad de incremento de la IA por segundo")]
    [SerializeField] private float aiIncrementSpeed = 6f;
    
    [Tooltip("Cantidad mínima que el jugador reduce al presionar")]
    [SerializeField] private int minPlayerDecrement = 1;
    
    [Tooltip("Cantidad máxima que el jugador reduce al presionar")]
    [SerializeField] private int maxPlayerDecrement = 5;

    [Header("UI - Textos de Porcentaje")]
    [Tooltip("Texto que muestra el porcentaje del jugador")]
    [SerializeField] private TextMeshProUGUI playerPercentageText;
    
    [Tooltip("Texto que muestra el porcentaje de la IA")]
    [SerializeField] private TextMeshProUGUI aiPercentageText;
    
    [Tooltip("Texto del temporizador (opcional)")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("UI - Botón de Acción")]
    [Tooltip("Botón que el jugador debe presionar para reducir el porcentaje de la IA")]
    [SerializeField] private Button actionButton;
    
    [Tooltip("Texto del botón (opcional, para feedback visual)")]
    [SerializeField] private TextMeshProUGUI buttonText;

   /* [Header("UI - Paneles de Resultado")]
    [Tooltip("Panel que se muestra al ganar")]
    [SerializeField] private GameObject victoryPanel;
    
    [Tooltip("Panel que se muestra al perder")]
    [SerializeField] private GameObject defeatPanel; */ // paneles de victoria y derrota

    /*[Header("Audio (Opcional)")]
    [Tooltip("AudioClipSO para victoria")]
    [SerializeField] private AudioClipSO victoryAudio;
    
    [Tooltip("AudioClipSO para derrota")]
    [SerializeField] private AudioClipSO defeatAudio; */  //referencias al audio
    
    [Tooltip("AudioClipSO para el efecto de la puerta (loop)")]
    [SerializeField] private AudioClipSO doorEffectAudio;
    
    [Tooltip("Retraso antes de iniciar el audio de la puerta (en segundos)")]
    [SerializeField] private float doorAudioDelay = 1f;

    // Estado del juego
    private float playerPercentage = 0f;
    private float aiPercentage = 0f;
    private float currentAngle = 0f;
    private float gameTimer = 0f;
    private bool gameActive = false;
    private bool gameEnded = false;

    private void Start()
    {
        // Configurar la puerta en el ángulo inicial
        if (door != null)
        {
            currentAngle = initialAngle;
            UpdateDoorRotation();
        }

        // Ocultar paneles de resultado
      //  if (victoryPanel != null) victoryPanel.SetActive(false);
       // if (defeatPanel != null) defeatPanel.SetActive(false); //desactivar paneles de victoria y derrota

        // Configurar el botón
        if (actionButton != null)
        {
            actionButton.onClick.AddListener(OnActionButtonPressed);
        }

        // Iniciar el juego
        StartGame();
    }

    private void Update()
    {
        if (!gameActive || gameEnded) return;

        // Incrementar temporizador
        gameTimer += Time.deltaTime;

        // Incrementar el porcentaje de la IA gradualmente
        aiPercentage += aiIncrementSpeed * Time.deltaTime;
        aiPercentage = Mathf.Clamp(aiPercentage, 0f, 100f);

        // El porcentaje del jugador es el complemento del de la IA (suman 100%)
        playerPercentage = 100f - aiPercentage;

        // Actualizar el ángulo de la puerta basado en el porcentaje de la IA
        UpdateDoorAngle();

        // Actualizar UI
        UpdateUI();

        // Verificar condición de derrota (ángulo llegó a -90)
        if (currentAngle <= defeatAngle)
        {
            TriggerDefeat();
            return;
        }

        // Verificar condición de victoria (tiempo completado)
        if (gameTimer >= gameDuration)
        {
            TriggerVictory();
        }
    }

    /// <summary>
    /// Inicia el juego
    /// </summary>
    private void StartGame()
    {
        gameActive = true;
        gameEnded = false;
        gameTimer = 0f;
        
        // Iniciar con porcentajes balanceados
        aiPercentage = 0f;
        playerPercentage = 100f;
        currentAngle = initialAngle;

        Debug.Log("🎮 ¡Juego iniciado! Mantén la puerta abierta por 10 segundos.");
        
        UpdateUI();
        
        // Iniciar el audio de la puerta con delay
        if (doorEffectAudio != null)
        {
            StartCoroutine(StartDoorAudioWithDelay());
        }
    }

    /// <summary>
    /// Se llama cuando el jugador presiona el botón
    /// </summary>
    private void OnActionButtonPressed()
    {
        if (!gameActive || gameEnded) return;

        // Reducir el porcentaje de la IA de manera aleatoria
        int reduction = Random.Range(minPlayerDecrement, maxPlayerDecrement + 1);
        aiPercentage -= reduction;
        aiPercentage = Mathf.Clamp(aiPercentage, 0f, 100f);
        
        // El porcentaje del jugador se actualiza automáticamente como el complemento
        playerPercentage = 100f - aiPercentage;

        Debug.Log($"🎯 Jugador presionó el botón! Redujo {reduction}% a la IA (Jugador: {playerPercentage:F0}%, IA: {aiPercentage:F0}%)");

        // Feedback visual (opcional)
        if (buttonText != null)
        {
            buttonText.text = $"-{reduction}%";
            Invoke(nameof(ResetButtonText), 0.3f);
        }

        UpdateUI();
    }

    /// <summary>
    /// Resetea el texto del botón
    /// </summary>
    private void ResetButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = "¡JALA!";
        }
    }

    /// <summary>
    /// Actualiza el ángulo de la puerta basado en el porcentaje de la IA
    /// </summary>
    private void UpdateDoorAngle()
    {
        // Interpolar el ángulo de la puerta entre initialAngle (0%) y defeatAngle (100%)
        currentAngle = Mathf.Lerp(initialAngle, defeatAngle, aiPercentage / 100f);
        UpdateDoorRotation();
    }

    /// <summary>
    /// Aplica la rotación a la puerta
    /// </summary>
    private void UpdateDoorRotation()
    {
        if (door != null)
        {
            door.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
        }
    }

    /// <summary>
    /// Actualiza todos los elementos de la UI
    /// </summary>
    private void UpdateUI()
    {
        // Actualizar texto de porcentaje del jugador (solo número)
        if (playerPercentageText != null)
        {
            playerPercentageText.text = $"{playerPercentage:F0}%";
        }

        // Actualizar texto de porcentaje de la IA (solo número)
        if (aiPercentageText != null)
        {
            aiPercentageText.text = $"{aiPercentage:F0}%";
        }

        // Actualizar texto del temporizador (solo tiempo)
        if (timerText != null)
        {
            float timeRemaining = gameDuration - gameTimer;
            timerText.text = $"{timeRemaining:F1}s";
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

        Debug.Log("🎉 ¡VICTORIA! Mantuviste la puerta abierta.");

        // Detener el audio de la puerta
        StopDoorAudio();

        // Reproducir audio de victoria
      /*  if (victoryAudio != null)
        {
            victoryAudio.PlayOneShoot();
        }*/  //si ganas reproduce audio de victoria

        // Mostrar panel de victoria
       /* if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }*/  //mostrar panel de victoria

        // Deshabilitar botón
        if (actionButton != null)
        {
            actionButton.interactable = false;
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

        Debug.Log("💀 ¡DERROTA! La IA cerró la puerta completamente.");

        // Detener el audio de la puerta
        StopDoorAudio();

        // Reproducir audio de derrota
       /* if (defeatAudio != null)
        {
            defeatAudio.PlayOneShoot();
        }*/  //si pierdes reproduce audio de derrota

        // Mostrar panel de derrota
       /* if (defeatPanel != null)
        {
            defeatPanel.SetActive(true);
        }*/  //mostrar panel de derrota

        // Deshabilitar botón
        if (actionButton != null)
        {
            actionButton.interactable = false;
        }
    }

    /// <summary>
    /// Reinicia el juego
    /// </summary>
    public void RestartGame()
    {
        // Ocultar paneles
       // if (victoryPanel != null) victoryPanel.SetActive(false);
       // if (defeatPanel != null) defeatPanel.SetActive(false);

        // Habilitar botón
        if (actionButton != null)
        {
            actionButton.interactable = true;
        }

        // Reiniciar el juego
        StartGame();
    }

    /// <summary>
    /// Vuelve al menú principal
    /// </summary>
    public void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuTest");
    }

    /// <summary>
    /// Corrutina que inicia el audio de la puerta con delay
    /// </summary>
    private IEnumerator StartDoorAudioWithDelay()
    {
        // Esperar el delay configurado
        yield return new WaitForSeconds(doorAudioDelay);

        // Reproducir el audio en loop
        if (doorEffectAudio != null)
        {
            doorEffectAudio.PlayLoop();
            Debug.Log("🚪 Audio de puerta iniciado en loop");
        }
    }

    /// <summary>
    /// Detiene el audio de la puerta
    /// </summary>
    private void StopDoorAudio()
    {
        if (doorEffectAudio != null)
        {
            doorEffectAudio.StopPlay();
            Debug.Log("🚪 Audio de puerta detenido");
        }
    }
}
