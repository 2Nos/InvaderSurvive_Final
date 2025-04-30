using Unity.Hierarchy;
using UnityEngine;
using System.Linq;
using DUS.Player.Locomotion;

public class JumpState : LocomotionStrategyState
{
    public JumpState(PlayerCore playerCore) : base(playerCore) { }
    protected override LocomotionMainState DetermineStateType() => LocomotionMainState.Jump;
    protected override AniParmType SetAniParmType() => AniParmType.SetTrigger;

    private float m_JumpTimer;

    public override void Enter()
    {
        base.Enter();

        m_PlayerCore.SetRigidVelocity(m_Locomotion.m_CurrentVelocity/2);
        m_PlayerCore.SetRigidVelocityY(0);
        m_JumpTimer = 0f;

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!m_PlayerCore.m_AnimationManager.m_IsJumpStart) return;

        UpdateJumpForce();
    }

    public override void Update()
    {
        base.Update();

    }

    public override void Exit()
    {
        base.Exit();
        m_PlayerCore.m_AnimationManager.m_IsJumpStart = false;
    }

    public override void UpdateMovement()
    {
        m_Locomotion.HandleRotation();
    }

    public void UpdateJumpForce()
    {
        // 자연스럽게 점프포스를 주는 계산
        m_JumpTimer += Time.fixedDeltaTime;
        float t = m_JumpTimer / m_PlayerCore.m_JumpDuration;

        if (t < 1f)
        {
            float force = m_PlayerCore.m_JumpForce * m_PlayerCore.m_JumpCurve.Evaluate(t);
            m_PlayerCore.SetRigidVelocityY(force);
            m_Locomotion.m_CurrentVelocityY = force;
        }
        else
        {
            m_Locomotion.SetNextState(LocomotionMainState.InAir);
        }
    }
}
