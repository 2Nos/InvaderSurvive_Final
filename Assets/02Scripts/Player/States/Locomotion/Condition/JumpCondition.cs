using UnityEngine;

public class JumpCondition : IStateTransitionCondition
{
    private PlayerLocomotion locomotion;

    public int Priority => 50;

    public JumpCondition(PlayerLocomotion locomotion)
    {
        this.locomotion = locomotion;
    }

    public bool ShouldTransition()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public LocomotionStateBase GetNextState()
    {
        return new InAirState(locomotion);
    }
}