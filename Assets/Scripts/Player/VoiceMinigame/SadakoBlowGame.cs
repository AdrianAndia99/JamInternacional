using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SadakoBlowGame : MonoBehaviour
{
    [Header("Micrófono")]
    public string micName;
    private AudioClip micClip;

    [Header("UI")]
    public Slider volumeSlider; // Asigna el slider desde el inspector

    [Header("Sensibilidad")]
    [Range(0.01f, 1f)]
    public float sensitivity = 0.1f; // Qué tan sensible es el medidor

    [Header("Visual del volumen")]
    public Color quietColor = Color.green;
    public Color loudColor = Color.red;
    public Image fillImage; // Asigna la parte del slider que se llena

    // Umbrales para definir la intensidad del sonido
    [Header("Umbrales de detección")]
    public float weakThreshold = 0.1f;   // sonido débil
    public float strongThreshold = 0.3f; // sonido fuerte

    private string lastState = ""; // Para evitar spam en consola

    [Header("Sadako - Referencias")]
    public Transform hair; // Cubo del cabello
    public Transform eye;  // Cubo del ojo
    public Material eyeMaterial; // Material del ojo (usa _BaseColor)
    public TextMeshProUGUI infoText; // Texto UI en pantalla

    [Header("Sadako - Lógica de juego")]
    public float eyeChangeInterval = 3f; // Cada cuánto cambia el estado del ojo
    private bool eyeHappy = false;
    private float eyeTimer = 0f;

    void Start()
    {
        // Iniciar micrófono
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

        // Estado inicial del ojo
        SetEyeState(false);
        if (infoText)
            infoText.text = "Espera... sopla cuando el ojo esté FELIZ ";
    }

    void Update()
    {
        if (micClip == null) return;

        // Obtener nivel de sonido
        float volume = GetAverageVolume() / sensitivity;
        volume = Mathf.Clamp01(volume);

        // Actualizar UI del micrófono
        if (volumeSlider)
            volumeSlider.value = volume;

        if (fillImage)
            fillImage.color = Color.Lerp(quietColor, loudColor, volume);

        ////Detectar tipo de sonido
        //string currentState;
        //if (volume < weakThreshold)
        //    currentState = "silencio";
        //else if (volume < strongThreshold)
        //    currentState = "sonido débil";
        //else
        //    currentState = "sonido fuerte";

        //if (currentState != lastState)
        //{
        //    Debug.Log($"Nivel actual: {currentState} (volumen: {volume:F3})");
        //    lastState = currentState;
        //}

        //Cambiar estado del ojo periódicamente
        eyeTimer += Time.deltaTime;
        if (eyeTimer >= eyeChangeInterval)
        {
            eyeTimer = 0;
            SetEyeState(!eyeHappy);
        }

        //Detección del soplido fuerte
        if (volume > strongThreshold)
        {
            if (eyeHappy)
            {
                if (infoText)
                    infoText.text = "¡Buen soplido! El cabello se aparta...";
                MoveHair();
            }
            else
            {
                if (infoText)
                    infoText.text = "¡Soplaste en mal momento! Has perdido.";
                ResetHair();
                SetEyeState(false);
            }
        }
    }

    float GetAverageVolume()
    {
        float[] samples = new float[256];
        int micPos = Microphone.GetPosition(micName) - samples.Length;
        if (micPos < 0) return 0;

        micClip.GetData(samples, micPos);
        return samples.Average(x => Mathf.Abs(x));
    }

    //Cambia el estado del ojo
    void SetEyeState(bool happy)
    {
        eyeHappy = happy;
        if (eyeMaterial)
        {
            eyeMaterial.SetColor("_BaseColor", happy ? Color.green : Color.red);
        }
        else if (eye)
        {
            var renderer = eye.GetComponent<Renderer>();
            if (renderer != null && renderer.material.HasProperty("_BaseColor"))
                renderer.material.SetColor("_BaseColor", happy ? Color.green : Color.red);
        }
    }

    //Mover el cabello hacia un lado
    void MoveHair()
    {
        if (hair)
            hair.localPosition += new Vector3(0.05f, 0, 0);
    }

    //Reiniciar cabello
    void ResetHair()
    {
        if (hair)
            hair.localPosition = new Vector3(0, 0, 0);
    }
}
