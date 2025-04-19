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
            //이동중이라면 이동상태로 전환
            SetMainState(LocomotionMainState.Move);
        }
        if (m_PlayerCore.m_Locomotion.m_GetIsGrounded())
        {
            m_PlayerCore.m_Locomotion.HandleInAir();
            m_PlayerCore.m_Locomotion.SetIsGrounded(false);
            //착지중이라면 대기상태로 전환
            SetMainState(LocomotionMainState.Idle);
        }
        m_PlayerCore.m_Locomotion.UpdateLocomotionFlagAnimation();
    }
}
