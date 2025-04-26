using UnityEngine;

public class InAirState : LocomotionBaseState
{
    public InAirState(PlayerCore playerCore) : base(playerCore) { }
    public override LocomotionMainState DetermineStateType() => LocomotionMainState.InAir;

    public override AniParmType SetAniParmType() => AniParmType.SetBool;

    public override void Enter()
    {
        base.Enter();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
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