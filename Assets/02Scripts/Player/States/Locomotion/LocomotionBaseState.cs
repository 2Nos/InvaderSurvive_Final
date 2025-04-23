using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class LocomotionBaseState
{
    protected PlayerLocomotion m_PlayerLocomotion;
    protected PlayerCore m_PlayerCore;

    public LocomotionBaseState(PlayerLocomotion playerLocomotion)
    {
        m_PlayerLocomotion = playerLocomotion;
        m_PlayerCore = playerLocomotion.m_PlayerCore;
    }

    // 상태 변환 로직 : DetermineStateType-> GetCurrentLocomotionMainState -> TransitionLocomotion
    // 각 상태에서 DetermineStateType()를 오버라이드하여 상태를 반환하도록 한다.
    public abstract LocomotionMainState DetermineStateType(); 
    protected LocomotionMainState? prevLocomotionMainState = null;

    public LocomotionBaseState GetCheckTransition() => TransitionLocomotion();

    public virtual void Enter()
    {
        var state = DetermineStateType();
        prevLocomotionMainState = state;
        m_PlayerCore.m_StateFlagManager.m_LocomotionMain = state;
        m_PlayerLocomotion.UpdateLocomotionMainStateAnimation(state, true);
    }

    public virtual void FixedUpdate()
    {
        // 고정 업데이트는 물리 연산에 사용되므로, 물리 연산이 필요한 경우에만 사용
    }

    /// <summary>
    /// IsAction() 메서드에서 ActionStateFlags 우선순위 처리가 끝났으면 일반적인 MainState 체크하여 변환
    /// m_locomotionMainAniMap을 통해 상태변환과 애니메이션 모두 매핑하여 관리
    /// </summary>
    public virtual void Update()
    {
        Movement();
    }

    public virtual void Exit()
    {
        var state = DetermineStateType();
        prevLocomotionMainState = state;
        m_PlayerLocomotion.UpdateLocomotionMainStateAnimation(state, false);
    }

    protected virtual void Movement()
    {
        m_PlayerLocomotion.UpdateVerticalMovement();
        m_PlayerLocomotion.HandleRotation();
        m_PlayerLocomotion.HandleMove();
    }

    /// <summary>
    /// Locomotion ActionState Flags 우선순위 처리
    /// </summary>
    /// <returns></returns>
    protected LocomotionBaseState? IsAction()
    {
        // 공통 우선순위: 구르기, 피격 상태 등 전역 ActionState 우선

        /*if (m_PlayerLocomotion.m_StateFlagManager.HasActionStateFlags(ActionStateFlags.Dodge))
        {
            return new DodgeState(m_PlayerLocomotion);
        }
        if (m_PlayerLocomotion.m_StateFlagManager.HasActionStateFlags(ActionStateFlags.Staggered))
        {
            return new StaggeredState(m_PlayerLocomotion);
        }*/
        return null;
    }

    protected void SetMainState(LocomotionMainState state)
    {
        m_PlayerCore.m_StateFlagManager.m_LocomotionMain = state;
    }

    /// <summary>
    /// 상태 전환 검열
    /// </summary>
    protected void CheckLocomotion()
    {
        if(m_PlayerLocomotion.m_IsProgress) return;

        if (m_PlayerCore.m_InputManager.m_IsInAir_LocoM)
        {
            SetMainState(LocomotionMainState.InAir);
        }
        else if (m_PlayerCore.m_InputManager.m_IsClimb_LocoM)
        {
            SetMainState(LocomotionMainState.Climb);
        }
        else if (m_PlayerCore.m_InputManager.m_IsWallRun_LocoM)
        {
            SetMainState(LocomotionMainState.WallRun);
        }
        else if (m_PlayerCore.m_InputManager.m_IsMove_LocoM)
        {
            SetMainState(LocomotionMainState.Move);
        }
        else
        {
            SetMainState(LocomotionMainState.Idle);
        }
        m_PlayerLocomotion.UpdateLocomotionFlagAnimation();
    }

    /// <summary>
    /// 상태 전환 처리
    /// </summary>
    /// <returns></returns>
    public LocomotionBaseState TransitionLocomotion()
    {
        // =================== 상태 전환 처리 ===================
        // ActionStateFlags 우선순위 처리가 끝났으면 일반적인 MainState 체크

        // Todo : LocomotionActionStateFlags에 따라 상태 전환 처리 작성 완료 시 해당 코드로 변경
        //if (IsAction() is { } actionState) return actionState;

        if (IsAction() != null) return IsAction();

        CheckLocomotion();

        var current = m_PlayerCore.m_StateFlagManager.m_LocomotionMain;
        if (current == prevLocomotionMainState) return m_PlayerLocomotion.m_currentState;
        
        Debug.Log($"State Transition: {prevLocomotionMainState} -> {current}");
        return Create(current);
    }

    public LocomotionBaseState Create(LocomotionMainState state) =>
        state switch
        {
            LocomotionMainState.Idle => new IdleState(m_PlayerLocomotion),
            LocomotionMainState.Move => new MoveState(m_PlayerLocomotion),
            LocomotionMainState.InAir => new InAirState(m_PlayerLocomotion),
            LocomotionMainState.Land => new LandState(m_PlayerLocomotion),
            LocomotionMainState.Slide => new SlideState(m_PlayerLocomotion),
            LocomotionMainState.Climb => new ClimbState(m_PlayerLocomotion),
            LocomotionMainState.WallRun => new WallRunState(m_PlayerLocomotion),
            _ => null,
        };
}