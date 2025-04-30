using DUS.Player.Locomotion;

public class InAirState : LocomotionStrategyState
{
    public InAirState(PlayerCore playerCore) : base(playerCore) { }
    protected override LocomotionMainState DetermineStateType() => LocomotionMainState.InAir;

    protected override AniParmType SetAniParmType() => AniParmType.SetBool;

    public override void Enter()
    {
        base.Enter();
        //TODO : 애니메이션에서 Trigger 설정의 경우 절벽에서 낙하 시 공중상태도 있기에 차후 조건 적용 필요
        //일단은 점프 상태에서의 공중만 적용
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