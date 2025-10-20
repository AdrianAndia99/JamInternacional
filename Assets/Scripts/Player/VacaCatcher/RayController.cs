using UnityEngine;
using System;
using System.Collections;
public class RayController : MonoBehaviour
{
    public Action onFinish;
    public Action onMiss;
    public Action onAbductionSuccess; // nuevo evento

    [SerializeField] private float activeTime = 1f;
    private bool hitSomething = false;
    private Coroutine checkRoutine;

    private void OnEnable()
    {
        hitSomething = false;
        checkRoutine = StartCoroutine(CheckMiss());
    }

    private void OnDisable()
    {
        if (checkRoutine != null)
            StopCoroutine(checkRoutine);
    }

    private IEnumerator CheckMiss()
    {
        yield return new WaitForSeconds(activeTime);

        if (!hitSomething)
        {
            onMiss?.Invoke();
        }

        EndRay();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cow") && !hitSomething)
        {
            hitSomething = true;

            var cow = other.GetComponent<CowController>();
            if (cow != null)
                cow.OnCaptured();

            onAbductionSuccess?.Invoke();

            Debug.Log("¡Vaca abducida!");
            StopAllCoroutines(); // detener comprobación de fallo
            EndRay();
        }
    }

    private void EndRay()
    {
        onFinish?.Invoke();
        //gameObject.SetActive(false); // desactivar en vez de destruir prefab reusable
    }
}
