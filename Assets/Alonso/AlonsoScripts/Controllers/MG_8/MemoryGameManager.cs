using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MemoryGameManager : MonoBehaviour
{
    public MemoryCard cardPrefab;
    public Sprite[] cardFrontSprites;
    public Transform gridParent;
    public TMP_Text messageText;
    public TMP_Text timerText;
    public float revealDelay = 1f;
    public float gameTime = 60f;

    private List<MemoryCard> cards = new List<MemoryCard>();
    private MemoryCard firstCard, secondCard;
    private float timer;
    private bool gameActive = false;
    private bool isChecking = false; // 🔒 bloquea clicks mientras se comparan cartas

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        timer = gameTime;
        messageText.text = "";
        CreateCards();
        gameActive = true;
    }

    void Update()
    {
        if (!gameActive) return;

        timer -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.CeilToInt(timer);

        if (timer <= 0)
        {
            gameActive = false;
            messageText.text = "Time’s up!";
        }
    }

    void CreateCards()
    {
        // Elimina cartas previas si hay
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        cards.Clear();

        // Duplica los sprites para formar pares
        List<Sprite> spriteList = new List<Sprite>();
        for (int i = 0; i < cardFrontSprites.Length; i++)
        {
            spriteList.Add(cardFrontSprites[i]);
            spriteList.Add(cardFrontSprites[i]);
        }

        // Baraja las cartas
        for (int i = 0; i < spriteList.Count; i++)
        {
            Sprite temp = spriteList[i];
            int rand = Random.Range(i, spriteList.Count);
            spriteList[i] = spriteList[rand];
            spriteList[rand] = temp;
        }

        // Crea las cartas en el grid
        foreach (Sprite sprite in spriteList)
        {
            MemoryCard newCard = Instantiate(cardPrefab, gridParent);
            newCard.SetupCard(this, sprite);
            cards.Add(newCard);
        }
    }

    public void CardSelected(MemoryCard card)
    {
        if (isChecking) return; // ❌ si está comparando, no hacer nada
        if (firstCard == null)
        {
            firstCard = card;
        }
        else if (secondCard == null && card != firstCard)
        {
            secondCard = card;
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        isChecking = true; // 🔒 bloquea más clics

        yield return new WaitForSeconds(revealDelay);

        if (firstCard.GetSprite() == secondCard.GetSprite())
        {
            firstCard.HideCard();
            secondCard.HideCard();
            messageText.text = "Match!";
        }
        else
        {
            firstCard.FlipBack();
            secondCard.FlipBack();
            messageText.text = "Try again!";
        }

        firstCard = null;
        secondCard = null;

        isChecking = false; // 🔓 vuelve a permitir clics
    }
}
