using UnityEngine;


public static class AnimKeys
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

    /// <summary>
    /// 애니메이션 Bool 상태 설정
    /// </summary>
    public void SetBool(string paramName, bool value)
    {
        if (!m_animator) return;
        m_animator.SetBool(paramName, value);
    }
    /// <summary>
    /// 애니메이션 Float 값 설정 (ex. 속도 등)
    /// </summary>
    public void SetFloat(string paramName, float value)
    {
        if (!m_animator) return;
        m_animator.SetFloat(paramName, value);
    }

    /// <summary>
    /// 애니메이션 Int 값 설정
    /// </summary>
    public void SetInt(string paramName, int value)
    {
        if (!m_animator) return;
        m_animator.SetInteger(paramName, value);
    }
    /// <summary>
    /// 애니메이션 Trigger 설정
    /// </summary>
    public void SetTrigger(string paramName)
    {
        if (!m_animator) return;
        m_animator.SetTrigger(paramName);
    }

    /// <summary>
    /// Trigger 리셋
    /// </summary>
    public void ResetTrigger(string paramName)
    {
        if (!m_animator) return;
        m_animator.ResetTrigger(paramName);
    }

    /// <summary>
    /// 현재 애니메이션 상태 이름 반환
    /// </summary>
    public string GetCurrentStateName(int layerIndex = 0)
    {
        if (!m_animator) return string.Empty;
        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(layerIndex);
        return stateInfo.IsName("") ? "" : stateInfo.shortNameHash.ToString();
    }

    public void UpdateMovementAnimation(Vector3 inputMovement)
    {
        m_animator.SetFloat(m_moveSpeedHashX, inputMovement.x);
        m_animator.SetFloat(m_moveSpeedHashY, inputMovement.y);
    }

    /*public void SetIdle(bool isIdle)
    {
        m_animator.SetBool(m_isIdleHash, isIdle);
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

    public void SetWallRun(bool isWallRunning)
    {
        m_animator.SetBool(m_isWallRunningHash, isWallRunning);
    }*/

    public void CrossFadeAnimation(string animationName, float transitionDuration = 0.25f)
    {
        m_animator.CrossFade(animationName, transitionDuration);
    }
    
}