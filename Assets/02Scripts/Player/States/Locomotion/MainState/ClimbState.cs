using UnityEngine;

public class ClimbState : LocomotionBaseState
{

    public ClimbState(PlayerCore playerCore) : base(playerCore){}

    public override LocomotionMainState EnterState() => LocomotionMainState.Climb;
}
