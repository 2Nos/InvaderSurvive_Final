using UnityEngine;

public class MoveState : LocomotionBaseState
{
    public MoveState(PlayerLocomotion playerLocomotion) : base(playerLocomotion){}

    public override LocomotionMainState DetermineStateType() => LocomotionMainState.Move;
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

}