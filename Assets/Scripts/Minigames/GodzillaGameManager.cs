using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GodzillaGameManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("Panel que se muestra al ganar")]
    [SerializeField] private GameObject victoryPanel;
    
    [Tooltip("Texto del panel de victoria")]
    [SerializeField] private TextMeshProUGUI victoryText;

    [Header("Audio")]
    [Tooltip("AudioClipSO del rugido de victoria")]
    [SerializeField] private AudioClipSO victoryRoarAudio;

    [Header("Referencias")]
    [Tooltip("Referencia al controlador de Godzilla")]
    [SerializeField] private GodzillaController godzillaController;

    // Estado del juego
    private List<GodzillaEnemy> enemies = new List<GodzillaEnemy>();
    private bool gameEnded = false;
    private bool enemyDestroyedDuringAttack = false;

    private void Start()
    {
        // Ocultar panel de victoria al inicio
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
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
        Debug.Log($"Enemigo {enemy.gameObject.name} fue destruido!");
        
        // Marcar que un enemigo fue destruido durante el ataque
        if (godzillaController != null && godzillaController.IsAttacking)
        {
            enemyDestroyedDuringAttack = true;
            Debug.Log("¡Enemigo destruido durante el ataque!");
        }
    }

    /// <summary>
    /// Se llama desde GodzillaController cuando termina la secuencia de ataque
    /// </summary>
    public void OnAttackSequenceComplete()
    {
        if (gameEnded) return;

        Debug.Log($"Secuencia de ataque completada. Enemigo destruido: {enemyDestroyedDuringAttack}");

        // Si se destruyó un enemigo durante el ataque
        if (enemyDestroyedDuringAttack)
        {
            // Verificar si todos los enemigos están destruidos
            bool allEnemiesDestroyed = true;
            foreach (GodzillaEnemy enemy in enemies)
            {
                if (enemy != null && !enemy.IsDestroyed)
                {
                    allEnemiesDestroyed = false;
                    break;
                }
            }

            if (allEnemiesDestroyed)
            {
                // ¡VICTORIA!
                TriggerVictory();
            }
            
            // Resetear flag
            enemyDestroyedDuringAttack = false;
        }
    }

    /// <summary>
    /// Activa la secuencia de victoria
    /// </summary>
    private void TriggerVictory()
    {
        if (gameEnded) return;

        gameEnded = true;
        Debug.Log("¡VICTORIA! Todos los enemigos destruidos.");

        // Reproducir rugido de victoria
        PlayVictoryRoar();

        // Mostrar panel de victoria después del rugido
        Invoke(nameof(ShowVictoryPanel), 1f);
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
