using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScareShootGame : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject scarecrow;          // El espantapájaros
    public Transform[] appearPoints;      // Puntos donde aparece
    public Transform crosshair; //Mira en el mundo (no UI)
    public TextMeshProUGUI resultText;               // Texto de resultado ("You Win" / "You Lose")
    public float moveSpeed = 10f;
    public float appearDuration = 3f;     // Tiempo visible del espantapájaros
    public float hideOffsetY = 1.2f;      // Cuánto baja al esconderse

    [Header("Feedback")]
    public float crosshairScaleAmount = 1.2f;
    public float crosshairScaleDuration = 0.1f;

    private bool isVisible = false;
    private bool hasBullet = true;
    private bool roundActive = false;
    private float timer;

    private Collider2D scareCollider;
    private Animator scareAnimator;
    private Vector3 originalCrosshairScale;

    void Start()
    {
        scareAnimator = scarecrow.GetComponentInChildren<Animator>();
        scareCollider = scarecrow.GetComponent<Collider2D>();

        HideScarecrowInstant();
        if (crosshair != null)
            originalCrosshairScale = crosshair.localScale;

        Cursor.visible = false; // Ocultamos el cursor del sistema
        resultText.text = "";
        StartNextRound();
    }

    void Update()
    {
        MoveCrosshair();

        // Tiempo visible
        if (roundActive && isVisible)
        {
            timer += Time.deltaTime;
            if (timer >= appearDuration)
            {
                LoseRound(); // se acabó el tiempo sin disparar
            }
        }
    }
    void MoveCrosshair()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // Forzamos a quedarse en el plano 2D
        crosshair.position = Vector3.Lerp(crosshair.position, mousePos, Time.deltaTime * moveSpeed);
    }

    void StartNextRound()
    {
        resultText.DOFade(0, 0.3f);
        Invoke(nameof(RandomAppear), 1.2f);
    }

    void RandomAppear()
    {
        if (isVisible) return;

        int randomIndex = Random.Range(0, appearPoints.Length);
        scarecrow.transform.position = appearPoints[randomIndex].position;
        scarecrow.SetActive(true);
        scareAnimator.Play("Idle");
        scareCollider.enabled = true;

        isVisible = true;
        hasBullet = true;
        roundActive = true;
        timer = 0;
    }

    void HideScarecrow()
    {
        if (!scarecrow.activeSelf) return;

        isVisible = false;
        scareCollider.enabled = false;

        scareAnimator.Play("Move");
        scarecrow.transform
            .DOMoveY(scarecrow.transform.position.y - hideOffsetY, 0.5f)
            .SetEase(Ease.InQuad)
            .OnComplete(() => scarecrow.SetActive(false));
    }

    void HideScarecrowInstant()
    {
        scarecrow.SetActive(false);
        isVisible = false;
        scareCollider.enabled = false;
    }

    public void VoiceShoot()
    {
        if (!roundActive || !hasBullet) return;
        hasBullet = false;

        //Feedback mira (recoil)
        crosshair.DOScale(originalCrosshairScale * crosshairScaleAmount, crosshairScaleDuration)
            .OnComplete(() => crosshair.DOScale(originalCrosshairScale, crosshairScaleDuration));

        //Dibuja un rayo desde la cámara hacia la mira
        Vector3 origin = Camera.main.transform.position;
        Vector3 dir = (crosshair.position - origin).normalized;
        Debug.DrawRay(origin, dir * 20f, Color.yellow, 0.5f);

        //Detectar colisión
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, 100f);

        if (hit.collider != null && hit.collider.CompareTag("Scarecrow"))
            WinRound();
        else
            LoseRound();
    }

    void WinRound()
    {
        Debug.Log("¡Le diste al espantapájaros!");
        resultText.text = "¡GANASTE!";
        resultText.color = Color.green;
        resultText.DOFade(1, 0.3f);
        scareAnimator.Play("Move");
        HideScarecrow();
        EndRound();
    }

    void LoseRound()
    {
        Debug.Log("Fallaste o se acabó el tiempo...");
        resultText.text = "PERDISTE";
        resultText.color = Color.red;
        resultText.DOFade(1, 0.3f);
        HideScarecrow();
        EndRound();
    }

    void EndRound()
    {
        roundActive = false;
        Invoke(nameof(StartNextRound), 2.5f);
    }
}
