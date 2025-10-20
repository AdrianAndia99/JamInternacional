using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class BalloonMinigame : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SoundDetector soundDetector;
    [SerializeField] private Image balloonImage;         // Imagen del globo (UI)
    [SerializeField] private Sprite[] balloonStages;     // Sprites desde vacío inflado explota
    [SerializeField] private Animator clownAnimator;     // Animator del payaso
    
    [Header("Audios")]
    [SerializeField] private AudioClipSO inflatingClip;    // Sonido mientras infla
    [SerializeField] private AudioClipSO explodeClip;      // Sonido al explotar

    [Header("Configuración del globo")]
    [SerializeField] private float inflateSpeed = 1f;    // Qué tan rápido sube el nivel (por segundo)
    [SerializeField] private float deflateSpeed = 0.5f;  // Qué tan rápido baja el nivel
    [SerializeField] private float winTime = 5f;         // Tiempo en mantener nivel medio

    private float currentLevel = 0f;  // Nivel de inflado (0 = vacío, balloonStages.Length-1 = explotar)
    private float winTimer = 0f;
    private bool gameOver = false;

    private bool isInflatingSoundPlaying = false;

    void Update()
    {
        if (gameOver || soundDetector == null) return;

        string soundState = soundDetector.GetCurrentSoundState();

        // Ajustar nivel del globo según el sonido
        switch (soundState)
        {
            case "silencio":
                currentLevel -= deflateSpeed * Time.deltaTime;
                clownAnimator.Play("Idle");
                StopInflatingSound();
                break;

            case "sonido débil":
                currentLevel += inflateSpeed * Time.deltaTime;
                clownAnimator.Play("Reir");
                PlayInflatingSound();
                break;

            case "sonido fuerte":
                StopInflatingSound();
                Explode();
                return;
        }

        // Limitar rango y actualizar sprite
        currentLevel = Mathf.Clamp(currentLevel, 0f, balloonStages.Length - 1);
        UpdateBalloonSprite();

        // Comprobar victoria (si el globo se mantiene en rango medio)
        float midMin = (balloonStages.Length - 1) * 0.4f;
        float midMax = (balloonStages.Length - 1) * 0.6f;

        if (currentLevel >= midMin && currentLevel <= midMax)
        {
            winTimer += Time.deltaTime;
            if (winTimer >= winTime)
            {
                Win();
            }
        }
        else
        {
            winTimer = Mathf.Max(0, winTimer - Time.deltaTime * 0.5f);
        }
    }

    void UpdateBalloonSprite()
    {
        int spriteIndex = Mathf.RoundToInt(currentLevel);
        spriteIndex = Mathf.Clamp(spriteIndex, 0, balloonStages.Length - 1);
        balloonImage.sprite = balloonStages[spriteIndex];
    }
    void PlayInflatingSound()
    {
        if (inflatingClip && !isInflatingSoundPlaying)
        {
            inflatingClip.PlayLoop();
            isInflatingSoundPlaying = true;
        }
    }

    void StopInflatingSound()
    {
        if (isInflatingSoundPlaying)
        {
            inflatingClip.StopPlay();
            isInflatingSoundPlaying = false;
        }
    }
    void Explode()
    {
        gameOver = true;
        clownAnimator.Play("Despedirse");

        if (explodeClip)
            explodeClip.PlayOneShoot();
        // Usa el último sprite como globo explotado si lo tienes
        balloonImage.sprite = balloonStages[balloonStages.Length - 1];
        Debug.Log("El globo explotó. Has perdido.");
    }

    void Win()
    {
        gameOver = true;
        clownAnimator.Play("Despedirse");
        Debug.Log("Mantuvistes el globo en equilibrio. ¡Ganaste!");
    }
}
