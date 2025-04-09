using UnityEngine;

public class WallRunState : PlayerState
{
    public WallRunState(PlayerLocomotion locomotion) : base(locomotion) { }
    protected override string AnimationBoolName => AnimKeys.WallRun;
    public override void Enter()
    {
        base.Enter();
        m_flagManager.LocomotionMain = LocomotionMainState.WallRunning;
    }

    public override void Exit()
    {
        base.Exit();
    }

    protected override Transition[] DefaultTransitions => new[]
    {
        Transition.IsInAir,
        Transition.IsSliding,
        Transition.IsMoving
    };
}
