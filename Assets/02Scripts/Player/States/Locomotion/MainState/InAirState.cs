using UnityEngine;

public class InAirState : LocomotionBaseState
{
    public InAirState(PlayerLocomotion playerLocomotion) : base(playerLocomotion){}

    public override LocomotionMainState DetermineStateType() => LocomotionMainState.InAir;

    public override void Enter()
    {
        base.Enter();
        m_PlayerLocomotion.ExecuteJumpFromEnter();
        m_PlayerLocomotion.m_IsProgress = true;
        //m_PlayerLocomotion.m_AnimationManager.SetTrigger("Jump");
    }

    public override void Update()
    {
        base.Update();
        //MoevY에서 체크 중
        if (m_PlayerLocomotion.m_IsGrounded)
        {
            //m_PlayerLocomotion.m_IsProgress = false;
            SetMainState(LocomotionMainState.Land);
        }
    }
    public override void Exit()
    {
        base.Exit();

    }

    protected override void Movement()
    {
        m_PlayerLocomotion.HandleRotation();
        m_PlayerLocomotion.UpdateVerticalMovement();
    }
}
