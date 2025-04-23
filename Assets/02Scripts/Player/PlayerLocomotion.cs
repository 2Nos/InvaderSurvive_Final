//���ͽ��� �帣�� ��� ���� �̵��� ����
//Movment ����

using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using Unity.Android.Gradle.Manifest;

public class PlayerLocomotion
{
    public PlayerCore m_PlayerCore;
    public LocomotionBaseState m_currentState;
    public LocomotionBaseState m_prevState;
    // Move ����
    private float m_currentVelocityY = 0;
    private Vector2 m_currentAnimInput = Vector2.zero;

    public bool m_IsGrounded { get; private set; }

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

    public void FixedUpdate()
    {
        m_currentState?.FixedUpdate();
    }
    public void Update()
    {
        m_currentState?.Update();
        Debug.Log(m_currentState);
        LocomotionBaseState newState = m_currentState.GetCheckTransition();

        ChangeState(newState);
    }

    public void ChangeState(LocomotionBaseState newState)
    {
        if(newState == null || newState == m_currentState) return; //null�� ���� ���°� ���ٴ� ��, �� Idle���·� ���ư�
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

    public void UpdateCheckInAir()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_PlayerCore.m_Rigidbody.position, -m_PlayerCore.transform.up, out hit, 10f, m_PlayerCore.m_GroundMask))
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

    }

    //���� �Լ� ���� InAir�� �̵�
    //�߷� ����ϰ� �����Ƿ� �ϴ��� �ּ�ó��
    private float m_IgnoreGroundCheckTime = 1f;
    private float m_GroundCheckTimer = 0f;
    /// <summary>
    /// ���� �� �߷� ó��
    /// </summary>
    public void UpdateVerticalMovement()
    {
        // TODO : ���� �� �浹 ó��

        /*// 1. Ground Check Ÿ�̸� ����
        if (m_GroundCheckTimer > 0f)
        {
            m_GroundCheckTimer -= Time.deltaTime;
        }
        if (m_GroundCheckTimer <= 0f)
        {
            RaycastHit hit;
            if (Physics.Raycast(m_PlayerCore.m_Rigidbody.position, -m_PlayerCore.transform.up, out hit, 10f, m_PlayerCore.m_GroundMask))
            {

                if (hit.distance <= 0.1f)
                {
                    if (m_IsGrounded) return;
                    m_currentVelocityY = 0f;
                    m_IsGrounded = true;

                    // �ٴ� �մ� �� ����
                    Vector3 fixedPos = m_PlayerCore.transform.position;
                    fixedPos.y = hit.point.y + 0.05f;
                    m_PlayerCore.m_Rigidbody.MovePosition(fixedPos);
                    return;
                }
                else
                {
                    m_IsGrounded = false;
                }
            }
        }
        // 3. �߷� ����
        if (!m_IsGrounded)
        {
            m_currentVelocityY += m_PlayerCore.m_Gravity * Time.deltaTime;

            //TODO : ���� �ӵ� ����
            m_currentVelocityY = Mathf.Clamp(m_currentVelocityY, -m_PlayerCore.m_MaxFallingSpeed, m_PlayerCore.m_jumpForce);
        }

        // 4. �̵� ����
        Vector3 velocity = new Vector3(0, m_currentVelocityY, 0);
        m_PlayerCore.m_Rigidbody.MovePosition(m_PlayerCore.transform.position + velocity * Time.deltaTime);*/
    }

    /// <summary>
    /// JumpŰ ������ �� LocomotionBase�� CheckLocomotion���� ȣ��;
    /// </summary>
    public void ExecuteJump()
    {
        m_PlayerCore.m_Rigidbody.AddForce(m_PlayerCore.transform.up * m_PlayerCore.m_jumpForce, ForceMode.Impulse); //���� ���� �ֱ� ���� Rigidbody�� ���� ��
        //m_currentVelocityY = m_PlayerCore.m_jumpForce;
        //m_GroundCheckTimer = m_IgnoreGroundCheckTime; // ���� ���� �ٴ� üũ ����!
    }

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
        //TODO : �ִϸ��̼� ������ ���� Enum�� ���� ���� ����
        /*switch (setType)
                {
                    case SetType.SetBool:
                        m_PlayerCore.m_AnimationManager.SetBool(m_locomotionMainAniMap[locomotionMainState], isPlay);
                        break;
                    case SetType.SetTrigger:
                        m_PlayerCore.m_AnimationManager.SetTrigger(m_locomotionMainAniMap[locomotionMainState]);
                        break;
                    case SetType.SetFloat:
                        break;
                    default:
                        break;
                }*/
        if (m_locomotionMainAniMap.ContainsKey(locomotionMainState))
        {
            m_PlayerCore.m_AnimationManager.SetBool(m_locomotionMainAniMap[locomotionMainState], isPlay);

            //SetTrigger�� ����
            if (locomotionMainState == LocomotionMainState.InAir)
            {
                if (!isPlay) return;
                m_PlayerCore.m_AnimationManager.SetTrigger("IsJump");
            }
        }
        else
        {
            Debug.LogError("�ִϸ��̼��� �������� �ʽ��ϴ�.");
        }

        
    }
}