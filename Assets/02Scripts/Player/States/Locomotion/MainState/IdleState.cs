using UnityEngine;

public class IdleState : LocomotionBaseState
{
    public IdleState(PlayerCore playerCore) : base(playerCore){}

    //���� ���� ����
    protected override LocomotionMainState GetMainState() => LocomotionMainState.Idle;
    //���� �ִϸ��̼�
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