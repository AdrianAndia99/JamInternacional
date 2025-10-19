using UnityEngine;
using TMPro;

public class CandyCatchManager : MonoBehaviour
{
    public static CandyCatchManager Instance;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text gameOverText;

    [Header("Game Settings")]
    public float gameDuration = 30f;

    private int score = 0;
    private float timeRemaining;
    private bool isGameOver = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timeRemaining = gameDuration;
        gameOverText.gameObject.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        if (isGameOver) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            EndGame();
        }

        UpdateUI();
    }

    public void AddScore(int amount)
    {
        if (!isGameOver)
        {
            score += amount;
            UpdateUI();
        }
    }

    public void EndGame()
    {
        isGameOver = true;
        gameOverText.gameObject.SetActive(true);
        gameOverText.text = "¡Juego Terminado!";
    }

    void UpdateUI()
    {
        scoreText.text = "Puntaje: " + score;
        timerText.text = "Tiempo: " + Mathf.CeilToInt(timeRemaining);
    }
}

