using UnityEngine;
using System.Collections.Generic;

public class MinigameMaster : MonoBehaviour
{
    public List<GameObject> Minigames;

    private GameObject _currentMinigame;
    public void GetRandomMinigame()
    {
        if (_currentMinigame != null) return;

        _currentMinigame = Instantiate(Minigames[Random.Range(0, Minigames.Count)]);

        Camera.main.cullingMask = ~(1 << 3);
        //MinigameManager newMng = newMg.GetComponent<MinigameManager>();
    }
    public void DestroyCurrent()
    {
        if (_currentMinigame == null) return;

        Destroy( _currentMinigame );
        Camera.main.cullingMask = ~0;
    }
}