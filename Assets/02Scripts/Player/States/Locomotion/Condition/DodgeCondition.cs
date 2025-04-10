using UnityEngine;

public class DodgeCondition : IStateTransitionCondition
{
    private PlayerLocomotion locomotion;

    public int Priority => 100;

    public DodgeCondition(PlayerLocomotion locomotion)
    {
        this.locomotion = locomotion;
    }

    public bool ShouldTransition()
    {
        return Input.GetKeyDown(KeyCode.LeftShift); // ¿¹½Ã
    }

    public LocomotionStateBase GetNextState()
    {
        return new DodgeState(locomotion);
    }
}