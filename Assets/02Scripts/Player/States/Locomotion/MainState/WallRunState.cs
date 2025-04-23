using UnityEngine;

public class WallRunState : LocomotionBaseState
{
    public WallRunState(PlayerLocomotion playerLocomotion) : base(playerLocomotion)
    {
    }

    public override LocomotionMainState DetermineStateType() => LocomotionMainState.WallRun;
}
