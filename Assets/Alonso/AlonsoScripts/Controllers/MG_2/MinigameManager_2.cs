public class MinigameManager_2 : MinigameManager
{
    public void Defeat()
    {
        OnDefeat?.Invoke();
    }
}