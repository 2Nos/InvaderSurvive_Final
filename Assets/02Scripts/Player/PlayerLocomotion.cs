//���ͽ��� �帣�� ��� ���� �̵��� ����
//Movment ����

using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

//TODO : ���� ���� ������ ��� LocomotionStateInfo ���Ϸ� �̵��� ����
#region ======================================== MainState And Flags
/// <summary>
/// ���� �̵� ����
/// �׻� ����(MainState���� �ϳ���, Flags ����x) ���·� ���� + Flags ������ ����
/// Move + m_LocomotionFlags(FalgButtonGroupManager + Aming)
/// </summary>
public enum LocomotionMainState
{
    Idle = 0,           // Idle
    Move = 1,           // �⺻ �̵�
    Jump = 2,          // ����
    InAir = 3,          // ���� (����/����)
    Land = 4,           // ����
    Dodge = 5,          // ������(ȸ�Ǳ�)
    Slide = 6,          // �����̵�
    Climb = 7,          // ���
    WallRun = 8,         // �� �޸���
    Staggered = 9,         // �ǰ� ���� ���� ����
    Knockback = 10,         // �˹�
}

/// <summary>
/// Flags�� ������ ������ ������ ������ ���. ��, ���� ���°� ����
/// Flags�� 2�� �������� 2�� ������ ������ ����Ͽ� �����ؾ���
/// None =0, FalgButtonGroupManager = 1, Croucning = 2, 4, 8 �̷��� ���ٴ� ����Ʈ����)
/// </summary>
[Flags]
public enum LocomotionSubFlags
{
    None = 0,
    Run = 1 << 1,           // �޸���
    Crouch = 1 << 2,        // �ɱ�
    CrouchRun = 1 << 3      // �ɾƼ� �޸��� 
}

//TODO: ���߿� �ʿ����� �𸣴� ���ܵ�
/*public ActionStateFlags m_ActionFlags
{
   get => m_actionFlags;
   set => m_actionFlags = value;
}
/// <summary>
/// �������� ��� ���¿� ������ ���� ���´� �̷��� ���� ����
/// </summary>
[Flags]
public enum ActionStateFlags
{
   None = 0,
   Dodge = 1 << 0,           // ������(ȸ�Ǳ�)
   Staggered = 1 << 1,         // �ǰ� ���� ���� ����
   Knockback = 1 << 2          // �˹�
}*/
#endregion ======================================== /MainState And Flags

public class PlayerLocomotion
{
    #region ======================================== Module
    private PlayerCore m_playerCore;
    public PlayerLocomotion(PlayerCore playerCore)
    {
        m_playerCore = playerCore;
    }
    #endregion ======================================== /Module


    #region ======================================== GetSetMain&Flags
    // Main State (���� ����)
    //private LocomotionMainState m_CurrentMainState { get; set; } = LocomotionMainState.Idle; // �� State���� DoTransitionState()�Լ��� ���� ��ȯ
    //private LocomotionMainState m_PrevMainState { get; set; } = LocomotionMainState.Idle;

    // Flags (������ ���� ����)
    private LocomotionSubFlags m_LocomotionFlags { get; set; } = LocomotionSubFlags.None;
    public void SetLocomotionFlag(LocomotionSubFlags flag) => m_LocomotionFlags |= flag;      // �ش� ���·� ����
    public bool HasLocomotionFlag(LocomotionSubFlags flag) => (m_LocomotionFlags & flag) != 0;
    public void ClearLocomotionFlag(LocomotionSubFlags flag) => m_LocomotionFlags &= ~flag;   // ���� ���� ����
    public void ClearAllLocomotionFlags() => m_LocomotionFlags = LocomotionSubFlags.None; // ��� ���� ����

    //TODO: ���߿� �ʿ����� �𸣴� ���ܵ� (Dodge���� �׼ǵ��� ���� Ȯ�强�� ���� �з� �ʿ������� ��)
    /*#region ======================================== Action
   private ActionStateFlags m_actionFlags = ActionStateFlags.None;
   public void SetActionStateFlags(ActionStateFlags flag) => m_actionFlags |= flag;
   public bool HasActionStateFlags(ActionStateFlags flag) => (m_actionFlags & flag) != 0;
   public void ClearActionStateFlags(ActionStateFlags flag) => m_actionFlags &= ~flag;
   #endregion ======================================== /Action*/
    #endregion ======================================== /GetSetFlags

