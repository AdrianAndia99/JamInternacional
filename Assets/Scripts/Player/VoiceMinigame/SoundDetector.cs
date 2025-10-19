using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SoundDetector : MonoBehaviour
{
    [Header("Micrófono")]
    [SerializeField] private string micName;
    private AudioClip micClip;

    [Header("UI")]
    [SerializeField] private Slider volumeSlider; // Asigna el slider desde el inspector

    [Header("Sensibilidad")]
    [Range(0.01f, 1f)]
    [SerializeField] private float sensitivity; // Qué tan sensible es el medidor

    [Header("Visual")]
    [SerializeField] private Color quietColor = Color.green;
    [SerializeField] private Color loudColor = Color.red;
    [SerializeField] private Image fillImage; // Asigna la parte del slider que se llena

    // Umbrales para definir la intensidad del sonido
    [Header("Umbrales de detección")]
    [SerializeField] private float weakThreshold;   // sonido débil
    [SerializeField] private float strongThreshold; // sonido fuerte

    private string lastState = ""; // Para no spamear logs

    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            micName = Microphone.devices[0];
            micClip = Microphone.Start(micName, true, 1, 44100);
            Debug.Log("Micrófono detectado: " + micName);
        }
        else
        {
            Debug.LogWarning("No se encontró micrófono.");
        }
    }

    void Update()
    {
        if (micClip == null) return;

        // Obtener volumen promedio y ajustar sensibilidad
        float volume = GetAverageVolume() / sensitivity;
        volume = Mathf.Clamp01(volume);

        // Actualizar UI
        if (volumeSlider)
            volumeSlider.value = volume;

        if (fillImage)
            fillImage.color = Color.Lerp(quietColor, loudColor, volume);

        // Detectar tipo de sonido (débil / fuerte)
        string currentState;
        if (volume < weakThreshold)
            currentState = "silencio";
        else if (volume < strongThreshold)
            currentState = "sonido débil";
        else
            currentState = "sonido fuerte";

        // Solo hacer Debug cuando cambia el estado
        if (currentState != lastState)
        {
            Debug.Log($"Nivel actual: {currentState} (volumen: {volume:F3})");
            lastState = currentState;
        }
    }
    public string GetCurrentSoundState()
    {
        float volume = GetAverageVolume() / sensitivity;
        volume = Mathf.Clamp01(volume);

        if (volume < weakThreshold)
            return "silencio";
        else if (volume < strongThreshold)
            return "sonido débil";
        else
            return "sonido fuerte";
    }

    float GetAverageVolume()
    {
        float[] samples = new float[256];
        int micPos = Microphone.GetPosition(micName) - samples.Length;
        if (micPos < 0) return 0;

        micClip.GetData(samples, micPos);
        return samples.Average(x => Mathf.Abs(x));
    }
}
