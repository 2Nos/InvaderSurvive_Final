using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class LocomotionBaseState
{
    public PlayerCore m_PlayerCore;

    //playerCore �ѱ�� ���� ������, ��� �� �Ű������� ���� �����ڰ� �ڵ� ������ ���� �ʱ⿡ ���� ������ �ۼ��������.
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
    /// IsAction() �޼��忡�� ActionStateFlags �켱���� ó���� �������� �Ϲ����� MainState üũ�Ͽ� ��ȯ
    /// m_locomotionMainAniMap�� ���� ���º�ȯ�� �ִϸ��̼� ��� �����Ͽ� ����
    /// </summary>
    public virtual void Update()
    {
        Movement(); // �̵� ���� ó��

        // ���� ��ȯ üũ
    }

    public virtual void Exit()
    {
        m_PlayerCore.m_Locomotion.UpdateLocomotionMainStateAnimation(EnterState(), false);
    }

    // �̵� �� ȸ�� ó��
    protected void Movement()
    {
        m_PlayerCore.m_Locomotion.HandleMove();
        m_PlayerCore.m_Locomotion.HandleRotation();
    }

    protected void SetMainState(LocomotionMainState state)
    {
        m_PlayerCore.m_StateFlagManager.m_LocomotionMain = state;
    }

    // ActionStateFlags �켱���� ó��
    protected LocomotionBaseState? IsAction()
    {
        // ���� �켱����: ������, �ǰ� ���� �� ���� ActionState �켱
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

    // Locomotion���� ���� Ȯ���ϸ� ��ȯ
    public LocomotionBaseState? CheckLocomotionTransition()
    {
        // =================== ���� ��ȯ ó�� ===================
        // ActionStateFlags �켱���� ó���� �������� �Ϲ����� MainState üũ
        if (IsAction() != null) return IsAction();

        // �ٸ� ���·� ��ȯ üũ
        return Create(GetLocomotionMainState(), m_PlayerCore);
    }


}