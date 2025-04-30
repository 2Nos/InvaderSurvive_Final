using DUS.Player.Locomotion;

public class ClimbState : LocomotionStrategyState
{
    public ClimbState(PlayerCore playerCore) : base(playerCore) { }

    protected override LocomotionMainState DetermineStateType() => LocomotionMainState.Climb;

    protected override AniParmType SetAniParmType() => AniParmType.SetBool;

    //public override LocomotionMainState DetermineStateType() => LocomotionMainState.Climb;
}
