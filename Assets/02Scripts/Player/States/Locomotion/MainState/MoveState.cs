using UnityEngine;

public class MoveState : LocomotionBaseState
{
    public MoveState(PlayerCore playerCore) : base(playerCore){}

    public override LocomotionMainState EnterState() => LocomotionMainState.Move;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("MoveState Enter");
    }

    public override void Update()
    {
        base.Update();
        if (!m_PlayerCore.m_InputManager.m_IsMove)
        {
            SetMainState(LocomotionMainState.Idle);
        }

        m_PlayerCore.m_Locomotion.UpdateLocomotionFlagAnimation();
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("MoveState Exit");
    }

}