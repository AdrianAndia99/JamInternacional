using UnityEngine;
using System;
using System.Collections;
public class RayController : MonoBehaviour
{
    public Action onFinish;
    public Action onMiss;

    [SerializeField] private float activeTime = 1f; // tiempo de verificación (1 segundo)
    private bool hitSomething = false;

    private void OnEnable()
    {
        // Empieza la comprobación de fallo
        StartCoroutine(CheckMiss());
    }

    private IEnumerator CheckMiss()
    {
        yield return new WaitForSeconds(activeTime);

        // Si después de 1 segundo no tocó nada se considera fallo
        if (!hitSomething)
        {
            onMiss?.Invoke();
            this.gameObject.SetActive(false);
        }

        // Finaliza el rayo (destruye o desactiva)
        EndRay();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cow"))
        {
            hitSomething = true;

            var cow = other.GetComponent<CowController>();
            if (cow != null)
                cow.OnCaptured();

            Debug.Log("¡Vaca abducida! Ganaste");
            EndRay();
        }
    }

    private void EndRay()
    {
        onFinish?.Invoke();
    }
}
