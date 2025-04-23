using UnityEngine;

public class ClimbState : LocomotionBaseState
{

    public ClimbState(PlayerLocomotion playerLocomotion) : base(playerLocomotion){}

    public override LocomotionMainState DetermineStateType() => LocomotionMainState.Climb;
}
