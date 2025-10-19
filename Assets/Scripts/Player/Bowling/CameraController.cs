using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Movimiento del mouse")]
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float pitchLimit = 60f;
    [SerializeField] private float yawLimit = 70f;

    [Header("Zoom")]
    [SerializeField] private float zoomFOV = 30f;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float zoomSpeed = 10f;

    private float yaw = 0f;
    private float pitch = 0f;
    private bool isZooming = false;

    private Quaternion initialRotation;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        initialRotation = transform.localRotation;

    }

    // Callback para el movimiento del mouse
    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 mouseDelta = context.ReadValue<Vector2>();
        float deltaX = mouseDelta.x * sensitivity * Time.deltaTime;
        float deltaY = mouseDelta.y * sensitivity * Time.deltaTime;

        yaw += deltaX;
        pitch -= deltaY;

        // Limitar pitch (arriba / abajo)
        pitch = Mathf.Clamp(pitch, -pitchLimit, pitchLimit);
        // Limitar yaw (izquierda / derecha)
        yaw = Mathf.Clamp(yaw, -yawLimit, yawLimit);

        // Aplicar la rotación RELATIVA a la inicial
        Quaternion rotationOffset = Quaternion.Euler(pitch, yaw, 0f);
        transform.localRotation = initialRotation * rotationOffset;
    }

    // Callback para zoom (clic)
    public void OnZoom(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isZooming = true;
        }
        else if (context.canceled)
        {
            isZooming = false;
        }
    }

    private void Update()
    {
        float targetFOV = isZooming ? zoomFOV : normalFOV;
        cam.Lens.FieldOfView = Mathf.Lerp(cam.Lens.FieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }
}
