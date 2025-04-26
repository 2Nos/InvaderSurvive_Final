
public class ClimbState : LocomotionBaseState
{
    public ClimbState(PlayerCore playerCore) : base(playerCore) { }

    public override LocomotionMainState DetermineStateType() => LocomotionMainState.Climb;

    public override AniParmType SetAniParmType()
    {
        throw new System.NotImplementedException();
    }

    //public override LocomotionMainState DetermineStateType() => LocomotionMainState.Climb;
}
