using UnityEngine;

public class AnimationHelper : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void SetOnlyBoolTrue(string parameterName)
    {
        if (animator == null) return;

        AnimatorControllerParameter[] parameters = animator.parameters;

        foreach (AnimatorControllerParameter param in parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool)
            {
                bool valueToSet = param.name == parameterName;
                animator.SetBool(param.name, valueToSet);
            }
        }
    }
    public void PlayAnimation(string animationName)
    {
        animator.Play(animationName, 0, 0f);   
    }
}