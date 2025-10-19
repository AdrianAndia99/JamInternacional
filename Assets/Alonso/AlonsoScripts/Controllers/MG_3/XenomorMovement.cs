using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class XenomorMovement : MonoBehaviour
{
    private Coroutine _moveCoroutine;

    private MoveTween _moveTween;

    public Vector3[] range = new Vector3[]{};

    private Vector3 _postionTarget;

    public UnityEvent OnClicked;

    private void Start()
    {
        _moveTween = GetComponent<MoveTween>();
        _moveTween._vectorMove = true;

        _moveCoroutine = StartCoroutine(MoveCycle());
    }
    public void StopMovement()
    {
        StopCoroutine(_moveCoroutine);
        _moveCoroutine = null;
        Debug.Log("STOP");
    }
    public IEnumerator MoveCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.05f, 0.3f));

            _postionTarget = RandomPositionUtility.GetRandomPointInQuad(range[0], range[1], range[2], range[3]);

            
            _moveTween.SetTargetVector(_postionTarget);
            _moveTween.SetDuration(0.1f);
            _moveTween.LocalMove();
        }
    }
    public void Click()
    {
        OnClicked?.Invoke();
    }
}