using UnityEngine;

public class InAirState : LocomotionBaseState
{
    public InAirState(PlayerCore playerCore) : base(playerCore){}

    protected override LocomotionMainState GetMainState() => LocomotionMainState.InAir;
    protected override string SetAnimationBoolName() => AniKeys.InAir;
    
}
