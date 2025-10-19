using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class SadakoBlowGame : MonoBehaviour
{
    [Header("Micrófono")]
    [SerializeField] private string micName;
    private AudioClip micClip;

    [Header("UI")]
    [SerializeField] private Slider volumeSlider; // Asigna el slider desde el inspector

    [Header("Sensibilidad")]
    [Range(0.01f, 1f)]
    [SerializeField] private float sensitivity = 0.1f; // Qué tan sensible es el medidor

    [Header("Visual del volumen")]
    [SerializeField] private Color quietColor = Color.green;
    [SerializeField] private Color loudColor = Color.red;
    [SerializeField] private Image fillImage; // Asigna la parte del slider que se llena

    // Umbrales para definir la intensidad del sonido
    [Header("Umbrales de detección")]
    [SerializeField] private float weakThreshold;   // sonido débil
    [SerializeField] private float strongThreshold; // sonido fuerte

    private string lastState = ""; // Para evitar spam en consola

    [Header("Sadako - Referencias")]
    [SerializeField] private List<Transform> hairStrands = new List<Transform>(); // varios mechones
    [SerializeField] private Transform eye;  // Cubo del ojo
    [SerializeField] private Material eyeMaterial; // Material del ojo (usa _BaseColor)
    [SerializeField] private TextMeshProUGUI infoText; // Texto UI en pantalla
    [SerializeField] private Color BadEye;
    [SerializeField] private Color GoodEye;

    [Header("Sadako - Lógica de juego")]
    [SerializeField] private float minEyeChangeInterval;
    [SerializeField] private float maxEyeChangeInterval;

    [Header("Audios")]
    [SerializeField] private AudioClipSO Grito;
    [SerializeField] private AudioClipSO risa;
    [SerializeField] private AudioClipSO soplido;

    private float currentInterval;
    private bool eyeHappy = false;
    private float eyeTimer = 0f;

    private int nextHairIndex = 0; // controla qué mechón se mueve
    public bool hasBlownThisCycle = false; // evita múltiples mechones por soplido


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
        SetRandomInterval();
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
        if (eyeTimer >= currentInterval)
        {
            eyeTimer = 0;
            SetEyeState(!eyeHappy);
            SetRandomInterval();
            // Reinicia posibilidad de soplar cuando cambia el estado del ojo
            hasBlownThisCycle = false;
        }

        //Detección del soplido fuerte
        if (volume > strongThreshold && !hasBlownThisCycle)
        {
            hasBlownThisCycle = true;

            soplido.PlayOneShoot();

            if (eyeHappy)
            {
                if (infoText)
                    infoText.text = "¡Buen soplido! El cabello se aparta...";
                BlowNextHair();
            }
            else
            {
                if (infoText)
                    infoText.text = "¡Soplaste en mal momento! Has perdido.";
                Grito.PlayOneShoot();
                ResetAllHair();
                SetEyeState(false);
            }

            //Permitir volver a soplar cuando el sonido baja
            if (volume < weakThreshold)
            {
                hasBlownThisCycle = false;
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
            eyeMaterial.SetColor("_BaseColor", happy ? GoodEye : BadEye);
        }
        else if (eye)
        {
            var renderer = eye.GetComponent<Renderer>();
            if (renderer != null && renderer.material.HasProperty("_BaseColor"))
                renderer.material.SetColor("_BaseColor", happy ? GoodEye : BadEye);
        }

        if (happy)
            risa.PlayOneShoot();
    }

    void SetRandomInterval()
    {
        currentInterval = Random.Range(minEyeChangeInterval, maxEyeChangeInterval);
        Debug.Log($"Nuevo intervalo del ojo: {currentInterval:F2}s");
    }

    void BlowNextHair()
    {
        if (hairStrands.Count == 0) return;


        // Si ya se apartaron todos los mechones
        if (nextHairIndex >= hairStrands.Count)
        {
            if (infoText)
                infoText.text = "¡Revelaste el rostro de Sadako!";
            return;
        }

        Transform hair = hairStrands[nextHairIndex];
        nextHairIndex++;

        //Mover mechón de forma natural, sin volver atrás
        hair.DOKill();
        Vector3 startPos = hair.localPosition;
        Vector3 blowPos = startPos + new Vector3(0.3f, 0, Random.Range(-0.05f, 0.05f));

        hair.DOLocalMove(blowPos, 0.4f)
            .SetEase(Ease.OutQuad)
            .OnStart(() =>
            {
                // Pequeño efecto de “temblor” inicial
                hair.DOPunchPosition(new Vector3(0.1f, 0, 0), 0.3f, 5, 0.8f);
            });

        // Si ya es el último mechón, mensaje final
        if (nextHairIndex == hairStrands.Count)
        {
            if (infoText)
                infoText.text = "¡Revelaste completamente el rostro de Sadako!";
        }
    }

    void ResetAllHair()
    {
        foreach (Transform h in hairStrands)
        {
            h.DOKill();
            h.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.InOutQuad);
        }
        nextHairIndex = 0;
    }
}
