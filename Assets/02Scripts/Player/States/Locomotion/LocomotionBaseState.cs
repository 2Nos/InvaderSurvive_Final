using System;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public abstract class LocomotionBaseState : ISetBaseState
{
    #region ======================================== Module
    protected PlayerCore m_PlayerCore;
    protected PlayerLocomotion m_Locomotion;
    #endregion ======================================== /Module

    #region ======================================== 생성자 & Interface & 변수
    public LocomotionBaseState(PlayerCore playerCore)
    {
        m_PlayerCore = playerCore;
        m_Locomotion = playerCore.m_Locomotion;
    }

    // Interface
    public abstract LocomotionMainState DetermineStateType();
    private LocomotionMainState m_determineStateType = LocomotionMainState.Idle;
    public abstract AniParmType SetAniParmType();
    // 동작이 긴 액션들 동작 종료 체크를 위해(상태 전환 체크 지연) 필요 시 체크
    //public abstract bool StopCheckTransitionToInProgress();
    private bool m_isProgress = false;
    
    // 각 상태에서 Locomotion.DoTransitionState()를 통해서 변환
    public LocomotionBaseState m_ChangeMainState { get; private set; }
    #endregion ======================================== /생성자 & Interface & 변수

    // Enter, Exit, Update, FixedUpdate는 전략 패턴(Strategy) 방식으로 구현
    #region ======================================== Update, FixedUpdate, Enter, Exit

    public void InitializeIdle()
    {
        m_Locomotion.ClearLocomotionFlag(LocomotionSubFlags.Crouch);
        m_Locomotion.ClearLocomotionFlag(LocomotionSubFlags.Run);
    }
    public virtual void Enter()
    {
        // TODO : 상태 전한 지연 필요 시 사용
        //m_isProgress = StopCheckTransitionToInProgress();
        m_determineStateType = DetermineStateType();
        // TODO : Enter과 Exit에서 int나 float을 다루는 애니메이션의 값이 있다면 그때 작업
        m_Locomotion.SetMainStateAnimation(m_determineStateType, SetAniParmType(), true);
    }

    public virtual void FixedUpdate()
    {
        // 1. Movement
        Movement();
    }

    public virtual void Update()
    {
        // 2. base.Update로 상태 체크 - 각 상태에서의 변환상태 생성에 의한 보류
        //CheckLocomotion();

        // 3. Flags 애니메이션 처리
        m_Locomotion.UpdateLocomotionFlagAnimation();
    }

    public virtual void Exit()
    {
        // 현재 상태에 대한 설정들 취소
        m_Locomotion.SetMainStateAnimation(m_determineStateType, SetAniParmType(), false);      // 애니메이션 종료
        //m_Locomotion.SetInProgress(false);                                  // 진행
    }

    // 상태별 Movement 재정의 
    public virtual void Movement()
    {
        m_Locomotion.HandleMove();
        m_Locomotion.HandleRotation();
        m_Locomotion.HandleGravityMovement();
    }

    #endregion ======================================== /Update, FixedUpdate, Enter, Exit

    #region ======================================== 상태 전환 처리 관리
    // 상태 우선순위 처리
    public LocomotionBaseState? IsAction()
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
}
#endregion ======================================== /상태 전환 처리 관리
