using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class QuickReactionGame : MonoBehaviour
{
    [Header("UI Elements")]
    public Button ghostButton;
    public TMP_Text scoreText;
    public TMP_Text timerText;

    [Header("Config")]
    public float gameDuration = 15f;      // duración total del minijuego
    public float minAppearTime = 1f;      // tiempo mínimo que aparece
    public float maxAppearTime = 2f;      // tiempo máximo que aparece

    private RectTransform ghostRect;
    private int score = 0;
    private float timer;

    private bool isPlaying = false;

    void Start()
    {
        ghostRect = ghostButton.GetComponent<RectTransform>();
        ghostButton.onClick.AddListener(OnGhostClicked);
        StartGame();
    }

    public void StartGame()
    {
        score = 0;
        timer = gameDuration;
        isPlaying = true;
        UpdateUI();
        StartCoroutine(GhostRoutine());
    }

    void Update()
    {
        if (!isPlaying) return;

        timer -= Time.deltaTime;
        timerText.text = "? " + Mathf.Ceil(timer).ToString();

        if (timer <= 0)
        {
            isPlaying = false;
            StopAllCoroutines();
            EndGame();
        }
    }

    IEnumerator GhostRoutine()
    {
        while (isPlaying)
        {
            // Mover fantasma a una posición aleatoria dentro del Canvas
            float x = Random.Range(-400f, 400f);
            float y = Random.Range(-200f, 200f);
            ghostRect.anchoredPosition = new Vector2(x, y);

            ghostButton.gameObject.SetActive(true);
            yield return new WaitForSeconds(Random.Range(minAppearTime, maxAppearTime));
            ghostButton.gameObject.SetActive(false);

            // Espera breve antes de reaparecer
            yield return new WaitForSeconds(0.5f);
        }
    }

    void OnGhostClicked()
    {
        score += 1;
        ghostButton.gameObject.SetActive(false);
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text = "?? " + score.ToString();
    }

    void EndGame()
    {
        ghostButton.gameObject.SetActive(false);
        timerText.text = "¡Fin!";
        // Aquí puedes llamar al sistema de premios, por ejemplo:
        // GameManager.Instance.AddCandies(score);
    }
}
 
