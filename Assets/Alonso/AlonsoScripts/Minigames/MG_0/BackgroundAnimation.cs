using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimation : MonoBehaviour
{
    [SerializeField] private float _transitionTime;

    [SerializeField] private List<Sprite> _frames;

    private SpriteRenderer _spriteRenderer;

    private Coroutine _animationCoroutine;

    private int _currentFrame;


    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        //DEBUG
        StartAnim();
    }
    public void StartAnim()
    {
        _animationCoroutine = StartCoroutine(AnimationCycle(_transitionTime));
    }
    public void StopAnim()
    {
        StopCoroutine(AnimationCycle(_transitionTime));
        _animationCoroutine = null;
    }
    public void ApplyFrame(int frame)
    {
        _spriteRenderer.sprite = _frames[frame];
    }
    public IEnumerator AnimationCycle(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            _currentFrame++;
            if (_currentFrame >= _frames.Count) 
            {
                _currentFrame = 0;
            }
            ApplyFrame(_currentFrame);
        }
    }
}