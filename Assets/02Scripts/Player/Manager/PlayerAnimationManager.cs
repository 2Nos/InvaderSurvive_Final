using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] Animator m_animator;
    private readonly int m_moveSpeedHashX = Animator.StringToHash("MoveDirectionX");
    private readonly int m_moveSpeedHashY = Animator.StringToHash("MoveDirectionY");
    private readonly int m_isMovingHash = Animator.StringToHash("IsMoving");
    private readonly int m_isAimingHash = Animator.StringToHash("IsAiming");
    private readonly int m_isSprintingHash = Animator.StringToHash("IsSprinting");
    private readonly int m_isCrouchingHash = Animator.StringToHash("IsCrouching");

    private void Awake()
    {
        //m_animator = GetComponent<Animator>();
    }

    public void UpdateMovementAnimation(Vector3 MoveDirection, bool isMoving)
    {
        m_animator.SetFloat(m_moveSpeedHashX, MoveDirection.x);
        m_animator.SetFloat(m_moveSpeedHashY, MoveDirection.z);
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

    public void PlayAnimation(string animationName)
    {
        m_animator.Play(animationName);
    }

    public void CrossFadeAnimation(string animationName, float transitionDuration = 0.25f)
    {
        m_animator.CrossFade(animationName, transitionDuration);
    }
}