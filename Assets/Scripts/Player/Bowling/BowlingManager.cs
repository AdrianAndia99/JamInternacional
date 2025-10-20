using UnityEngine;
using System.Collections;
public class BowlingManager : MonoBehaviour
{
    public PinController[] pins;
    public JackOBowlingController jack; // referencia al lanzador

    [Header("Configuración")]
    public float checkDelay; // tiempo para esperar a que los pinos terminen de moverse
    [SerializeField] private AudioClipSO StrikeSound;

    private bool checking = false;

    public void OnBallCollision()
    {
        if (!checking)
            StartCoroutine(CheckPinsAfterDelay());
    }

    private IEnumerator CheckPinsAfterDelay()
    {
        checking = true;
        yield return new WaitForSeconds(checkDelay);

        int fallenPins = 0;
        foreach (var pin in pins)
        {
            if (pin.IsDown())
                fallenPins++;
        }
        // Determinar resultado
        if (fallenPins == pins.Length)
        {
            StrikeSound.PlayOneShoot();
            Debug.Log("STRIKE!!! Todos los pinos caídos");
        }
        else if (fallenPins >= pins.Length / 2)
        {
            Debug.Log("Buena tirada: " + fallenPins + " pinos caídos");
        }
        else if (fallenPins > 0)
        {
            Debug.Log("Derribó " + fallenPins + " pinos");
        }
        else
        {
            Debug.Log("No derribó ninguno");
        }
        
        // Actualizar UI
        jack.UpdateScoreUI(fallenPins);

        // Reiniciar pinos y bola
        yield return new WaitForSeconds(2f);

        foreach (var pin in pins)
        {
            pin.ResetPin();
        }

        jack.ResetAttemptFromManager();

        checking = false;
    }
    public void ResetAllPins()
    {
        foreach (var pin in pins)
        {
            if (pin != null)
                pin.ResetPin();
        }

        Debug.Log("Todos los pinos fueron reiniciados.");
    }
}
