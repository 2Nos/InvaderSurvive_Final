using UnityEngine;

public class SlideState : LocomotionBaseState
{
    public SlideState(PlayerLocomotion playerLocomotion) : base(playerLocomotion){}
    public override LocomotionMainState DetermineStateType() => LocomotionMainState.Slide;
}
