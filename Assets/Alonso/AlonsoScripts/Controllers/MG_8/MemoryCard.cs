using UnityEngine;
using UnityEngine.UI;

public class MemoryCard : MonoBehaviour
{
    public Image frontImage;
    public Image backImage;
    private MemoryGameManager manager;
    private Sprite frontSprite;
    private bool isRevealed = false;
    private bool isMatched = false;

    public void SetupCard(MemoryGameManager gameManager, Sprite sprite)
    {
        manager = gameManager;
        frontSprite = sprite;
        frontImage.sprite = frontSprite;
        FlipBack();
    }

    public void OnCardClicked()
    {
        if (isMatched || isRevealed) return;
        Flip();
        manager.CardSelected(this);
    }

    public void Flip()
    {
        isRevealed = true;
        frontImage.gameObject.SetActive(true);
        backImage.gameObject.SetActive(false);
    }

    public void FlipBack()
    {
        isRevealed = false;
        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);
    }

    public void HideCard()
    {
        isMatched = true;
        gameObject.SetActive(false);
    }

    public Sprite GetSprite()
    {
        return frontSprite;
    }
}



