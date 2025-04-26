using UnityEngine;

//ÂøÁö »óÅÂ
public class LandState : LocomotionBaseState
{
    public LandState(PlayerCore playerCore) : base(playerCore){}
    public override LocomotionMainState DetermineStateType() => LocomotionMainState.Land;
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
