using UnityEngine;
using DUS.Player.Locomotion;

public class IdleState : LocomotionStrategyState
{
    public IdleState(PlayerCore playerCore) : base(playerCore) { }
    protected override LocomotionMainState DetermineStateType() => LocomotionMainState.Idle;

    //public override bool StopCheckTransitionToInProgress() => false; // 딱히 필요 없음
    protected override AniParmType SetAniParmType() => AniParmType.SetBool;
    public override void Enter()
    {
        base.Enter();

        //m_PlayerCore.SetCurrentMoveSpeed(0f);
        //m_Locomotion.InitializeVelocity();
        //if(m_Locomotion.m_IsGrounded) m_PlayerCore.SetRigidVelocity(Vector3.zero);
    }

    public override void FixedUpdate()
    {
        // 1. UpdateMovement 동작
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();
        bool isCrouch = m_PlayerCore.m_InputManager.m_IsCrouch_LocoF;
        bool isJump = m_PlayerCore.m_InputManager.m_IsJump_LocoM;
        bool isMove = m_PlayerCore.m_InputManager.m_IsMove_LocoM;

        HandleCheckFlags(LocomotionSubFlags.Crouch, isCrouch);

        // Main
        if (isJump)
        {
            m_Locomotion.SetNextState(LocomotionMainState.Jump);
        }
        else if (isMove)
        {
            m_Locomotion.SetNextState(LocomotionMainState.Move);
        }
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void UpdateMovement()
    {
        m_Locomotion.HandleRotation();
        //if(!m_Locomotion.m_IsGrounded) m_Locomotion.HandleGravityMovement();
    }

}