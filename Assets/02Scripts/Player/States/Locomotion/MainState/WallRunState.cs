using DUS.Player.Locomotion;

public class WallRunState : LocomotionStrategyState
{
    public WallRunState(PlayerCore playerCore) : base(playerCore){}

    protected override LocomotionMainState DetermineStateType() => LocomotionMainState.WallRun;

    protected override AniParmType SetAniParmType() => AniParmType.SetBool;
}
