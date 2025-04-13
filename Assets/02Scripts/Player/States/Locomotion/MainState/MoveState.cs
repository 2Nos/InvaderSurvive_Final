using UnityEngine;

public class MoveState : LocomotionBaseState
{
    public MoveState(PlayerCore playerCore) : base(playerCore){}

    protected override LocomotionMainState GetMainState() => LocomotionMainState.Moving;
    

    protected override string SetAnimationBoolName() => AniKeys.Move;
}