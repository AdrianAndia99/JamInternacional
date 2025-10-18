using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MemoryGameManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform gridParent;
    public List<Sprite> iconSprites;
    public Text timerText;
    public float showTime = 2f;
    public float gameTime = 30f;

    private List<Card> selectedCards = new List<Card>();
    private List<Card> allCards = new List<Card>();
    private float timeLeft;

    void Start()
    {
        GenerateCards();
        StartCoroutine(StartGame());
    }

    void GenerateCards()
    {
        List<Sprite> pairList = new List<Sprite>();
        for (int i = 0; i < iconSprites.Count; i++)
        {
            pairList.Add(iconSprites[i]);
            pairList.Add(iconSprites[i]);
        }

        // Mezclar
        for (int i = 0; i < pairList.Count; i++)
        {
            Sprite temp = pairList[i];
            int rand = Random.Range(0, pairList.Count);
            pairList[i] = pairList[rand];
            pairList[rand] = temp;
        }

        // Crear cartas
        for (int i = 0; i < pairList.Count; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, gridParent);
            Card card = cardObj.GetComponent<Card>();
            card.Setup(i, pairList[i], this);
            allCards.Add(card);
        }
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(showTime);
        foreach (var card in allCards)
            card.HideCard();

        timeLeft = gameTime;
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerText.text = $"Tiempo: {Mathf.Ceil(timeLeft)}";
            yield return null;
        }

        EndGame(false);
    }

    public void CardSelected(Card card)
    {
        selectedCards.Add(card);

        if (selectedCards.Count == 2)
        {
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.5f);
        if (selectedCards[0].frontImage.sprite == selectedCards[1].frontImage.sprite)
        {
            selectedCards[0].DisableCard();
            selectedCards[1].DisableCard();
            selectedCards.Clear();

            if (AllMatched())
                EndGame(true);
        }
        else
        {
            selectedCards[0].HideCard();
            selectedCards[1].HideCard();
            selectedCards.Clear();
        }
    }

    bool AllMatched()
    {
        foreach (var c in allCards)
        {
            if (c.GetComponent<Button>().interactable)
                return false;
        }
        return true;
    }

    void EndGame(bool win)
    {
        StopAllCoroutines();
        if (win)
            Debug.Log("🎉 ¡Premio doble!");
        else
            Debug.Log("😢 Juego terminado");
    }
}
