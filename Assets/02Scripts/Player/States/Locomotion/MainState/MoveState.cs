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
        // 1. UpdateMovement ����
        base.FixedUpdate();
    }
    public override void Update()
    {
        // �ִϸ��̼� �� ��ȯ ���� ����
        base.Update();

        //Flags && Speed ����
        bool isRunning = m_PlayerCore.m_InputManager.m_IsRun_LocoF;
        bool isCrouching = m_PlayerCore.m_InputManager.m_IsCrouch_LocoF;
        bool isMoving = m_PlayerCore.m_InputManager.m_IsMove_LocoM; // �̵� �Է� ����
        bool isJumping = m_PlayerCore.m_InputManager.m_IsJump_LocoM;

        // 3. 5�� �Ǳ����� ���� SubFlag ���� üũ
        bool hasRun = m_Locomotion.m_StateUtility.HasLocomotionFlag(LocomotionSubFlags.Run);
        bool hasCrouch = m_Locomotion.m_StateUtility.HasLocomotionFlag(LocomotionSubFlags.Crouch);
        bool hasCrouchRun = m_Locomotion.m_StateUtility.HasLocomotionFlag(LocomotionSubFlags.CrouchRun);

        // 4. �� ���� ���� 
        if (!hasCrouch && hasRun && isCrouching)             // �޸��� �� �ɱ� = �����̵�
        {
            // �����̵� ����
            m_Locomotion.SetNextState(LocomotionMainState.Slide);
            HandleCheckFlags(LocomotionSubFlags.Crouch, isCrouching);
            return;
        }

        if (isRunning && isCrouching)       // �ɱ� �� �޸��� = ������ �޸���
        {
            m_PlayerCore.SetCurrentMoveSpeed(m_PlayerCore.m_crouchRunSpeed);
            hasCrouchRun = true;
        }
        else if (isRunning)          // �Ϲ� �޸���
        {
            m_PlayerCore.SetCurrentMoveSpeed(m_PlayerCore.m_runSpeed);
            hasCrouchRun = false;
        }
        else if (isCrouching)       // �Ϲ� �ɱ�
        {
            m_PlayerCore.SetCurrentMoveSpeed(m_PlayerCore.m_crouchSpeed);
            hasCrouchRun = false;
        }
        else
        {
            m_PlayerCore.SetCurrentMoveSpeed(m_PlayerCore.m_walkSpeed);
            hasCrouchRun = false;
        }

        // 5. SubFlag ������Ʈ
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