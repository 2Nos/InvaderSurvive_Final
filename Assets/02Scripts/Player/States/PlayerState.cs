using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

//Idle, Moving, IsInAir, Sliding, Climbing 같은 상태는 메인으로
public abstract class PlayerState
{
    protected PlayerLocomotion m_locomotion;
    protected FlagManager m_flagManager;
    protected PlayerAnimationManager m_animationManager;
    protected PlayerInputManager m_inputManager;
    protected PlayerInventoryManager m_inventoryManager;
    protected PlayerAbilityManager m_abilityManager;
    protected WeaponManager m_weaponManager;

    // 추가: 기본은 None 반환
    protected virtual LocomotionSubFlags LocomotionFlag => LocomotionSubFlags.None;
    protected virtual CombatSubFlags CombatFlag => CombatSubFlags.None;
    protected virtual InteractionFlags InteractionFlag => InteractionFlags.None;

    /// <summary>
    /// 선택적으로 사용할 수 있는 공통 조건 처리
    /// Dodge와 다르게 저 둘은 당하는 상태이기에
    /// </summary>
    /// <returns></returns>
    protected bool IsActionBlocked() => m_flagManager.HasActionStateFlags(ActionStateFlags.Staggered | ActionStateFlags.Knockback); //기절 상태나 넉백일 때 다른 동작 안되도록 하기위한 bool


    //생성자, 각 State에 상속시 매개변수가 있는 생성자는 자동 생성이 안되기에 만들어줘야함
    public PlayerState(PlayerLocomotion locomotion) 
    {
        m_locomotion = locomotion;
        m_animationManager = locomotion.GetAnimationManager();
        m_inputManager = locomotion.GetInputManager();
        m_flagManager = locomotion.GetFlagManager();
        m_inventoryManager = locomotion.GetInventoryManager();
        m_abilityManager = locomotion.GetAbilityManager();
        m_weaponManager = locomotion.GetWeaponManager();
    }



    #region ---------------------------------------- Animation 관리
    /// <summary>
    /// 각 상태마다 애니메이션 처리 자동으로 하기 위해
    /// </summary>
    protected virtual string AnimationBoolName => null;
    protected void SetAnimationState(string animName, bool value)
    {
        m_animationManager?.SetBool(animName, value);
    }
    #endregion ---------------------------------------- /Animation 관리

    public virtual void Enter()
    {
        if (AnimationBoolName != null) SetAnimationState(AnimationBoolName, true);
    }
    public virtual void Update() 
    {
        if (IsActionBlocked()) return;

        foreach (var transition in DefaultTransitions)
        {
            if (TryCommonTransition(transition)) return;
        }
    }
    public virtual void Exit() 
    {
        if (AnimationBoolName != null) SetAnimationState(AnimationBoolName, false);
    }

    protected void HandleMovement()
    {
        m_locomotion.HandleMovement();
        m_locomotion.HandleRotation();
    }

    #region ---------------------------------------- MainState 상태 전이 관리
    // 상태 전이용 enum
    protected enum Transition
    {
        IsDodging, IsMoving, IsInAir, IsClimbing, IsSliding, IsWallRunning
    }
    
    /// <summary>
    /// 기본 전이 우선순위 (필요 시 다른 State 클래스에서 오버라이드) 즉, 해당 상태로 못 넘어가는 State들 한해 오버라이드
    /// </summary>
    protected virtual Transition[] DefaultTransitions => new[]
    {
        Transition.IsDodging,
        Transition.IsMoving,
        Transition.IsInAir,
        Transition.IsClimbing,
        Transition.IsSliding,
        Transition.IsWallRunning
    };
    // 전이 시도 (공통)
    protected bool TryCommonTransition(Transition transition)
    {
        switch (transition)
        {
            case Transition.IsDodging:
                if (m_inputManager.IsDodging)
                {
                    m_flagManager.SetActionStateFlags(ActionStateFlags.Dodging);
                    return true;
                }
                break;

            case Transition.IsMoving:
                if (m_inputManager.IsMoving)
                {
                    m_locomotion.ChangeState(new MoveState(m_locomotion));
                    return true;
                }
                break;

            case Transition.IsInAir:
                if (m_locomotion.IsAir())
                {
                    m_locomotion.ChangeState(new InAirState(m_locomotion));
                    return true;
                }
                break;

            case Transition.IsClimbing:
                if (m_locomotion.IsClimbing())
                {
                    m_locomotion.ChangeState(new ClimbingState(m_locomotion));
                    return true;
                }
                break;

            case Transition.IsSliding:
                if (m_locomotion.IsSliding())
                {
                    m_locomotion.ChangeState(new SlideState(m_locomotion));
                    return true;
                }
                break;

            case Transition.IsWallRunning:
                if (m_locomotion.IsWallRunning())
                {
                    m_locomotion.ChangeState(new WallRunState(m_locomotion));
                    return true;
                }
                break;
        }
        return false;
    }
    #endregion ---------------------------------------- /MainState 상태 전이에 대한

    #region ---------------------------------------- SubFlag 상태 전이 관리
    protected void HandleLocomotionSubFlags()
    {
        // Sprinting
        if (m_inputManager.IsSprinting)
            m_flagManager.SetLocomotionFlag(LocomotionSubFlags.Sprinting);
        else
            m_flagManager.ClearLocomotionFlag(LocomotionSubFlags.Sprinting);

        // Crouching
        if (m_inputManager.IsCrouching)
            m_flagManager.SetLocomotionFlag(LocomotionSubFlags.Crouching);
        else
            m_flagManager.ClearLocomotionFlag(LocomotionSubFlags.Crouching);
    }
    #endregion -------------------------------------- /SubFlag 상태 전이 관리
}
