//���ͽ��� �帣�� ��� ���� �̵��� ����
//Movment ����

using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;

public class PlayerLocomotion
{
    public PlayerCore m_PlayerCore;
    public LocomotionBaseState m_currentState;
    public LocomotionBaseState m_prevState;
    // Move ����
    public float m_currentVelocityY { get; private set; }
    private Vector2 m_currentAnimInput = Vector2.zero;

    public bool m_IsGrounded { get; private set; }
    private float m_IgnoreGroundCheckTime = 0.3f; //�Ϻ� ���¿��� �� üũ�� �����ϱ� ���� �ð�
    private float m_GroundCheckTimer = 0f; // Ÿ�̸�

    public bool m_IsProgress = false;
    //������ - PlayerCore���� New�� ����
    public PlayerLocomotion(PlayerCore core)
    {
        m_PlayerCore = core;
        m_currentState = new IdleState(this);

        //m_PlayerLocomotion.m_locomotion�� ���� �������� �ʾ����Ƿ� PlayerCore���� ���� �� Enter()�� ȣ���ؾ���
        //m_currentState.Enter();
    }

    //�ִϸ��̼� �� ���¸� Dictionary�� �����ϸ� ����
    public Dictionary<LocomotionMainState, string> m_locomotionMainAniMap = new()
    {
        { LocomotionMainState.Idle, "IsIdle" },
        { LocomotionMainState.Move, "IsMove" },
        { LocomotionMainState.InAir, "IsInAir" },
        { LocomotionMainState.Land, "IsLand" },
        { LocomotionMainState.Slide, "IsSlide" },
        { LocomotionMainState.Climb, "IsClimb" },
        { LocomotionMainState.WallRun, "IsWallRun" }
    };

    public Dictionary<LocomotionSubFlags, string> m_locomotionFlagAniMap = new()
    {
        { LocomotionSubFlags.Run, "IsRun" },
        { LocomotionSubFlags.Crouch, "IsCrouch" },
    };

    /// <summary>
    /// PlayerCore���� Start���� ȣ��
    /// </summary>
    public void InitState()
    {
        m_currentState.Enter();
        m_IsGrounded = true;
    }

    public void Update()
    {
        m_currentState?.Update();
        if (m_prevState == m_currentState.GetCheckTransition()) return;
        LocomotionBaseState newState = m_currentState.GetCheckTransition();

        ChangeState(newState);
    }

    public void ChangeState(LocomotionBaseState newState)
    {
        //LocomotionBaseState���� ����������Locomotion�� ���������� ����
        m_currentState?.Exit();
        m_currentState = newState;
        m_currentState?.Enter();
        m_prevState = m_currentState;
    }

    #region ======================================== Move & Rotation

    public void HandleMove()
    {
        Vector3 m_moveDirection = Vector3.zero;
        float m_currentMoveSpeed = m_PlayerCore.m_moveSpeed;

        if (!m_PlayerCore.m_InputManager.m_IsMove_LocoM)
        {
            m_moveDirection = Vector3.zero;
            m_currentMoveSpeed = 0f;
            m_PlayerCore.m_Rigidbody.linearVelocity = Vector3.zero;
        }

        var prevMoveSpeed = m_currentMoveSpeed;

        // �̵� �ӵ� ����
        if (m_PlayerCore.m_InputManager.m_IsRun_LocoF)
        {
            if (m_PlayerCore.m_InputManager.m_IsCrouch_LocoF) m_currentMoveSpeed = m_PlayerCore.m_crouchRunSpeed;
            else m_currentMoveSpeed = m_PlayerCore.m_runSpeed;
        }
        else if (m_PlayerCore.m_InputManager.m_IsCrouch_LocoF) m_currentMoveSpeed = m_PlayerCore.m_crouchSpeed;

        // �̵� ���� ���
        Vector2 moveInput = m_PlayerCore.m_InputManager.m_MovementInput;
        m_currentAnimInput = Vector2.Lerp(m_currentAnimInput, moveInput, Time.deltaTime * 10f); //�ִϸ��̼� ���� ���� ������ ����

        //���� ����(transform�� ���� �������� ������ ���� �� ��ֶ�����)
        m_moveDirection = Vector3.Normalize(m_PlayerCore.transform.right * moveInput.x + m_PlayerCore.transform.forward * moveInput.y);    
        
        Vector3 velocity = m_moveDirection * m_currentMoveSpeed; //�ӵ��� �ΰ����� ������ ��/�� �׸��� ���� ���� ����Ͽ� �������� ��ģ��(�̷��� �ؾ� ������ currentVelocityY�� ���� ����� �� ����)

        m_PlayerCore.m_Rigidbody.MovePosition(m_PlayerCore.transform.position + velocity * Time.deltaTime); //�̵����� Rigidbody�� ����

        //�̵� �ִϸ��̼�
        m_PlayerCore.m_AnimationManager.UpdateMovementAnimation(m_currentAnimInput);
    }

