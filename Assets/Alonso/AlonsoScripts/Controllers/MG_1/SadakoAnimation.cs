using DG.Tweening;
using System.Collections;
using UnityEngine;

public class SadakoController : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Animator _animator;

    private Coroutine _positionCoroutine;

    private Vector3 _targetPosition;

    private bool _canMove = true;

    private bool _isLeftSided;


    [SerializeField] private MinigameManager_1 _manager;
    
    private void Start()
    {
        _targetPosition = transform.position;
    }
    private void Update()
    {
        if (_canMove == false) return;

        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _speed);
    }
    public void EnableMove(bool enable)
    {
        _canMove = enable;
    }
    public void StartAnim()
    {
        StartCoroutine(StartCycle());
    }
    public IEnumerator StartCycle()
    {
        yield return new WaitForSeconds(2);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        _animator.SetTrigger("IsRunning");

         StartCoroutine(SelectRandomPositionToMove());
    }
    public IEnumerator SelectRandomPositionToMove()
    {
        while (true) 
        { 
            yield return new WaitForSeconds(Random.Range(0f, 1f));

            int boolValue = Random.Range(0, 2);

            if (boolValue == 1)
            {
                _targetPosition = new Vector3(-35f, transform.localPosition.y, transform.localPosition.z);
            }
            else if (boolValue == 0)
            {
                _targetPosition = new Vector3(35f, transform.localPosition.y, transform.localPosition.z);
            }
        }
    }
    public void EvaluateDefeate(Collider other)
    {
        if (other.transform.tag == "DefeatLimit")
        {
            _manager.OnDefeat?.Invoke();
        }
        else if (other.transform.tag == "Player")
        {
            _animator.SetTrigger("IsDead");
            GetComponent<MoveTween>().KillCurrent();
            _manager.OnWin?.Invoke();
        }
    }
}