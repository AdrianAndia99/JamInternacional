using TMPro;
using UnityEngine;
using System.Collections;

public class TeleDisplayController : MonoBehaviour
{
    [Header("Referencias de Teles")]
    public TextMeshProUGUI teleKeyboardText;
    public TextMeshProUGUI teleMouseText;
    public TextMeshProUGUI TimerText;

    [Header("Mensaje Inicial (Canvas de la Bola)")]
    public TextMeshProUGUI startMessageText;

    [Header("Duración del mensaje inicial")]
    public float messageDuration = 4f;

    private bool firstThrowDone = false;

    private void Start()
    {
        // Mostrar controles al inicio
        ShowInitialInstructions();

        // Mostrar mensaje inicial en canvas de la bola
        if (startMessageText != null)
        {
            startMessageText.text = "Tienes solo 2 intentos";
            StartCoroutine(HideStartMessageAfterDelay());
        }
    }

    private IEnumerator HideStartMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);
        startMessageText.gameObject.SetActive(false);
    }

    private void ShowInitialInstructions()
    {
        if (teleKeyboardText != null)
            teleKeyboardText.fontSize = 8;
            teleKeyboardText.text =
                "Controles Teclado:\nflecha Iz, Dr Mover calabaza\n flecha Abajo Fijar posición\nflecha arriba Lanzar";

        if (teleMouseText != null)
            teleMouseText.text =
                "Controles Mouse:\nMover cámara\nClick = Zoom";
    }

    // Llamar a esto cuando el jugador haga su primer tiro
    public void OnFirstThrow()
    {
        firstThrowDone = true;
    }

    // Llamar después de cada tiro, pasando los resultados
    public void UpdateTeleScores(int knockedPins, int remainingTries, bool strike)
    {
        if (!firstThrowDone) return;

        if (strike)
        {
            teleKeyboardText.text = "¡Strike!";
            teleMouseText.text = "¡Todos los pinos derribados!";
        }
        else
        {
            teleKeyboardText.fontSize = 12;
            teleKeyboardText.alignment = TextAlignmentOptions.Midline;
            teleMouseText.alignment = TextAlignmentOptions.Midline;
            teleKeyboardText.text = $"Pinos derribados: {knockedPins}";
            teleMouseText.text = $"Intentos restantes: {remainingTries}";

        }
    }
    public void UpdateTimerUI(float time)
    {
        TimerText.text = $"Tiempo: {time:F1}s";
    }

    public void ShowGameOver()
    {
        teleKeyboardText.text = "¡Fin del juego!";
        teleMouseText.text = "No lograste el strike";
    }
}
