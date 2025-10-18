using UnityEngine;
using UnityEngine.UI;

public class MemoryCard : MonoBehaviour
{
    public Sprite frontSprite;  // imagen del frente
    public Sprite backSprite;   // imagen del reverso

    private Image cardImage;
    private bool isFlipped = false;
    private MemoryGameManager gameManager;

    public void SetCard(Sprite front, Sprite back, MemoryGameManager manager)
    {
        frontSprite = front;
        backSprite = back;
        gameManager = manager;

        cardImage = GetComponent<Image>();
        cardImage.sprite = backSprite; // comienza volteada
    }

    public void OnClick()
    {
        // evita reactivar si ya está volteada
        if (!isFlipped)
        {
            Flip();
            gameManager.CardRevealed(this);
        }
    }

    public void Flip()
    {
        isFlipped = !isFlipped;
        cardImage.sprite = isFlipped ? frontSprite : backSprite;
    }

    public Sprite GetFrontSprite()
    {
        return frontSprite;
    }

    public void DisableCard()
    {
        GetComponent<Button>().interactable = false;
    }
}


