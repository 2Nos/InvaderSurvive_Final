using UnityEngine;
using UnityEngine.UI;

public class IdleState : LocomotionBaseState
{
    public IdleState(PlayerLocomotion playerLocomotion) : base(playerLocomotion){}

    public override LocomotionMainState DetermineStateType() => LocomotionMainState.Idle; //현재 상태

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }
    public override void Exit()
    {
        base.Exit();
    }

    protected override void Movement()
    {
        m_PlayerLocomotion.UpdateVerticalMovement();
        m_PlayerLocomotion.HandleRotation();
    }
}