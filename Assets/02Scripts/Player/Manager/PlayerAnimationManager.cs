using UnityEngine;


public static class AniKeys
{
    public const string Idle = "IsIdle";
    public const string Move = "IsMoving";
    public const string Slide = "IsSliding";
    public const string InAir = "IsInAir";
    public const string InClimb = "IsClimbing";
    public const string WallRun = "IsWallRunning";
}

[RequireComponent(typeof(Animator))]
public class PlayerAnimationManager : MonoBehaviour
{
    //Awake가 Locomotion보다 느려서 GetComponent 대신 직접 넣기(순서 바꾸는 방법은 있으나 일단 이렇게)
    [SerializeField] Animator m_animator;

    private readonly int m_moveSpeedHashX = Animator.StringToHash("MoveDirectionX");
    private readonly int m_moveSpeedHashY = Animator.StringToHash("MoveDirectionY");
    public void SetBool(string name, bool value) => m_animator.SetBool(name, value);
    public void SetTrigger(string name) => m_animator.SetTrigger(name);
    public void SetFloat(string name, float value) => m_animator.SetFloat(name, value);

    public void UpdateMovementAnimation(Vector3 inputMovement)
    {
        m_animator.SetFloat(m_moveSpeedHashX, inputMovement.x);
        m_animator.SetFloat(m_moveSpeedHashY, inputMovement.y);
    }

    public void CrossFadeAnimation(string animationName, float transitionDuration = 0.25f)
    {
        m_animator.CrossFade(animationName, transitionDuration);
    }
    
}