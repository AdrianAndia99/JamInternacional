using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class SadakoBlowGame : MonoBehaviour
{
    [Header("Micr�fono")]
    [SerializeField] private string micName;
    private AudioClip micClip;

    [Header("UI")]
    [SerializeField] private Slider volumeSlider;

    [Header("Sensibilidad")]
    [Range(0.01f, 1f)]
    [SerializeField] private float sensitivity = 0.1f;

    [Header("Visual del volumen")]
    [SerializeField] private Color quietColor = Color.green;
    [SerializeField] private Color loudColor = Color.red;
    [SerializeField] private Image fillImage;

    [Header("Umbrales de detecci�n")]
    [SerializeField] private float weakThreshold;
    [SerializeField] private float strongThreshold;

    [Header("Sadako - Referencias")]
    [SerializeField] private List<Transform> hairStrands = new List<Transform>();
    [SerializeField] private Transform eye;
    [SerializeField] private Material eyeMaterial;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Color BadEye;
    [SerializeField] private Color GoodEye;

    [Header("Sadako - L�gica de juego")]
    [SerializeField] private float minEyeChangeInterval;
    [SerializeField] private float maxEyeChangeInterval;

    [Header("Audios")]
    [SerializeField] private AudioClipSO Grito;
    [SerializeField] private AudioClipSO risa;
    [SerializeField] private AudioClipSO soplido;

    private float currentInterval;
    private bool eyeHappy = false;
    private float eyeTimer = 0f;

    private int nextHairIndex = 0;
    private bool hasBlownThisCycle = false;
    private bool gameOver = false;

    private Dictionary<Transform, Vector3> originalHairPositions = new Dictionary<Transform, Vector3>();


    //--- CICLO DE VIDA PREFAB ---
    void OnEnable()
    {
        nextHairIndex = 0;
        hasBlownThisCycle = false;
        eyeTimer = 0;
        gameOver = false;

        SetRandomInterval();
        SetEyeState(false);

        originalHairPositions.Clear();
        foreach (var h in hairStrands)
        {
            if (h != null)
            {
                originalHairPositions[h] = h.localPosition;
                h.gameObject.SetActive(true);
            }
        }

        // Reiniciar micr�fono
        if (Microphone.devices.Length > 0)
        {
            micName = Microphone.devices[0];
            micClip = Microphone.Start(micName, true, 1, 44100);
            Debug.Log("Micr�fono detectado: " + micName);
        }
        else
        {
            Debug.LogWarning("No se encontr� micr�fono.");
        }

        if (infoText)
            infoText.text = "Espera... sopla cuando el ojo est� FELIZ";
    }

    void OnDisable()
    {
        if (!string.IsNullOrEmpty(micName) && Microphone.IsRecording(micName))
            Microphone.End(micName);

        foreach (Transform h in hairStrands)
            if (h) h.DOKill();
    }

    // --- LOOP ---
    void Update()
    {
        if (micClip == null || gameOver) return;

        float volume = GetAverageVolume() / sensitivity;
        volume = Mathf.Clamp01(volume);

        if (volumeSlider)
            volumeSlider.value = volume;
        if (fillImage)
            fillImage.color = Color.Lerp(quietColor, loudColor, volume);

        // Cambiar estado del ojo peri�dicamente
        eyeTimer += Time.deltaTime;
        if (eyeTimer >= currentInterval)
        {
            eyeTimer = 0;
            SetEyeState(!eyeHappy);
            SetRandomInterval();
            hasBlownThisCycle = false;
        }

        // Detecci�n de soplido fuerte
        if (volume > strongThreshold && !hasBlownThisCycle)
        {
            hasBlownThisCycle = true;
            soplido.PlayOneShoot();

            if (eyeHappy)
            {
                if (infoText)
                    infoText.text = "�Buen soplido! El cabello se aparta...";
                BlowNextHair();
            }
            else
            {
                LoseGame(); //pierde cuando sopla con ojo rojo
                return;
            }
        }

        if (volume < weakThreshold)
        {
            hasBlownThisCycle = false;
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

    // --- VISUAL DEL OJO ---
    void SetEyeState(bool happy)
    {
        eyeHappy = happy;

        if (eyeMaterial)
            eyeMaterial.SetColor("_BaseColor", happy ? GoodEye : BadEye);
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
    }

    // --- MOVIMIENTO DEL CABELLO ---
    void BlowNextHair()
    {
        if (nextHairIndex >= hairStrands.Count || gameOver) return;

        Transform hair = hairStrands[nextHairIndex];
        nextHairIndex++;

        if (hair == null) return;
        hair.DOKill();

        Vector3 startPos = hair.localPosition;
        Vector3 blowDir = new Vector3(0, 0, -1f);
        Vector3 blowPos = startPos + blowDir * Random.Range(0.4f, 0.7f);

        hair.DOLocalMove(blowPos, 0.5f)
            .SetEase(Ease.OutQuad)
            .OnStart(() =>
            {
                hair.DOPunchPosition(new Vector3(0.1f, 0, 0), 0.3f, 5, 0.8f);
            })
            .OnComplete(() =>
            {
                hair.gameObject.SetActive(false);

                // Si ya no quedan m�s mechones
                if (nextHairIndex >= hairStrands.Count)
                {
                    WinGame();
                }
            });
    }

    void ResetAllHair()
    {
        foreach (Transform h in hairStrands)
        {
            if (h == null) continue;

            h.DOKill();
            h.gameObject.SetActive(true);

            if (originalHairPositions.ContainsKey(h))
                h.localPosition = originalHairPositions[h];
        }

        nextHairIndex = 0;
    }

    // --- RESULTADOS ---
    void WinGame()
    {
        gameOver = true;
        if (infoText)
            infoText.text = "�Revelaste completamente el rostro de Sadako!";
        Grito.PlayOneShoot();
        SetEyeState(false);
    }

    void LoseGame()
    {
        gameOver = true;
        if (infoText)
            infoText.text = "�Soplaste en mal momento! Has perdido.";
        SetEyeState(false);
        ResetAllHair();
    }
}
