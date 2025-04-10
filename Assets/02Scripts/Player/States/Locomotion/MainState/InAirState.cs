using UnityEngine;

public class InAirState : LocomotionStateBase
{
    private PlayerLocomotion locomotion;

    public InAirState(PlayerLocomotion locomotion)
    {
        this.locomotion = locomotion;
    }

    //public InAirState(PlayerLocomotion locomotion) : base(locomotion)
    //{
    // }

    protected override LocomotionMainState GetMainState()
    {
        throw new System.NotImplementedException();
    }
}
