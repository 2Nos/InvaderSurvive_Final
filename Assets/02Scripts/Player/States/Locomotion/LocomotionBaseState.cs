using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class LocomotionBaseState
{
    public PlayerCore m_PlayerCore;

    //playerCore 넘기기 위한 생성자, 상속 시 매개변수로 인해 생성자가 자동 생성이 되지 않기에 받은 곳에서 작성해줘야함.
    protected LocomotionBaseState(PlayerCore playerCore)
    {
        m_PlayerCore = playerCore;
    }

    /// <summary>
    /// EnterState-> GetLocomotionMainState -> CheckLocomotionTransition
    /// </summary>
    /// <returns></returns>
    public abstract LocomotionMainState EnterState();
    protected LocomotionMainState GetLocomotionMainState() => m_PlayerCore.m_StateFlagManager.m_LocomotionMain;
    public LocomotionBaseState GetCheckTransition() => CheckLocomotionTransition();

    protected LocomotionBaseState Create(LocomotionMainState state, PlayerCore core) =>
        state switch
        {
            LocomotionMainState.Idle => new IdleState(core),
            LocomotionMainState.Move => new MoveState(core),
            LocomotionMainState.InAir => new InAirState(core),
            LocomotionMainState.Slide => new SlideState(core),
            LocomotionMainState.Climb => new ClimbState(core),
            LocomotionMainState.WallRun => new WallRunState(core),
            _ => null,
        };

    public virtual void Enter()
    {
        m_PlayerCore.m_StateFlagManager.m_LocomotionMain = EnterState();
        m_PlayerCore.m_Locomotion.UpdateLocomotionMainStateAnimation(EnterState(), true);
    }

    /// <summary>
    /// IsAction() 메서드에서 ActionStateFlags 우선순위 처리가 끝났으면 일반적인 MainState 체크하여 변환
    /// m_locomotionMainAniMap을 통해 상태변환과 애니메이션 모두 매핑하여 관리
    /// </summary>
    public virtual void Update()
    {
        Movement(); // 이동 관련 처리

        // 상태 전환 체크
    }

    public virtual void Exit()
    {
        m_PlayerCore.m_Locomotion.UpdateLocomotionMainStateAnimation(EnterState(), false);
    }

    // 이동 및 회전 처리
    protected void Movement()
    {
        m_PlayerCore.m_Locomotion.HandleMove();
        m_PlayerCore.m_Locomotion.HandleRotation();
    }

    protected void SetMainState(LocomotionMainState state)
    {
        m_PlayerCore.m_StateFlagManager.m_LocomotionMain = state;
    }

    // ActionStateFlags 우선순위 처리
    protected LocomotionBaseState? IsAction()
    {
        // 공통 우선순위: 구르기, 피격 상태 등 전역 ActionState 우선
        /*if (m_PlayerCore.m_StateFlagManager.HasActionStateFlags(ActionStateFlags.Dodge))
        {
            return new DodgeState(m_PlayerCore);
        }
        if (m_PlayerCore.m_StateFlagManager.HasActionStateFlags(ActionStateFlags.Staggered))
        {
            return new StaggeredState(m_PlayerCore);
        }*/
        return null;
    }

    // Locomotion에서 상태 확인하며 전환
    public LocomotionBaseState? CheckLocomotionTransition()
    {
        // =================== 상태 전환 처리 ===================
        // ActionStateFlags 우선순위 처리가 끝났으면 일반적인 MainState 체크
        if (IsAction() != null) return IsAction();

        // 다른 상태로 전환 체크
        return Create(GetLocomotionMainState(), m_PlayerCore);
    }


}