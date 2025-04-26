using UnityEngine;

public class IdleState : LocomotionBaseState
{
    public IdleState(PlayerCore playerCore) : base(playerCore) { }
    public override LocomotionMainState DetermineStateType() => LocomotionMainState.Idle;

    //public override bool StopCheckTransitionToInProgress() => false; // 딱히 필요 없음
    public override AniParmType SetAniParmType() => AniParmType.SetBool;
    public override void Enter()
    {
        base.Enter();

        m_PlayerCore.SetCurrentMoveSpeed(0f);
        m_PlayerCore.SetRigidVelocity(Vector3.zero);
        InitializeIdle();
    }

    public override void FixedUpdate()
    {
        // 1. Movement 동작
        base.FixedUpdate();
    }

    public override void Update()
    {
        //Flags
        if (m_PlayerCore.m_InputManager.m_IsCrouch_LocoF)
        {
            m_Locomotion.SetLocomotionFlag(LocomotionSubFlags.Crouch);
        }
        else if(!m_PlayerCore.m_InputManager.m_IsCrouch_LocoF)
        {
            m_Locomotion.ClearLocomotionFlag(LocomotionSubFlags.Crouch);
        }

        // Main
        if(m_PlayerCore.m_InputManager.m_IsJump_LocoM)
        {
            m_Locomotion.ChangeMainState(LocomotionMainState.Jump);
        }
        else if (m_PlayerCore.m_InputManager.m_IsMove_LocoM)
        {
            m_Locomotion.ChangeMainState(LocomotionMainState.Move);
        }

        // 애니메이션 및 변환 상태 생성
        base.Update();
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void Movement()
    {
        m_Locomotion.HandleRotation();
        if(!m_Locomotion.m_IsGrounded) m_Locomotion.HandleGravityMovement();
    }

}