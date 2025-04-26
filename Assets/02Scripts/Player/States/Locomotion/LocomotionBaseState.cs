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

    #region ======================================== ������ & Interface & ����
    public LocomotionBaseState(PlayerCore playerCore)
    {
        m_PlayerCore = playerCore;
        m_Locomotion = playerCore.m_Locomotion;
    }

    // Interface
    public abstract LocomotionMainState DetermineStateType();
    private LocomotionMainState m_determineStateType = LocomotionMainState.Idle;
    public abstract AniParmType SetAniParmType();
    // ������ �� �׼ǵ� ���� ���� üũ�� ����(���� ��ȯ üũ ����) �ʿ� �� üũ
    //public abstract bool StopCheckTransitionToInProgress();
    private bool m_isProgress = false;
    
    // �� ���¿��� Locomotion.DoTransitionState()�� ���ؼ� ��ȯ
    public LocomotionBaseState m_ChangeMainState { get; private set; }
    #endregion ======================================== /������ & Interface & ����

    // Enter, Exit, Update, FixedUpdate�� ���� ����(Strategy) ������� ����
    #region ======================================== Update, FixedUpdate, Enter, Exit

    public void InitializeIdle()
    {
        m_Locomotion.ClearLocomotionFlag(LocomotionSubFlags.Crouch);
        m_Locomotion.ClearLocomotionFlag(LocomotionSubFlags.Run);
    }
    public virtual void Enter()
    {
        // TODO : ���� ���� ���� �ʿ� �� ���
        //m_isProgress = StopCheckTransitionToInProgress();
        m_determineStateType = DetermineStateType();
        // TODO : Enter�� Exit���� int�� float�� �ٷ�� �ִϸ��̼��� ���� �ִٸ� �׶� �۾�
        m_Locomotion.SetMainStateAnimation(m_determineStateType, SetAniParmType(), true);
    }

    public virtual void FixedUpdate()
    {
        // 1. Movement
        Movement();
    }

    public virtual void Update()
    {
        // 2. base.Update�� ���� üũ - �� ���¿����� ��ȯ���� ������ ���� ����
        //CheckLocomotion();

        // 3. Flags �ִϸ��̼� ó��
        m_Locomotion.UpdateLocomotionFlagAnimation();
    }

    public virtual void Exit()
    {
        // ���� ���¿� ���� ������ ���
        m_Locomotion.SetMainStateAnimation(m_determineStateType, SetAniParmType(), false);      // �ִϸ��̼� ����
        //m_Locomotion.SetInProgress(false);                                  // ����
    }

    // ���º� Movement ������ 
    public virtual void Movement()
    {
        m_Locomotion.HandleMove();
        m_Locomotion.HandleRotation();
        m_Locomotion.HandleGravityMovement();
    }

    #endregion ======================================== /Update, FixedUpdate, Enter, Exit

    #region ======================================== ���� ��ȯ ó�� ����
    // ���� �켱���� ó��
    public LocomotionBaseState? IsAction()
    {
        // ���� �켱����: ������, �ǰ� ���� �� ���� ActionState �켱

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
#endregion ======================================== /���� ��ȯ ó�� ����
