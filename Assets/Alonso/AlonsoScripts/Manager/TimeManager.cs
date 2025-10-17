using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private bool countDown = true;
    [SerializeField] private int startTimeInSeconds = 10;

    [Header("Events")]
    [SerializeField] private UnityEvent OnStartTimer;
    [SerializeField] private UnityEvent OnStopTimer;
    [SerializeField] private UnityEvent<int> OnSecondPassed;
    [SerializeField] private UnityEvent OnTimeFinished;


    private float elapsedTime = 0f;
    private int lastWholeSecond = 0;
    private bool isRunning = false;

    private void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;
        int timeValue;

        if (countDown)
        {
            timeValue = Mathf.Max(0, startTimeInSeconds - Mathf.FloorToInt(elapsedTime));
            if (timeValue < lastWholeSecond)
            {
                lastWholeSecond = timeValue;
                OnSecondPassed?.Invoke(timeValue);

                if (timeValue == 0)
                {
                    StopTimer();
                    OnTimeFinished?.Invoke();
                }
            }
        }
        else
        {
            timeValue = Mathf.FloorToInt(elapsedTime);
            if (timeValue > lastWholeSecond)
            {
                lastWholeSecond = timeValue;
                OnSecondPassed?.Invoke(timeValue);
            }
        }
    }
    public void StartTimer()
    {
        if (isRunning) return;
        isRunning = true;
        elapsedTime = 0f;
        lastWholeSecond = countDown ? startTimeInSeconds : 0;
        OnStartTimer?.Invoke();
    }
    public void StopTimer()
    {
        if (!isRunning) return;
        isRunning = false;
        OnStopTimer?.Invoke();
    }
    public void ResetTimer()
    {
        elapsedTime = 0f;
        lastWholeSecond = countDown ? startTimeInSeconds : 0;
    }
    public int GetCurrentTimeInSeconds()
    {
        return countDown ? Mathf.Max(0, startTimeInSeconds - Mathf.FloorToInt(elapsedTime)) : Mathf.FloorToInt(elapsedTime);
    }
    public bool IsRunning => isRunning;
}