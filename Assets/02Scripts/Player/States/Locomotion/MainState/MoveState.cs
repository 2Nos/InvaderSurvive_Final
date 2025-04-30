using DUS.Player.Locomotion;

public class MoveState : LocomotionStrategyState
{
    public MoveState(PlayerCore playerCore) : base(playerCore) { }

    protected override LocomotionMainState DetermineStateType() => LocomotionMainState.Move;
    protected override AniParmType SetAniParmType() => AniParmType.SetBool;
    public override void Enter()
    {
        base.Enter();
        m_PlayerCore.SetCurrentMoveSpeed(m_PlayerCore.m_walkSpeed);

    }
    public override void FixedUpdate()
    {
        // 1. UpdateMovement 동작
        base.FixedUpdate();
    }
    public override void Update()
    {
        // 애니메이션 및 변환 상태 생성
        base.Update();

        //Flags && Speed 관리
        bool isRunning = m_PlayerCore.m_InputManager.m_IsRun_LocoF;
        bool isCrouching = m_PlayerCore.m_InputManager.m_IsCrouch_LocoF;
        bool isMoving = m_PlayerCore.m_InputManager.m_IsMove_LocoM; // 이동 입력 여부
        bool isJumping = m_PlayerCore.m_InputManager.m_IsJump_LocoM;

        // 3. 5가 되기전의 이전 SubFlag 상태 체크
        bool hasRun = m_Locomotion.m_StateUtility.HasLocomotionFlag(LocomotionSubFlags.Run);
        bool hasCrouch = m_Locomotion.m_StateUtility.HasLocomotionFlag(LocomotionSubFlags.Crouch);
        bool hasCrouchRun = m_Locomotion.m_StateUtility.HasLocomotionFlag(LocomotionSubFlags.CrouchRun);

        // 4. 각 진입 조건 
        if (!hasCrouch && hasRun && isCrouching)             // 달리기 중 앉기 = 슬라이드
        {
            // 슬라이드 시작
            m_Locomotion.SetNextState(LocomotionMainState.Slide);
            HandleCheckFlags(LocomotionSubFlags.Crouch, isCrouching);
            return;
        }

        if (isRunning && isCrouching)       // 앉기 중 달리기 = 앉으며 달리기
        {
            m_PlayerCore.SetCurrentMoveSpeed(m_PlayerCore.m_crouchRunSpeed);
            hasCrouchRun = true;
        }
        else if (isRunning)          // 일반 달리기
        {
            m_PlayerCore.SetCurrentMoveSpeed(m_PlayerCore.m_runSpeed);
            hasCrouchRun = false;
        }
        else if (isCrouching)       // 일반 앉기
        {
            m_PlayerCore.SetCurrentMoveSpeed(m_PlayerCore.m_crouchSpeed);
            hasCrouchRun = false;
        }
        else
        {
            m_PlayerCore.SetCurrentMoveSpeed(m_PlayerCore.m_walkSpeed);
            hasCrouchRun = false;
        }

        // 5. SubFlag 업데이트
        HandleCheckFlags(LocomotionSubFlags.CrouchRun, hasCrouchRun);
        HandleCheckFlags(LocomotionSubFlags.Run, isRunning);
        HandleCheckFlags(LocomotionSubFlags.Crouch, isCrouching);


        // ChangeMain
        if (isJumping)
        {
            m_Locomotion.SetNextState(LocomotionMainState.Jump);
        }
        else if (!isMoving)
        {
            m_Locomotion.SetNextState(LocomotionMainState.Idle);
        }

    }
    public override void Exit()
    {
        base.Exit();
        m_Locomotion.m_StateUtility.AllClearFlags(m_PlayerCore.m_AnimationManager.m_Animator);

    }
}