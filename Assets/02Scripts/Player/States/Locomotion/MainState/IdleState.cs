using UnityEngine;
using UnityEngine.UI;

public class IdleState : LocomotionBaseState
{
    public IdleState(PlayerCore playerCore) : base(playerCore){}

    public override LocomotionMainState EnterState() => LocomotionMainState.Idle; //���� ����


    //���� �ִϸ��̼�

    public override void Enter()
    {
        base.Enter();
        Debug.Log("IdleState Enter");
    }

    public override void Update()
    {
        base.Update();
        if (m_PlayerCore.m_InputManager.m_IsMove)
        {
            //�̵����̶�� �̵����·� ��ȯ
            SetMainState(LocomotionMainState.Move);
        }
        if (m_PlayerCore.m_InputManager.m_IsInAir)
        {
            //�������̶�� ���߻��·� ��ȯ
            SetMainState(LocomotionMainState.InAir);
        }

        m_PlayerCore.m_Locomotion.UpdateLocomotionFlagAnimation();
    }
    public override void Exit()
    {
        base.Exit();
    }

}