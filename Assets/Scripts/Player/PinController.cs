using Unity.Mathematics;
using UnityEngine;

public class PinController : MonoBehaviour
{
    private Material materialInstance;
    private bool isDown = false;
    private float fallThreshold = 30f; // grados desde la vertical

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        materialInstance = GetComponent<Renderer>().material;
        if (materialInstance != null)
            materialInstance.SetColor("_OutlineColor", Color.green);
    }

    public bool IsDown()
    {
        if (isDown) return true;

        float tilt = Vector3.Angle(transform.up, Vector3.up);
        if (tilt > fallThreshold)
        {
            isDown = true;
        }
        return isDown;
    }

    public void ResetPin()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        if (materialInstance != null)
            materialInstance.SetColor("_OutlineColor", Color.green);

        isDown = false;
    }
    public void ColisionColorMat()
    {
        if (materialInstance != null)
            materialInstance.SetColor("_OutlineColor", Color.red);
    }
}
