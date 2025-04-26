public class MoveState : LocomotionBaseState
{
    public MoveState(PlayerCore playerCore) : base(playerCore) { }

    public override LocomotionMainState DetermineStateType() => LocomotionMainState.Move;
    public override AniParmType SetAniParmType() => AniParmType.SetBool;
    public override void Enter()
    {
        base.Enter();
        m_PlayerCore.SetCurrentMoveSpeed(m_PlayerCore.m_walkSpeed);
    }
    public override void FixedUpdate()
    {
        // 1. Movement 동작
        base.FixedUpdate();
    }
    public override void Update()
    {
        // 애니메이션 및 변환 상태 생성
        base.Update();

        //Flags && Speed 관리
        m_PlayerCore.SetCurrentMoveSpeed(m_PlayerCore.m_walkSpeed);

        bool isRunning = m_PlayerCore.m_InputManager.m_IsRun_LocoF;
        bool isCrouching = m_PlayerCore.m_InputManager.m_IsCrouch_LocoF;
        bool isMoving = m_PlayerCore.m_InputManager.m_IsMove_LocoM; // 이동 입력 여부

        // 3. 현재 SubFlag 상태
        bool isCurrentlyRunning = m_Locomotion.HasLocomotionFlag(LocomotionSubFlags.Run);
        bool isCurrentlyCrouching = m_Locomotion.HasLocomotionFlag(LocomotionSubFlags.Crouch);
        bool isCurrentlyCrouchRunning = m_Locomotion.HasLocomotionFlag(LocomotionSubFlags.CrouchRun);

        // 4. 슬라이드 진입 조건 (달리기 중 앉기)
        if (isCurrentlyRunning && isCrouching)
        {
            // 슬라이드 시작
            m_Locomotion.ChangeMainState(LocomotionMainState.Slide);
            return;
        }

        // 5. SubFlag 상태 관리
        m_Locomotion.ClearAllLocomotionFlags(); // 일단 매프레임 리셋하고 다시 세팅하는 방식

        if (isCrouching)
        {
            m_Locomotion.SetLocomotionFlag(LocomotionSubFlags.Crouch);

            if (isRunning && isMoving)
            {
                m_Locomotion.SetLocomotionFlag(LocomotionSubFlags.CrouchRun);
                m_PlayerCore.SetCurrentMoveSpeed(m_PlayerCore.m_crouchRunSpeed);
            }
            else
            {
                m_PlayerCore.SetCurrentMoveSpeed(m_PlayerCore.m_crouchSpeed);
            }
        }
        else
        {
            if (isRunning && isMoving)
            {
                m_Locomotion.SetLocomotionFlag(LocomotionSubFlags.Run);
                m_PlayerCore.SetCurrentMoveSpeed(m_PlayerCore.m_runSpeed);
            }
            else if (isMoving)
            {
                m_PlayerCore.SetCurrentMoveSpeed(m_PlayerCore.m_walkSpeed);
            }
        }

        // ChangeMain
        if (m_PlayerCore.m_InputManager.m_IsJump_LocoM)
        {
            m_Locomotion.ChangeMainState(LocomotionMainState.Jump);
        }
        else if (!m_PlayerCore.m_InputManager.m_IsMove_LocoM)
        {
            m_Locomotion.ChangeMainState(LocomotionMainState.Idle);
        }

    }
    public override void Exit()
    {
        base.Exit();
        InitializeMove();
    }

    private void InitializeMove()
    {
        m_Locomotion.ClearAllLocomotionFlags();
    }

}