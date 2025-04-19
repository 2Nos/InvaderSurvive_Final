using UnityEngine;
using UnityEngine.UI;

public class IdleState : LocomotionBaseState
{
    public IdleState(PlayerCore playerCore) : base(playerCore){}

    public override LocomotionMainState EnterState() => LocomotionMainState.Idle; //현재 상태


    //현재 애니메이션

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
            //이동중이라면 이동상태로 전환
            SetMainState(LocomotionMainState.Move);
        }
        if (m_PlayerCore.m_InputManager.m_IsInAir)
        {
            //점프중이라면 공중상태로 전환
            SetMainState(LocomotionMainState.InAir);
        }

        m_PlayerCore.m_Locomotion.UpdateLocomotionFlagAnimation();
    }
    public override void Exit()
    {
        base.Exit();
    }

}