    #region ======================================== Dictionary Map
    // �޸� ������ ���� ���� ĳ��, ���� ��ȯ �� ���(�̸� new�� ���� �ʴ� ������ static ������ �����Ͽ� m_PlayerCore�� �Ѱ������)
    public Dictionary<LocomotionMainState, LocomotionBaseState> m_MainStateMap = new();
    /*{
        {LocomotionMainState.Idle, new IdleState() },
        {LocomotionMainState.Move, new MoveState() },
        {LocomotionMainState.Jump, new JumpState() },
        {LocomotionMainState.InAir, new InAirState() },
        {LocomotionMainState.Land, new LandState() },
        {LocomotionMainState.Slide, new SlideState() },
        {LocomotionMainState.Climb, new ClimbState() },
        {LocomotionMainState.WallRun, new WallRunState() }
        {LocomotionMainState.Dodge, new DodgeState(m_playerCore) },
        {LocomotionMainState.Staggered, new StaggeredState(m_playerCore) },
        {LocomotionMainState.Knockback, new KnockbackState(m_playerCore) },
    };*/
    public Dictionary<LocomotionSubFlags, string> m_locomotionFlagAniMap = new()
    {
        { LocomotionSubFlags.Run, "IsRun" },
        { LocomotionSubFlags.Crouch, "IsCrouch" },
        { LocomotionSubFlags.CrouchRun, "IsCrouchRun" }
    };
    public Dictionary<LocomotionMainState, string> m_locomotionMainAniMap = new()
    {
        { LocomotionMainState.Idle, "IsIdle" },
        { LocomotionMainState.Move, "IsMove" },
        { LocomotionMainState.Jump, "IsJump" },
        { LocomotionMainState.InAir, "IsInAir" },
        { LocomotionMainState.Land, "IsLand" },
        { LocomotionMainState.Slide, "IsSlide" },
        { LocomotionMainState.Climb, "IsClimb" },
        { LocomotionMainState.WallRun, "IsWallRun" }
    };
    #endregion ======================================== /�ִϸ��̼� Map

    #region ======================================== State ����
    private LocomotionBaseState m_currentState;
    private LocomotionBaseState m_changeMainState;
    public LocomotionBaseState m_prevState { get; private set; }
    #endregion ======================================== /State ����

    // Move ����
    private Vector2 m_currentAniInput = Vector2.zero;
    private float m_currentVelocityY = 0;
    private float m_prevSpeed = 0;

    public bool m_IsGrounded { get; private set; }
    public bool m_IsNoCheckGround { get; private set; } = false; //�߷� ���� ����
    //public bool m_IsInAir { get; private set; } = false; //���� �������� üũ
    //public bool m_IsLand { get; private set; } = false; //���� �������� üũ

    //private Dictionary<LocomotionMainState, LocomotionBaseState> m_MainStateMap = new();

    // PlayerCore���� Start���� ȣ��
    public void InitializeLocomotion()
    {
        // ���� ��ü�� ������ �� PlayerCore�� ����
        m_MainStateMap[LocomotionMainState.Idle] = new IdleState(m_playerCore);
        m_MainStateMap[LocomotionMainState.Move] = new MoveState(m_playerCore);
        m_MainStateMap[LocomotionMainState.Jump] = new JumpState(m_playerCore);
        m_MainStateMap[LocomotionMainState.InAir] = new InAirState(m_playerCore);
        m_MainStateMap[LocomotionMainState.Land] = new LandState(m_playerCore);
        m_MainStateMap[LocomotionMainState.Slide] = new SlideState(m_playerCore);
        m_MainStateMap[LocomotionMainState.Climb] = new ClimbState(m_playerCore);
        m_MainStateMap[LocomotionMainState.WallRun] = new WallRunState(m_playerCore);

        m_currentState = m_MainStateMap[LocomotionMainState.Idle]; //�ʱ�ȭ ���´� Idle�� ����
        m_currentState.Enter();
    }

    public void FixedUpdate()
    {
        m_currentState?.FixedUpdate();
    }

    public void Update()
    {
        UpdateCheckGround();

        m_currentState?.Update();
        LocomotionBaseState newState = m_changeMainState;
        SwithcCurrentState(newState);

       
    }

    /*public void DoTransitionState(LocomotionMainState locomotionMainState)
    {
        m_ChangeCreateState = Create(locomotionMainState); //�̰ɷ� Update���� ���¸� üũ��
    }*/

