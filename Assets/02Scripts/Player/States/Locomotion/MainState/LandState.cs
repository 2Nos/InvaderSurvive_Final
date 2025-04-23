using UnityEngine;

public class LandState : LocomotionBaseState
{
    public LandState(PlayerLocomotion playerLocomotion) : base(playerLocomotion) { }
    public override LocomotionMainState DetermineStateType() => LocomotionMainState.Idle;
    public override void Enter()
    {
        base.Enter();
        m_PlayerLocomotion.m_IsProgress = true;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    float m_LandTime = 0f;
    public override void Update()
    {
        //base.Update();
        //MoevY에서 체크 중
        if (m_PlayerLocomotion.m_IsGrounded)
        {
            m_LandTime += Time.deltaTime;
            if(m_LandTime >= 0.5f)
            {
                m_PlayerLocomotion.m_IsProgress = false;
                SetMainState(LocomotionMainState.Idle);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        m_PlayerLocomotion.m_IsProgress = false;
    }
}
