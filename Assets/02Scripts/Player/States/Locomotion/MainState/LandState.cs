using UnityEngine;

public class LandState : LocomotionBaseState
{
    public LandState(PlayerLocomotion playerLocomotion) : base(playerLocomotion) { }
    public override LocomotionMainState DetermineStateType() => LocomotionMainState.Land;
    public override void Enter()
    {
        base.Enter();
    }

    float m_LandTime = 0f;
    public override void Update()
    {
        //MoevY에서 체크 중
        base.Update();
        m_LandTime += Time.deltaTime;
        if (m_LandTime >= 0.5f)
        {
            SetMainState(LocomotionMainState.Idle);
        }

    }

    public override void Exit()
    {
        base.Exit();
        m_PlayerLocomotion.m_IsProgress = false;
        m_LandTime = 0f;
    }

    protected override void Movement()
    {
        m_PlayerLocomotion.HandleRotation();
        //m_PlayerLocomotion.HandleMove();
        //m_PlayerLocomotion.UpdateGravityMovement();
    }
}
