using UnityEngine;

public class IdleState : LocomotionStateBase
{
    private PlayerLocomotion playerLocomotion;

    public IdleState(PlayerLocomotion playerLocomotion)
    {
        this.playerLocomotion = playerLocomotion;
    }

    protected override LocomotionMainState GetMainState()
    {
        throw new System.NotImplementedException();
    }
}