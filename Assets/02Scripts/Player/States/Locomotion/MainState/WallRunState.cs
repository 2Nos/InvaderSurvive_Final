public class WallRunState : LocomotionBaseState
{
    public WallRunState(PlayerCore playerCore) : base(playerCore){}

    public override LocomotionMainState DetermineStateType() => LocomotionMainState.WallRun;

    public override AniParmType SetAniParmType() => AniParmType.SetBool;
}
