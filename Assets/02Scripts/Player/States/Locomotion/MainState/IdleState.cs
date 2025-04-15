using UnityEngine;

public class IdleState : LocomotionBaseState
{
    public IdleState(PlayerCore playerCore) : base(playerCore){}

    //현재 상태 정의
    protected override LocomotionMainState GetMainState() => LocomotionMainState.Idle;
    //현재 애니메이션
    protected override string SetAnimationBoolName() => AniKeys.Idle;

    public override void Enter()
    {
        base.Enter();
        Debug.Log("IdleState Enter");
    }

    public override void Update()
    {
        base.Update();

    }

    public override void Exit()
    {
        base.Exit();
    }
}