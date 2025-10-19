using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MemoryGameManager : MonoBehaviour
{
    [Header("Setup")]
    public Transform gridParent;
    public GameObject cardPrefab;
    public List<Sprite> cardSprites;
    public Sprite backSprite;
    public TMP_Text messageText;
    public TMP_Text timerText; // ← nuevo texto para el tiempo

    [Header("Timer Settings")]
    public float gameTime = 60f; // duración total en segundos

    private MemoryCard firstCard;
    private MemoryCard secondCard;
    private int matchesFound = 0;
    private int totalPairs;
    private float timeRemaining;
    private bool gameActive = true;

    void Start()
    {
        GenerateCards();
        StartCoroutine(TimerCountdown());
    }

    void GenerateCards()
    {
        messageText.text = "";
        matchesFound = 0;
        totalPairs = cardSprites.Count;
        timeRemaining = gameTime;
        gameActive = true;

        List<Sprite> gameSprites = new List<Sprite>();
        foreach (Sprite sprite in cardSprites)
        {
            gameSprites.Add(sprite);
            gameSprites.Add(sprite);
        }

        gameSprites = gameSprites.OrderBy(a => Random.value).ToList();

        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Sprite s in gameSprites)
        {
            GameObject newCard = Instantiate(cardPrefab, gridParent);
            MemoryCard card = newCard.GetComponent<MemoryCard>();
            card.SetCard(s, backSprite, this);
            newCard.GetComponent<Button>().onClick.AddListener(card.OnClick);
        }

        messageText.text = "🧙‍♀️ Encuentra todas las parejas antes de que acabe el tiempo...";
    }

    IEnumerator TimerCountdown()
    {
        while (timeRemaining > 0 && gameActive)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
            yield return null;
        }

        if (gameActive && timeRemaining <= 0)
        {
            GameOver();
        }
    }

    void UpdateTimerUI()
    {
        int seconds = Mathf.CeilToInt(timeRemaining);
        timerText.text = $"⏱️ Tiempo: {seconds}s";
    }

    public void CardRevealed(MemoryCard card)
    {
        if (!gameActive) return;

        if (firstCard == null)
        {
            firstCard = card;
        }
        else
        {
            secondCard = card;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.5f);

        if (firstCard.GetFrontSprite() == secondCard.GetFrontSprite())
        {
            firstCard.DisableCard();
            secondCard.DisableCard();
            matchesFound++;
            messageText.text = "✅ ¡Pareja encontrada!";
        }
        else
        {
            messageText.text = "❌ No coinciden...";
            yield return new WaitForSeconds(0.8f);
            firstCard.Flip();
            secondCard.Flip();
        }

        firstCard = null;
        secondCard = null;

        if (matchesFound == totalPairs)
        {
            WinGame();
        }
    }

    void GameOver()
    {
        gameActive = false;
        messageText.text = "💀 ¡Se acabó el tiempo!";
        timerText.text = "⏱️ 0s";

        // Desactiva todas las cartas
        foreach (Transform child in gridParent)
        {
            Button b = child.GetComponent<Button>();
            if (b != null) b.interactable = false;
        }
    }

    void WinGame()
    {
        gameActive = false;
        messageText.text = "🎉 ¡Ganaste! Todas las parejas encontradas a tiempo.";
    }
}
