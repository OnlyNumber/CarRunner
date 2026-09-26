using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    private const string Idle_Animation = "Idle";
    private const string Move_Animation = "Move";
    private const string Hitted_Animation = "Hitted";

    [SerializeField] private Animator _animator;
    public StateAnimation CurrentAnimaion
    {
        get;
        private set;
    }

    public void SetAnimation(StateAnimation stateAnimation)
    {
        switch (stateAnimation)
        {
            case StateAnimation.Idle:
                _animator.Play(Idle_Animation);
                break;
            case StateAnimation.Move:
                _animator.Play(Move_Animation);
                break;
            case StateAnimation.Hitted:
                _animator.Play(Hitted_Animation);
                break;
        }

        CurrentAnimaion = stateAnimation;
    }

    public AnimatorClipInfo GetCurrentAnimatorClipInfo()
    {
        return _animator.GetCurrentAnimatorClipInfo(0)[0];
    }

    public enum StateAnimation
    {
        Idle,
        Move,
        Hitted
    }
}