using DUS.Player.Locomotion;

public class InAirState : LocomotionStrategyState
{
    public InAirState(PlayerCore playerCore) : base(playerCore) { }
    protected override LocomotionMainState DetermineStateType() => LocomotionMainState.InAir;

    protected override AniParmType SetAniParmType() => AniParmType.SetBool;

    public override void Enter()
    {
        base.Enter();
        //TODO : �ִϸ��̼ǿ��� Trigger ������ ��� �������� ���� �� ���߻��µ� �ֱ⿡ ���� ���� ���� �ʿ�
        //�ϴ��� ���� ���¿����� ���߸� ����
        m_PlayerCore.SetRigidVelocityY(0f);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void Update()
    {
        base.Update();
        if(m_Locomotion.m_IsGrounded)
        {
            m_Locomotion.SetNextState(LocomotionMainState.Land);
        }
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void UpdateMovement()
    {
        //base.UpdateMovement();
        m_Locomotion.HandleRotation();
    }
}