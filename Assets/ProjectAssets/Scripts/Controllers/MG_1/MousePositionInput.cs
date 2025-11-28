using UnityEngine;
using UnityEngine.Events;

public class MousePositionInput : MonoBehaviour
{
    private string lastSide = "";
    private Vector3 lastMousePosition;

    public UnityEvent OnTurnLeft;
    public UnityEvent OnTurnRight;

    void Update()
    {
        if (Input.mousePosition == lastMousePosition)
            return;

        lastMousePosition = Input.mousePosition;

        float mouseX = Input.mousePosition.x;
        float screenHalf = Screen.width / 2f;
        string currentSide = mouseX < screenHalf ? "left" : "right";

        if (currentSide != lastSide)
        {
            if (currentSide == "left")
                OnTurnLeft?.Invoke();
            else
                OnTurnRight?.Invoke();

            lastSide = currentSide;
        }
    }
}