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

    // ���� ��ȯ ���� : DetermineStateType-> GetCurrentLocomotionMainState -> TransitionLocomotion
    // �� ���¿��� DetermineStateType()�� �������̵��Ͽ� ���¸� ��ȯ�ϵ��� �Ѵ�.
    public abstract LocomotionMainState DetermineStateType(); 
    protected LocomotionMainState? prevLocomotionMainState = null;

    private LocomotionBaseState m_changeMainState = null;
    public LocomotionBaseState GetCheckTransition() => m_changeMainState;

    public virtual void Enter()
    {
        var state = DetermineStateType();
        prevLocomotionMainState = state;
        m_PlayerCore.m_StateFlagManager.m_LocomotionMain = state;
        m_PlayerLocomotion.UpdateLocomotionMainStateAnimation(state, true);
    }

    public virtual void FixedUpdate()
    {
        //VerticalMovement();
    }

    /// <summary>
    /// IsAction() �޼��忡�� ActionStateFlags �켱���� ó���� �������� �Ϲ����� MainState üũ�Ͽ� ��ȯ
    /// m_locomotionMainAniMap�� ���� ���º�ȯ�� �ִϸ��̼� ��� �����Ͽ� ����
    /// </summary>
    public virtual void Update()
    {
        m_changeMainState = TransitionLocomotion();
        m_PlayerLocomotion.UpdateCheckInAir();
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
        m_PlayerLocomotion.HandleRotation();
        m_PlayerLocomotion.HandleMove();
    }
    protected virtual void VerticalMovement()
    {
        //m_PlayerLocomotion.UpdateGravityMovement();

    }

    /// <summary>
    /// Locomotion ActionState Flags �켱���� ó��
    /// </summary>
    /// <returns></returns>
    protected LocomotionBaseState? IsAction()
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

    protected void SetMainState(LocomotionMainState state)
    {
        m_PlayerCore.m_StateFlagManager.m_LocomotionMain = state;
    }

    /// <summary>
    /// ���� ��ȯ �˿�
    /// </summary>
    protected void CheckLocomotion()
    {
        //m_IsProgress�� true�� ��쿡�� ���� ��ȯ�� ���� ����
        if (m_PlayerLocomotion.m_IsProgress) return;

        if (!m_PlayerLocomotion.m_IsGrounded) //���� �������� ��Ȳ ���
        {
            SetMainState(LocomotionMainState.InAir); //LasndState ���� ó��
        }
        else if (m_PlayerCore.m_InputManager.m_IsInAir_LocoM)
        {
            m_PlayerLocomotion.ExecuteJump();
            SetMainState(LocomotionMainState.InAir); //LasndState ���� ó��
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
        Debug.Log($"CheckLocomotion");
        m_PlayerLocomotion.UpdateLocomotionFlagAnimation();
    }

    /// <summary>
    /// ���� ��ȯ ó��
    /// </summary>
    /// <returns></returns>
    public LocomotionBaseState TransitionLocomotion()
    {
        // =================== ���� ��ȯ ó�� ===================
        // ActionStateFlags �켱���� ó���� �������� �Ϲ����� MainState üũ

        // Todo : LocomotionActionStateFlags�� ���� ���� ��ȯ ó�� �ۼ� �Ϸ� �� �ش� �ڵ�� ����
        //if (IsAction() is { } actionState) return actionState;

        if (IsAction() != null) return IsAction();

        CheckLocomotion();

        var current = m_PlayerCore.m_StateFlagManager.m_LocomotionMain;
        if (current == prevLocomotionMainState) return m_PlayerLocomotion.m_currentState;
        
        Debug.Log($"State Transition: {current}");
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