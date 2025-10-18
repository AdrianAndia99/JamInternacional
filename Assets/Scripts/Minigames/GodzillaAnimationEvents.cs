using UnityEngine;

/// <summary>
/// Helper para configurar eventos de animación de Godzilla
/// Agrega este script al mismo GameObject que tiene el Animator
/// </summary>
public class GodzillaAnimationEvents : MonoBehaviour
{
    private GodzillaController controller;

    private void Awake()
    {
        controller = GetComponentInParent<GodzillaController>();
        
        if (controller == null)
        {
            controller = GetComponent<GodzillaController>();
        }

        if (controller == null)
        {
            Debug.LogError("No se encontró GodzillaController!");
        }
    }

    /// <summary>
    /// Llamado desde un Animation Event cuando debe disparar el láser
    /// Agrega este evento en el frame exacto de la 3ra animación donde quieres el disparo
    /// </summary>
    public void OnShootLaser()
    {
        Debug.Log("Animation Event: OnShootLaser called");
        
        if (controller != null)
        {
            // El controller detectará automáticamente cuando entrar al estado de disparo
            // Este método es solo un backup por si quieres control manual
        }
    }

    /// <summary>
    /// Llamado cuando la secuencia de ataque termina completamente
    /// </summary>
    public void OnAttackSequenceComplete()
    {
        Debug.Log("Animation Event: Attack sequence completed");
    }

    /// <summary>
    /// Llamado cuando Godzilla empieza a cargar el ataque
    /// Puedes usar esto para efectos de sonido o partículas
    /// </summary>
    public void OnStartCharge()
    {
        Debug.Log("Animation Event: Started charging attack");
    }
}