    public void UpdateVerticalMovement()
    {
        RaycastHit hit;
        
        if (m_GroundCheckTimer > 0)
        {
            m_GroundCheckTimer -= Time.deltaTime;
        }
        else if (m_GroundCheckTimer <= 0)
        {
            if (Physics.Raycast(m_PlayerCore.transform.position, Vector3.down, out hit, 100f, m_PlayerCore.m_GroundMask))
            {
                Debug.Log(hit.distance);
                if (hit.distance <= 0.1f)
                {
                    m_currentVelocityY = 0f;
                    m_IsGrounded = true;
                    Debug.Log("Grounded");
                }
                else
                {
                    m_currentVelocityY -= 30f * Time.deltaTime; //�߷°�
                    m_IsGrounded = false;
                }
            }
        }
        Vector3 velocity = Vector3.up * m_currentVelocityY;

        m_PlayerCore.m_Rigidbody.MovePosition(m_PlayerCore.m_Rigidbody.position + velocity * Time.deltaTime);
    }

    // ȸ�� ó��
    public void HandleRotation()
    {
        // ȭ�� ȸ�� ���� ��
        if (m_PlayerCore.m_InputManager.m_IsStopCameraRot) return;
        
        // ���� �÷��̾� ����
        Quaternion currentRotation = m_PlayerCore.m_Rigidbody.transform.rotation;

        // �⺻������ �ٶ� ������ �÷��̾��� ����
        Vector3 targetForward = m_PlayerCore.m_Rigidbody.transform.forward;

        // ���� ���� ���, ī�޶� ����(���� ��ġ)���� ȸ��
        if (m_PlayerCore.m_InputManager.m_IsAim)
        {
            //������ ���� ��ġ�� ������
            Vector3 aimPos = m_PlayerCore.m_CameraManager.UpdateAimTargetPos();
            aimPos.y = m_PlayerCore.transform.position.y; //ĳ���Ͱ� x,z��(��,�Ʒ���)�� ���ư��� �ʵ��� �����ϱ� ���� ���� �����ϰ� ����

            // ���� ��ġ���� ����
            Vector3 aimDirection = (aimPos - m_PlayerCore.transform.position).normalized;

            // ȸ���� �߻� ��
            if (aimDirection.sqrMagnitude > 0.01f)
            {
                targetForward = aimDirection;
            }
        }
        else // �⺻ ����
        {
            float cameraYaw = m_PlayerCore.m_CameraManager.m_MouseX;
            Quaternion targetRot = Quaternion.Euler(0f, cameraYaw, 0f);
            targetForward = targetRot * Vector3.forward;
        }
        
        //���� �ٶ� ��������� �����̼� ��, Slerp�� ���� ����
        Quaternion targetRotation = Quaternion.LookRotation(targetForward); 

        // �Ϲݻ��¿� ���� ���¿� ���� ȸ�� �ӵ� ����
        float rotationSpeed = m_PlayerCore.m_InputManager.m_IsAim ? m_PlayerCore.m_rotationSpeed : m_PlayerCore.m_rotationAimSpeed;

        float maxDegrees = rotationSpeed * Time.deltaTime * m_PlayerCore.m_rotationDamping;

        Quaternion smoothedRotation = Quaternion.Slerp(currentRotation, targetRotation, maxDegrees);

        m_PlayerCore.m_Rigidbody.MoveRotation(smoothedRotation);
    }
    #endregion ======================================== /Move & Rotation

    /// <summary>
    /// UpdateVerticalMovement()���� ���� ȣ���Ͽ� ����
    /// </summary>
    public void ExecuteJumpFromEnter()
    {
        m_currentVelocityY = 0f;
        m_PlayerCore.m_Rigidbody.AddForce(Vector3.up * m_PlayerCore.m_jumpForce, ForceMode.Impulse);
        m_IsGrounded = false;
        m_GroundCheckTimer = m_IgnoreGroundCheckTime;
    }

    //Locomotion�� SubFlags �ִϸ��̼� ����
    public void UpdateLocomotionFlagAnimation()
    {
        bool isCrouch = m_PlayerCore.m_StateFlagManager.HasLocomotionFlag(LocomotionSubFlags.Crouch);
        bool isRun = m_PlayerCore.m_StateFlagManager.HasLocomotionFlag(LocomotionSubFlags.Run);

        m_PlayerCore.m_AnimationManager.SetBool(m_locomotionFlagAniMap[LocomotionSubFlags.Crouch], isCrouch);
        m_PlayerCore.m_AnimationManager.SetBool(m_locomotionFlagAniMap[LocomotionSubFlags.Run], isRun);
    }

    //Locomotion�� MainState �ִϸ��̼� ���� - LocomotionBaseState���� ȣ��
    public void UpdateLocomotionMainStateAnimation(LocomotionMainState locomotionMainState, bool isPlay)
    {
        if (m_locomotionMainAniMap.ContainsKey(locomotionMainState))
        {
            m_PlayerCore.m_AnimationManager.SetBool(m_locomotionMainAniMap[locomotionMainState], isPlay);
        }
        else
        {
            Debug.LogError("�ִϸ��̼��� �������� �ʽ��ϴ�.");
        }
    }
}