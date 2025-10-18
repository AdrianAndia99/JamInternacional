using UnityEngine;
using System.Collections;
public class BowlingManager : MonoBehaviour
{
    public PinController[] pins;
    public JackOBowlingController jack; // referencia al lanzador

    [Header("Configuraci�n")]
    public float checkDelay = 3f; // tiempo para esperar a que los pinos terminen de moverse

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
            Debug.Log("STRIKE!!! Todos los pinos ca�dos");
        }
        else if (fallenPins >= pins.Length / 2)
        {
            Debug.Log("Buena tirada: " + fallenPins + " pinos ca�dos");
        }
        else if (fallenPins > 0)
        {
            Debug.Log("Derrib� " + fallenPins + " pinos");
        }
        else
        {
            Debug.Log("No derrib� ninguno");
        }

        // Reiniciar pinos y bola
        yield return new WaitForSeconds(2f);

        foreach (var pin in pins)
        {
            pin.ResetPin();
        }

        jack.ResetAttemptFromManager();

        checking = false;
    }
}
