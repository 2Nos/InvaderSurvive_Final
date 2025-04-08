using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationManager : MonoBehaviour
{
    //Awake가 Locomotion보다 느려서 GetComponent 대신 직접 넣기(순서 바꾸는 방법은 있으나 일단 이렇게)
    [SerializeField] Animator m_animator;
    private readonly int m_moveSpeedHashX = Animator.StringToHash("MoveDirectionX");
    private readonly int m_moveSpeedHashY = Animator.StringToHash("MoveDirectionY");
    private readonly int m_isMovingHash = Animator.StringToHash("IsMoving");
    private readonly int m_isAimingHash = Animator.StringToHash("IsAiming");
    private readonly int m_isSprintingHash = Animator.StringToHash("IsSprinting");
    private readonly int m_isCrouchingHash = Animator.StringToHash("IsCrouching");
    private readonly int m_isJumpingHash = Animator.StringToHash("IsJumping");
    private readonly int m_isDodgingHash = Animator.StringToHash("IsDodging");
    private readonly int m_isRelodingHash = Animator.StringToHash("IsReloding");
    private readonly int m_isUsingSkillHash = Animator.StringToHash("IsUsingSkill");

    public void UpdateMovementAnimation(Vector3 inputMovement)
    {
        m_animator.SetFloat(m_moveSpeedHashX, inputMovement.x);
        m_animator.SetFloat(m_moveSpeedHashY, inputMovement.y);
    }

    public void SetMoving(bool isMoving)
    {
        m_animator.SetBool(m_isMovingHash, isMoving);
    }
    public void SetAiming(bool isAiming)
    {
        m_animator.SetBool(m_isAimingHash, isAiming);
    }

    public void SetSprinting(bool isSprinting)
    {
        m_animator.SetBool(m_isSprintingHash, isSprinting);
    }

    public void SetCrouching(bool isCrouching)
    {
        m_animator.SetBool(m_isCrouchingHash, isCrouching);
    }
    public void SetCrouchMoving(bool isCrouchMoving)
    {
        m_animator.SetBool(m_isCrouchingHash, isCrouchMoving);
    }
    public void SetJumping(bool isJumping)
    {
        m_animator.SetBool(m_isJumpingHash,isJumping);
    }
    public void SetDodging(bool isDodging)
    {
        m_animator.SetBool(m_isDodgingHash,isDodging);
    }
    public void SetUsingSkill(bool isCrouching)
    {
        m_animator.SetBool(m_isUsingSkillHash,isCrouching);
    }

    public void PlayAnimation(string animationName)
    {
        m_animator.Play(animationName);
    }

    public void CrossFadeAnimation(string animationName, float transitionDuration = 0.25f)
    {
        m_animator.CrossFade(animationName, transitionDuration);
    }
    
}