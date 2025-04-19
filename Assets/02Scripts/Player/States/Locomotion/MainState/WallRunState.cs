using UnityEngine;

public class WallRunState : LocomotionBaseState
{
    public WallRunState(PlayerCore playerCore) : base(playerCore)
    {
    }

    public override LocomotionMainState EnterState() => LocomotionMainState.WallRun;
}