    #region ======================================== State ����
    public void SwithcCurrentState(LocomotionBaseState newState)
    {
        if (newState == m_prevState) return; 
        
        m_currentState?.Exit();
        m_prevState = m_currentState; //���� ���� ����
        m_currentState = newState;
        m_currentState?.Enter();
        Debug.Log($"Current State: {m_currentState}");
    }

    //�� ���¿��� ȣ��
    public void ChangeMainState(LocomotionMainState state)
    {
        m_changeMainState = m_MainStateMap[state];
    }

    #endregion ======================================== /���� ��ȯ ����

    #region ======================================== Movement ����

    /// <summary>
    /// ���� üũ
    /// </summary>
    public void UpdateCheckGround()
    {
        if(m_IsNoCheckGround) return; //�߷� ���� ������ ���

        Vector3 centerRay = m_playerCore.transform.position;
        Vector3 forwardRay = m_playerCore.transform.position + m_playerCore.transform.forward * 0.3f + m_playerCore.transform.up * 0.3f;
        Vector3 rayDir = -m_playerCore.transform.up;

        Debug.DrawRay(centerRay, rayDir * 10f, Color.red, 0.1f);
        Debug.DrawRay(forwardRay, rayDir * 10f, Color.red, 0.1f);

        // ����� ���� ���� üũ
        RaycastHit hit;
        if (Physics.Raycast(centerRay, rayDir, out hit, 10f, m_playerCore.m_GroundMask))
        {
            if (hit.distance <= 0.1f)
            {
                m_IsGrounded = true;
            }
            else
            {
                m_IsGrounded = false;
            }
        }

        //TODO : õ�� üũ
        //TODO : ��� �� �������� üũ
    }

    /// <summary>
    /// FixedUpdate���� ȣ��
    /// �̵�, ȸ��, �߷�, ������ �� ���¿��� ȣ��
    /// </summary>
    public void HandleMove()
    {
        // 1.�ʱ�ȭ
        Vector2 moveInput = m_playerCore.m_InputManager.m_MovementInput;    //�Է°�
        Vector3 m_moveDirection = m_playerCore.transform.right * moveInput.x + m_playerCore.transform.forward * moveInput.y; //����
        m_moveDirection.Normalize(); //����ȭ

        // //�ִϸ��̼� ���� Ʈ������ ���� ����(Input 0.7, 1 �̷������� �����⿡ ����� ���� ����)
        m_currentAniInput = Vector2.Lerp(m_currentAniInput, moveInput, Time.deltaTime * 10f);

        // 3. �̵� ���� ��
        // 3.1. �̵� �ӵ� ������ �� ���¿��� Set�� ����(���ǵ� ������ ���� �����)
        float currentSpeed = m_playerCore.m_CurrentSpeed;
        
        currentSpeed = Mathf.Lerp(m_prevSpeed, currentSpeed, Time.deltaTime * 10f); //���� ���ǵ忡�� ���� ���ǵ�� ����
        m_prevSpeed = currentSpeed;
        Vector3 velocity = m_moveDirection * currentSpeed;

        // 3.2 �̵��� ���� (�̷����� ������ �ܼ� ��ǥ���ٴ� ���������̿�)
        m_playerCore.SetRigidVelocity(velocity);

        //�̵� �ִϸ��̼�
        m_playerCore.m_AnimationManager.UpdateMovementAnimation(m_currentAniInput);
    }

    public void HandleRotation()
    {
        // ȭ�� ȸ�� ���� ��
        if (m_playerCore.m_InputManager.m_IsStopBodyRot) return;

        // ���� �÷��̾� ����
        Quaternion currentRotation = m_playerCore.m_Rigidbody.transform.rotation;

        // �⺻������ �ٶ� ������ �÷��̾��� ����
        Vector3 targetForward = m_playerCore.m_Rigidbody.transform.forward;

        // ���� ���� ���, ī�޶� ����(���� ��ġ)���� ȸ��
        
        if (m_playerCore.m_InputManager.m_IsAim)
        {
            //������ ���� ��ġ�� ������
            Vector3 aimPos = m_playerCore.m_CameraManager.UpdateAimTargetPos();
            aimPos.y = m_playerCore.transform.position.y; //ĳ���Ͱ� x,z��(��,�Ʒ���)�� ���ư��� �ʵ��� �����ϱ� ���� ���� �����ϰ� ����

            // ���� ��ġ���� ����
            Vector3 aimDirection = (aimPos - m_playerCore.transform.position).normalized;

            // ȸ���� �߻� ��
            if (aimDirection.sqrMagnitude > 0.01f)
            {
                targetForward = aimDirection;
            }
        }
        else // �⺻ ����
        {
            float cameraYaw = m_playerCore.m_CameraManager.m_MouseX;
            Quaternion targetRot = Quaternion.Euler(0f, cameraYaw, 0f);
            targetForward = targetRot * Vector3.forward;
        }

        //���� �ٶ� ��������� �����̼� ��, Slerp�� ���� ����
        Quaternion targetRotation = Quaternion.LookRotation(targetForward);

        // �Ϲݻ��¿� ���� ���¿� ���� ȸ�� �ӵ� ����
        float rotationSpeed = m_playerCore.m_CurrentRotSpeed; //Aim ���¿��� ���ǵ� �ٷ��

        // ȸ�� �ӵ� ����
        //float maxDegrees = rotationSpeed * Time.deltaTime * m_playerCore.m_rotationDamping; //m_rotationDamping : ���� �ִ°�

        Quaternion smoothedRotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed);

