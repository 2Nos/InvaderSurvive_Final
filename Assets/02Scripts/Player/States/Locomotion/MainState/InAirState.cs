using UnityEngine;

public class InAirState : LocomotionBaseState
{
    public InAirState(PlayerCore playerCore) : base(playerCore){}

    public override LocomotionMainState EnterState() => LocomotionMainState.InAir;

    public override void Enter()
    {
        base.Enter();
        Debug.Log("InAirState Enter");
        m_PlayerCore.m_Locomotion.HandleInAir();
    }
    public override void Update()
    {
        base.Update();
        if (m_PlayerCore.m_InputManager.m_IsMove)
        {
            //�̵����̶�� �̵����·� ��ȯ
            SetMainState(LocomotionMainState.Move);
        }
        if (m_PlayerCore.m_Locomotion.m_GetIsGrounded())
        {
            m_PlayerCore.m_Locomotion.HandleInAir();
            m_PlayerCore.m_Locomotion.SetIsGrounded(false);
            //�������̶�� �����·� ��ȯ
            SetMainState(LocomotionMainState.Idle);
        }
        m_PlayerCore.m_Locomotion.UpdateLocomotionFlagAnimation();
    }
}
