using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using Unity.Mathematics;
using System.Collections;

public class JackOBowlingController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 60f;

    [Header("Lanzamiento")]
    [SerializeField] private float minThrowForce = 5f;
    [SerializeField] private float maxThrowForce = 20f;
    [SerializeField] private float stopTimeToReset = 2f; // tiempo que puede estar quieta antes de perder intento
    [SerializeField] private float strikeTimeLimit = 20f;

    [Header("Rotaci�n visual del mesh")]
    public Transform meshObject;        // el hijo visual (as�gnalo en el inspector)
    public float meshRotationMultiplier = 200f; // controla velocidad de giro visual

    [Header("UI")]
    [SerializeField] private Slider powerSlider; // Asigna un slider del canvas
    [SerializeField] private GameObject sliderContainer; // objeto padre del slider para activarlo/desactivarlo

    [Header("Referencias")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform SpawnPumpkin;
    [SerializeField] CinemachineCamera camBall;
    [SerializeField] CinemachineCamera camStatic;
    [SerializeField] TeleDisplayController TVDC;

    [Header("Sounds")]
    [SerializeField] private AudioClipSO PumpkinSound;

    private Vector2 moveInput;
    private Vector2 angleInput;

    private bool isFrozen = false;
    private bool isThrown = false;
    public bool timerActive = false;
    private bool firstThrow = false;
    private bool gameEnded = false;

    private int attempts = 2;

    private float powerValue = 0f;
    private float powerDirection = 1f;
    private float powerSpeed = 1f;
    private bool isCharging = false;
    private float stillTimer = 0f;
    private float timeRemaining;

    private quaternion InitialRotation;
    private Transform child;
    private void OnEnable()
    {
        ResetGame();
    }

    private void OnDisable()
    {
        // Opcional: detener físicas por seguridad
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    private void Start()
    {
        child = transform.GetChild(0);
        InitialRotation = child.transform.rotation;
    }

    public void ResetGame()
    {
        Debug.Log("Reiniciando minijuego de bolos...");

        // Variables base
        attempts = 2;
        isFrozen = false;
        isThrown = false;
        firstThrow = false;
        timerActive = true;
        gameEnded = false;
        isCharging = false;
        stillTimer = 0f;

        // Reiniciar tiempo
        timeRemaining = strikeTimeLimit;

        // Resetear posición y rotación de la bola
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = SpawnPumpkin.position;
        transform.rotation = SpawnPumpkin.rotation;
        if (meshObject != null) meshObject.localRotation = quaternion.identity;

        // Resetear UI
        sliderContainer.SetActive(false);
        powerSlider.value = 0f;

        // Cámaras
        if (camBall != null) camBall.Priority = 1;
        if (camStatic != null) camStatic.Priority = 0;

        StartCoroutine(ResetAfterDelay(1.5f));
    }

    // --- INPUT CALLBACKS ---
    public void OnMove(InputAction.CallbackContext context)
    {
        if (isThrown || isFrozen) return;
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnAngle(InputAction.CallbackContext context)
    {
        if (!isFrozen || isThrown) return;
        angleInput = context.ReadValue<Vector2>();
    }

    public void OnFreeze(InputAction.CallbackContext context)
    {
        if (context.performed && !isThrown)
        {
            isFrozen = !isFrozen;
            moveInput = Vector2.zero;
            angleInput = Vector2.zero;
        }
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (gameEnded) return;

        // Empieza a cargar la fuerza
        if (context.started && isFrozen && !isThrown && attempts > 0)
        {
            isCharging = true;
            powerValue = 0f;
            powerDirection = 1f;
            powerSpeed = UnityEngine.Random.Range(1.5f, 2f); // velocidad aleatoria de la barra
            sliderContainer.SetActive(true);
            powerSlider.value = 0f;
        }

        // Suelta la tecla: lanzar
        if (context.canceled && isCharging)
        {
            isCharging = false;
            sliderContainer.SetActive(false);

            float t = powerSlider.value; // valor de 0 a 1
            float force = Mathf.Lerp(minThrowForce, maxThrowForce, t);

            isThrown = true;
            attempts--;

            PumpkinSound.PlayOneShoot();
            Vector3 direction = Quaternion.Euler(0, transform.eulerAngles.y, 0) * Vector3.forward;
            rb.AddForce(direction * force, ForceMode.Impulse);

            if (!firstThrow)
            {
                firstThrow = true;
                TVDC.OnFirstThrow();
            }
        }
    }

    private void Update()
    {
        if (gameEnded) return;

        // Control del tiempo de strike
        if (timerActive)
        {
            timeRemaining -= Time.deltaTime;
            TVDC?.UpdateTimerUI(timeRemaining);

            if (timeRemaining <= 0f)
            {
                timerActive = false;
                EndGame();
            }
        }

        if (isThrown)
        {
            CheckIfStopped();

            // Rotaci�n visual del mesh seg�n la velocidad
            if (meshObject != null && rb.linearVelocity.magnitude > 0.1f)
            {
                float rotationSpeed = rb.linearVelocity.magnitude * meshRotationMultiplier * Time.deltaTime;
                meshObject.Rotate(Vector3.right, rotationSpeed, Space.Self);
            }

            return;
        }

        if (!isFrozen)
        {
            transform.Translate(Vector3.right * moveInput.x * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(Vector3.up * angleInput.x * rotationSpeed * Time.deltaTime);
        }

        // Movimiento de la barra de potencia
        if (isCharging)
        {
            powerValue += powerDirection * powerSpeed * Time.deltaTime;
            if (powerValue >= 1f)
            {
                powerValue = 1f;
                powerDirection = -1f;
            }
            else if (powerValue <= 0f)
            {
                powerValue = 0f;
                powerDirection = 1f;
            }

            powerSlider.value = powerValue;
        }
    }

    private void CheckIfStopped()
    {
        if (rb.linearVelocity.magnitude < 0.1f)
        {
            stillTimer += Time.deltaTime;

            if (stillTimer >= stopTimeToReset)
            {
                ResetAttemptFromManager();
            }
        }
        else
        {
            stillTimer = 0f;
        }
    }

    public void ResetAttemptFromManager()
    {
        stillTimer = 0f;
        isThrown = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = SpawnPumpkin.transform.rotation;
        transform.position = SpawnPumpkin.transform.position;
        meshObject.transform.rotation = InitialRotation;
        isFrozen = false;

        // Reactivar c�mara principal
        if (camBall != null)
        {
            camBall.Priority = 1;
        }
        if (camStatic != null)
        {
            camStatic.Priority = 0;
        }
        Debug.Log("Intentos restantes: " + attempts);

        // Si ya no quedan intentos, terminar
        if (attempts <= 0)
        {
            timerActive = false;
            TVDC?.ShowGameOver();
            EndGame();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isThrown) return;

        // Si golpea un pino o el suelo de los pinos, notifica al manager
        if (collision.gameObject.CompareTag("Pin") || collision.gameObject.CompareTag("BowlingArea"))
        {
            FindObjectOfType<BowlingManager>().OnBallCollision();
            collision.gameObject.GetComponent<PinController>().ColisionColorMat();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // Cambiar de c�mara al pasar cierto trigger
        if (other.CompareTag("CameraTrigger"))
        {
            if (camBall != null) camBall.Priority = 0;
            if (camStatic != null) camStatic.Priority = 1;
        }
    }
    // --- UI desde Manager ---
    public void UpdateScoreUI(int knockedPins)
    {
        bool strike = knockedPins >= 10;

        if (strike)
        {
            timerActive = false;
        }

        TVDC?.UpdateTeleScores(knockedPins, attempts, strike);
    }
    private void EndGame()
    {
        timerActive = false;
        gameEnded = true;
        Debug.Log("Fin del minijuego: sin tiempo o sin intentos.");

        // Aquí puedes desactivar el prefab si quieres automáticamente:
        // gameObject.SetActive(false);
    }

    private IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Llamamos al manager para reiniciar pinos y bola
        var manager = FindObjectOfType<BowlingManager>();
        if (manager != null)
        {
            manager.ResetAllPins();
            Debug.Log("Minijuego reseteado después del delay");
        }
    }
}