        m_playerCore.m_Rigidbody.MoveRotation(smoothedRotation);
    }

    private bool m_NotGravity; //�߷� ���� ����

    public void HandleGravityMovement()
    {
        //�߷� ���� ������ ���
        if (m_NotGravity || m_IsGrounded) return;

        // TODO : ���� �� �浹 ó��
        // 1. �߷°� ���
        m_currentVelocityY += m_playerCore.m_Gravity * Time.deltaTime;

        // 2. ���Ͻ� �ӵ� ����
        m_currentVelocityY = Mathf.Clamp(m_currentVelocityY, m_playerCore.m_MinVelocityY, m_playerCore.m_MaxVelocityY);

        // 3. �߷� ����
        m_playerCore.SetRigidVelocityY(m_currentVelocityY);
    }


    #endregion ======================================== /Move & Rotation

    #region ======================================== Animation ����

    //Enter�� Exit���� �ѹ��� �ҷ��͵� �Ǵ� �ִϸ��̼�, ���� �ϳ��� �޾ƿ��� ���̽��� ���
    public void SetMainStateAnimation(LocomotionMainState locomotionMainState, AniParmType setAniParmType, bool isPlay, float value = 0)
    {
        //TODO : �ִϸ��̼� ������ ���� Enum�� ���� ���� ����
        switch(setAniParmType)
        {
            case AniParmType.None:
                break;
            case AniParmType.SetBool:
                m_playerCore.m_AnimationManager.SetParmBool(m_locomotionMainAniMap[locomotionMainState], isPlay);
                break;
            case AniParmType.SetTrigger:
                m_playerCore.m_AnimationManager.SetParmTrigger(m_locomotionMainAniMap[locomotionMainState]);
                break;
            /*case AniParmType.SetInt:
                m_playerCore.m_AnimationManager.SetParmInt(m_locomotionMainAniMap[locomotionMainState], (int)value);
                break;
            case AniParmType.SetFloat:
                m_playerCore.m_AnimationManager.SetParmFloat(m_locomotionMainAniMap[locomotionMainState], value);
                break;*/
            default:
                break;
        }

    }
    //Locomotion�� SubFlags �ִϸ��̼� ����
    public virtual void UpdateLocomotionFlagAnimation()
    {
        bool isCrouch = HasLocomotionFlag(LocomotionSubFlags.Crouch);
        bool isRun = HasLocomotionFlag(LocomotionSubFlags.Run);
        bool isCrouchRun = HasLocomotionFlag(LocomotionSubFlags.CrouchRun);

        m_playerCore.m_AnimationManager.SetParmBool(m_locomotionFlagAniMap[LocomotionSubFlags.Crouch], isCrouch);
        m_playerCore.m_AnimationManager.SetParmBool(m_locomotionFlagAniMap[LocomotionSubFlags.Run], isRun);
        m_playerCore.m_AnimationManager.SetParmBool(m_locomotionFlagAniMap[LocomotionSubFlags.CrouchRun], isCrouchRun);
    }

    /*public void PlayMainStateAnimation(LocomotionMainState locomotionMainState, bool isPlay)
    {
        if (m_MainStateMap.ContainsKey(locomotionMainState))
        {
            m_playerCore.m_AnimationManager.SetParmBool(m_locomotionMainAniMap[locomotionMainState], isPlay);
        }
        else Debug.LogError("�ִϸ��̼� �̸��� �߸��Ǿ����ϴ�: ");
    }*/
    #endregion ======================================== /�ִϸ��̼� ȣ��
}