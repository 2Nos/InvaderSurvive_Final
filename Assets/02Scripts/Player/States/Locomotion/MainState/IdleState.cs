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

    // Idle일때는 회전만
    protected override void Movement()
    {
        m_PlayerLocomotion.HandleRotation();
    }
}