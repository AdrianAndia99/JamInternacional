using UnityEngine;
using UnityEngine.Events;

public abstract class MinigameManager : MonoBehaviour
{
    [Header("Configuración del Minijuego")]
    [Tooltip("El prefab del minijuego que se instanciará")]
    public GameObject MiniGamePrefab;

    [Header("Eventos")]
    [Tooltip("Se ejecuta cuando el minijuego inicia")]
    public UnityEvent OnStart;

    [Tooltip("Se ejecuta cuando el minijuego termina")]
    public UnityEvent OnFinish;

    /// <summary>
    /// Método abstracto que cada minijuego debe implementar para su lógica de inicio
    /// </summary>
    protected abstract void OnStartGame();

    /// <summary>
    /// Método abstracto que cada minijuego debe implementar para su lógica de finalización
    /// </summary>
    protected abstract void OnFinishGame();
}
