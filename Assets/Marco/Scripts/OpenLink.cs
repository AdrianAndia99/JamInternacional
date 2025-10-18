using UnityEngine;

public class OpenLink : MonoBehaviour
{
    // Llama esta función desde el botón
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}
