using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score;

    public bool ConsiderMaxScore = true;
    public int maxScoreToWin;

    [Header("UI")]
    public TextMeshProUGUI scoreText;

    [Header("Events")]
    public UnityEvent<int> OnScoreChanged;
    public UnityEvent OnWin;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateScoreText();
    }

    public void AddScore(int amount)
    {
        int tmpScore = Score;
        tmpScore += amount;
        if (tmpScore < 0) return;

        Score += amount;
        OnScoreChanged.Invoke(Score);
        UpdateScoreText();

        if (ConsiderMaxScore && Score >= maxScoreToWin)
            OnWin.Invoke();
    }

    public void ResetScore()
    {
        Score = 0;
        OnScoreChanged.Invoke(Score);
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{Score.ToString("00")}/{maxScoreToWin.ToString("00")}";
        }
    }
}