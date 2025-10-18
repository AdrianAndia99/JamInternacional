using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GodzillaGameManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("Panel que se muestra al ganar")]
    [SerializeField] private GameObject victoryPanel;
    
    [Tooltip("Panel que se muestra al perder")]
    [SerializeField] private GameObject defeatPanel;
    
    [Tooltip("Texto del panel de victoria")]
    [SerializeField] private TextMeshProUGUI victoryText;
    
    [Tooltip("Texto del panel de derrota")]
    [SerializeField] private TextMeshProUGUI defeatText;

    [Header("Audio")]
    [Tooltip("AudioClipSO del rugido de victoria")]
    [SerializeField] private AudioClipSO victoryRoarAudio;

    [Header("Referencias")]
    [Tooltip("Referencia al controlador de Godzilla (opcional)")]
    [SerializeField] private GodzillaController godzillaController;

    // Estado del juego
    private List<GodzillaEnemy> enemies = new List<GodzillaEnemy>();
    private bool gameEnded = false;

    private void Start()
    {
        // Ocultar paneles al inicio
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
        
        if (defeatPanel != null)
        {
            defeatPanel.SetActive(false);
        }

        // Buscar el GodzillaController si no está asignado
        if (godzillaController == null)
        {
            godzillaController = FindFirstObjectByType<GodzillaController>();
        }

        if (godzillaController == null)
        {
            Debug.LogError("No se encontró GodzillaController en la escena!");
        }
    }

    /// <summary>
    /// Registra un enemigo en el manager
    /// </summary>
    public void RegisterEnemy(GodzillaEnemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            Debug.Log($"Enemigo registrado: {enemy.gameObject.name}. Total: {enemies.Count}");
        }
    }

    /// <summary>
    /// Se llama cuando un enemigo es destruido
    /// </summary>
    public void OnEnemyDestroyed(GodzillaEnemy enemy)
    {
        Debug.Log($"✅ Enemigo {enemy.gameObject.name} fue destruido!");
    }

    /// <summary>
    /// Activa la secuencia de victoria
    /// </summary>
    public void TriggerVictory()
    {
        if (gameEnded) return;

        gameEnded = true;
        Debug.Log("🎉 ¡VICTORIA! Enemigo eliminado.");

        // Reproducir audio
        PlayVictoryRoar();

        // Mostrar panel de victoria después del audio
        Invoke(nameof(ShowVictoryPanel), 1f);
    }

    /// <summary>
    /// Activa la secuencia de derrota
    /// </summary>
    public void TriggerDefeat()
    {
        if (gameEnded) return;

        gameEnded = true;
        Debug.Log("💀 ¡DERROTA! No lograste eliminar al enemigo.");

        // Reproducir el mismo audio que victoria
        PlayVictoryRoar();

        // Mostrar panel de derrota después del audio
        Invoke(nameof(ShowDefeatPanel), 1f);
    }

    /// <summary>
    /// Se llama desde GodzillaController cuando termina la secuencia de ataque
    /// </summary>
    public void OnAttackSequenceComplete()
    {
        // Este método ya no se usa con el nuevo sistema
        Debug.Log("Método legacy - ya no se usa");
    }

    /// <summary>
    /// Reproduce el audio de rugido de victoria
    /// </summary>
    private void PlayVictoryRoar()
    {
        // Reproducir el rugido directamente con PlayOneShot
        if (victoryRoarAudio != null)
        {
            victoryRoarAudio.PlayOneShoot();
            Debug.Log("Rugido de victoria reproducido!");
        }
        else
        {
            Debug.LogWarning("No se asignó el AudioClipSO de rugido de victoria.");
        }
    }

    /// <summary>
    /// Muestra el panel de victoria
    /// </summary>
    private void ShowVictoryPanel()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            
            if (victoryText != null)
            {
                victoryText.text = "¡VICTORIA!\n¡Has derrotado al enemigo!";
            }
        }
        else
        {
            Debug.LogWarning("No se asignó el panel de victoria.");
        }
    }

    /// <summary>
    /// Muestra el panel de derrota
    /// </summary>
    private void ShowDefeatPanel()
    {
        if (defeatPanel != null)
        {
            defeatPanel.SetActive(true);
            
            if (defeatText != null)
            {
                defeatText.text = "¡DERROTA!\n¡Fallaste el disparo!";
            }
        }
        else
        {
            Debug.LogWarning("No se asignó el panel de derrota.");
        }
    }

    /// <summary>
    /// Reinicia el juego
    /// </summary>
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    /// <summary>
    /// Vuelve al menú principal
    /// </summary>
    public void ReturnToMenu()
    {
        // Cambia "MenuTest" por el nombre de tu escena de menú
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuTest");
    }
}
