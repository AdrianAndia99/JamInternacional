using UnityEngine;

public class ClickToRay : MonoBehaviour
{
    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.name == "XENOMORF")
                {
                    hit.transform.GetComponent<XenomorMovement>().Click();
                    return;
                }
            }
        }
    }
}