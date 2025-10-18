using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movimiento")]
    public float moveSpeed = 5f;

    [Header("C�mara")]
    public Transform cameraTransform;
    public float mouseSensitivity = 1f;
    public float pitchLimit = 85f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float xRotation = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void LateUpdate()
    {
        HandleCamera();
    }

    // -------------------------------
    //        INPUT CALLBACKS
    // -------------------------------
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    // -------------------------------
    //        MOVIMIENTO
    // -------------------------------
    private void HandleMovement()
    {
        // Direcci�n de movimiento relativa a la c�mara
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredMove = (forward * moveInput.y + right * moveInput.x).normalized * moveSpeed;
        rb.linearVelocity = new Vector3(desiredMove.x, rb.linearVelocity.y, desiredMove.z);
    }

    // -------------------------------
    //        C�MARA
    // -------------------------------
    private void HandleCamera()
    {
        // Rotaci�n horizontal (yaw)
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

        // Rotaci�n vertical (pitch)
        xRotation -= lookInput.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -pitchLimit, pitchLimit);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
