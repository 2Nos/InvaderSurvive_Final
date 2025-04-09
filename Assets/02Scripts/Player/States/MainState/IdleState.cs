using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerLocomotion locomotion) : base(locomotion){}
    protected override string AnimationBoolName => AnimKeys.Idle;
    public override void Enter()
    {
        base.Enter();
        m_flagManager.LocomotionMain = LocomotionMainState.Idle;
    }

    public override void Update()
    {
        base.Update();
        HandleMovement();
    }

    public override void Exit()
    {
        base.Exit();
    }

}
