using UnityEngine;

public class OpenLink : MonoBehaviour
{
    // Llama esta funci�n desde el bot�n
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}
