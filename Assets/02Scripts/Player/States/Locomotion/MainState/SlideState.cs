using UnityEngine;

public class SlideState : LocomotionBaseState
{
    public SlideState(PlayerCore playerCore) : base(playerCore){}
    public override LocomotionMainState EnterState() => LocomotionMainState.Slide;
}
