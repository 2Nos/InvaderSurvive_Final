using UnityEngine;

public class DodgeState : LocomotionStateBase
{
    private PlayerLocomotion locomotion;

    public DodgeState(PlayerLocomotion locomotion)
    {
        this.locomotion = locomotion;
    }

    protected override LocomotionMainState GetMainState()
    {
        throw new System.NotImplementedException();
    }
}
