using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private bool countDown = true;
    [SerializeField] private float startTimeInSeconds = 10;

    [Header("Events")]
    [SerializeField] public UnityEvent OnStartTimer;
    [SerializeField] public UnityEvent OnStopTimer;
    [SerializeField] public UnityEvent<int> OnSecondPassed;
    [SerializeField] public UnityEvent OnTimeFinished;

    private Coroutine timerCoroutine;
    private bool isRunning = false;
    private float currentTime;

    public void StartTimer()
    {
        if (isRunning) return;

        isRunning = true;
        currentTime = countDown ? startTimeInSeconds : 0f;
        OnStartTimer?.Invoke();

        timerCoroutine = StartCoroutine(TimerRoutine());
    }
    public void StopTimer()
    {
        if (!isRunning) return;

        isRunning = false;

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        OnStopTimer?.Invoke();
    }
    public void ResetTimer()
    {
        StopTimer();
        currentTime = countDown ? startTimeInSeconds : 0f;
    }
    private IEnumerator TimerRoutine()
    {
        int lastWholeSecond = Mathf.FloorToInt(currentTime);
        OnSecondPassed?.Invoke(lastWholeSecond);

        while (isRunning)
        {
            yield return null;
            currentTime += (countDown ? -Time.deltaTime : Time.deltaTime);

            int wholeSecond = Mathf.FloorToInt(currentTime);
            if (wholeSecond != lastWholeSecond)
            {
                lastWholeSecond = wholeSecond;
                OnSecondPassed?.Invoke(wholeSecond);
            }

            if (countDown && currentTime <= 0f)
            {
                currentTime = 0f;
                OnTimeFinished?.Invoke();
                StopTimer();
                yield break;
            }
        }
    }
    public float GetCurrentTime() => currentTime;
    public int GetCurrentTimeInSeconds() => Mathf.FloorToInt(currentTime);
    public bool IsRunning => isRunning;
}