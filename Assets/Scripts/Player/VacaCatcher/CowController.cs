using UnityEngine;

public class CowController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float changeDirTime = 2f; // cada cuánto cambia de dirección
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;

    [Header("Animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float direction = 1f;
    private float timer;
    private bool isCaptured = false;

    private void OnEnable()
    {
        // reset total cuando el prefab se active de nuevo
        isCaptured = false;
        direction = Random.value > 0.5f ? 1f : -1f;
        timer = 0f;
        animator.Rebind();
        animator.Update(0f);
        animator.SetBool("isCaptured", false);
        animator.SetBool("isRunning", false);
    }

    private void Update()
    {
        if (isCaptured) return; // no moverse si fue capturada

        timer += Time.deltaTime;

        // cambiar dirección erráticamente
        if (timer >= changeDirTime)
        {
            direction = Random.value > 0.5f ? 1f : -1f;
            timer = 0f;
        }

        // movimiento
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);

        // límites
        Vector3 pos = transform.position;
        if (pos.x < minX)
        {
            pos.x = minX;
            direction = 1f;
        }
        else if (pos.x > maxX)
        {
            pos.x = maxX;
            direction = -1f;
        }
        transform.position = pos;

        // animaciones
        animator.SetBool("isRunning", true);
        spriteRenderer.flipX = direction < 0;
    }

    public void OnCaptured()
    {
        if (isCaptured) return;

        isCaptured = true;
        animator.SetBool("isRunning", false);
        animator.SetBool("isCaptured", true);
        Debug.Log("La vaca fue abducida");

        // podrías agregar aquí una animación de "subir" hacia el OVNI
        StartCoroutine(AscendAndDeactivate());
    }

    private System.Collections.IEnumerator AscendAndDeactivate()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * 5f; // subir un poco
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
