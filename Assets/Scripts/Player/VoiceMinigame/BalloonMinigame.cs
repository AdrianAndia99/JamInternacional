using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BalloonMinigame : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SoundDetector soundDetector;
    [SerializeField] private Image balloonImage;          // Imagen del globo (UI)
    [SerializeField] private Sprite[] balloonStages;      // Sprites desde vacío hasta explotar
    [SerializeField] private Animator clownAnimator;      // Animator del payaso
    [SerializeField] private TextMeshProUGUI timerText;   // Texto del tiempo restante

    [Header("Audios")]
    [SerializeField] private AudioClipSO inflatingClip;   // Sonido mientras infla
    [SerializeField] private AudioClipSO explodeClip;     // Sonido al explotar

    [Header("Configuración del globo")]
    [SerializeField] private float inflateSpeed = 1f;     // Qué tan rápido sube el nivel (por segundo)
    [SerializeField] private float deflateSpeed = 0.5f;   // Qué tan rápido baja el nivel
    [SerializeField] private float winTime = 5f;          // Tiempo en mantener nivel medio

    private float currentLevel = 0f;  // Nivel de inflado (0 = vacío, balloonStages.Length-1 = explotar)
    private float winTimer = 0f;
    private bool gameOver = false;

    private bool isInflatingSoundPlaying = false;

    private void OnEnable()
    {
        ResetGame();
    }

    private void Update()
    {
        if (soundDetector == null) return;

        if (gameOver)
        {
            UpdateTimerUI(); // mantener actualizado el texto en caso de que el juego haya terminado
            return;
        }
        string soundState = soundDetector.GetCurrentSoundState();

        // Ajustar nivel del globo según el sonido
        switch (soundState)
        {
            case "silencio":
                currentLevel -= deflateSpeed * Time.deltaTime;
                clownAnimator.Play("Quieto");
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

        UpdateTimerUI();
    }

    private void UpdateBalloonSprite()
    {
        int spriteIndex = Mathf.RoundToInt(currentLevel);
        spriteIndex = Mathf.Clamp(spriteIndex, 0, balloonStages.Length - 1);
        balloonImage.sprite = balloonStages[spriteIndex];
    }

    private void PlayInflatingSound()
    {
        if (inflatingClip && !isInflatingSoundPlaying)
        {
            inflatingClip.PlayLoop();
            isInflatingSoundPlaying = true;
        }
    }

    private void StopInflatingSound()
    {
        if (isInflatingSoundPlaying)
        {
            inflatingClip.StopPlay();
            isInflatingSoundPlaying = false;
        }
    }

    private void Explode()
    {
        gameOver = true;
        clownAnimator.Play("Despedirse");

        if (explodeClip)
            explodeClip.PlayOneShoot();

        // Usa el último sprite como globo explotado
        balloonImage.sprite = balloonStages[balloonStages.Length - 1];
        Debug.Log("El globo explotó. Has perdido.");
        UpdateTimerUI(true);
    }

    private void Win()
    {
        gameOver = true;
        clownAnimator.Play("Despedirse");
        Debug.Log("Mantuvistes el globo en equilibrio. ¡Ganaste!");
        UpdateTimerUI(true);
    }

    private void UpdateTimerUI(bool hide = false)
    {
        if (timerText == null) return;

        if (hide)
        {
            timerText.text = "";
            return;
        }

        float remaining = Mathf.Clamp(winTime - winTimer, 0, winTime);
        timerText.text = $"Tiempo restante: {remaining:F1}s";
    }

    private void ResetGame()
    {
        gameOver = false;
        currentLevel = 0f;
        winTimer = 0f;
        isInflatingSoundPlaying = false;

        if (balloonImage && balloonStages.Length > 0)
            balloonImage.sprite = balloonStages[0];

        UpdateTimerUI();
    }
}
