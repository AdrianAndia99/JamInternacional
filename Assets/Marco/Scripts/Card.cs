using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Image frontImage;
    public Image backImage;
    [HideInInspector] public int cardId;
    private bool isFlipped = false;
    private MemoryGameManager manager;

    public void Setup(int id, Sprite sprite, MemoryGameManager mgr)
    {
        cardId = id;
        frontImage.sprite = sprite;
        manager = mgr;
    }

    public void OnClickCard()
    {
        if (!isFlipped)
        {
            isFlipped = true;
            frontImage.gameObject.SetActive(true);
            backImage.gameObject.SetActive(false);
            manager.CardSelected(this);
        }
    }

    public void HideCard()
    {
        isFlipped = false;
        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);
    }

    public void DisableCard()
    {
        GetComponent<Button>().interactable = false;
    }
}
