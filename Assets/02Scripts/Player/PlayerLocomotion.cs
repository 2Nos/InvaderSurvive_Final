//Movment ����
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;

public class PlayerLocomotion
{
    public PlayerCore m_playerCore;
    private LocomotionBaseState m_currentState;
    
    // Move ����
    float m_speedSmoothVelocity;  //ref �̵����� ��ȭ �ӵ�
    float m_speedSmoothTime = 0.1f;  //�÷��̾��� ������ �ӵ��� ��ȭ�� �ε巴�� ���ִ� ������
    float m_currentVelocityY;
    private Vector2 m_currentAnimInput = Vector2.zero;

    public bool m_isGrounded { get; private set; }

    public bool m_GetIsGrounded() => m_isGrounded;

    //������ - PlayerCore���� New�� ����
    public PlayerLocomotion(PlayerCore core)
    {
        m_playerCore = core;
        m_currentState = new IdleState(m_playerCore);

        //m_playerCore.m_locomotion�� ���� �������� �ʾ����Ƿ� PlayerCore���� ���� �� Enter()�� ȣ���ؾ���
        //m_currentState.Enter();
    }

    //�ִϸ��̼� �� ���¸� Dictionary�� �����ϸ� ����
    public Dictionary<LocomotionMainState, string> m_locomotionMainAniMap = new()
    {
        { LocomotionMainState.Idle, "IsIdle" },
        { LocomotionMainState.Move, "IsMove" },
        { LocomotionMainState.Slide, "IsSlide" },
        { LocomotionMainState.InAir, "IsInAir" },
        { LocomotionMainState.Climb, "IsClimb" },
        { LocomotionMainState.WallRun, "IsWallRun" }
    };

    public Dictionary<LocomotionSubFlags, string> m_locomotionFlagAniMap = new()
    {
        { LocomotionSubFlags.Run, "IsRun" },
        { LocomotionSubFlags.Crouch, "IsCrouch" },
    };

    public void InitState()
    {
        m_currentState.Enter();
    }

    public void Update()
    {
        m_currentState?.Update();

        LocomotionBaseState newState = m_currentState.GetCheckTransition();
        ChangeState(newState);
    }

    public void ChangeState(LocomotionBaseState newState)
    {
        if (newState == null || newState.GetType() == m_currentState.GetType()) return;

        m_currentState?.Exit();
        m_currentState = newState;
        m_currentState?.Enter();
    }

    #region ======================================== Move & Rotation

    public void HandleMove()
    {
        Vector3 m_moveDirection = Vector3.zero;
        float m_currentMoveSpeed = m_playerCore.m_moveSpeed;

        if (!m_playerCore.m_InputManager.m_IsMove)
        {
            m_moveDirection = Vector3.zero;
            m_currentMoveSpeed = 0f;
            m_playerCore.m_Rigidbody.linearVelocity = Vector3.zero;
            //m_playerCore.m_AnimationManager.UpdateMovementAnimation(Vector3.zero);
            //return;
        }

        var prevMoveSpeed = m_currentMoveSpeed;


        // �̵� �ӵ� ����
        if (m_playerCore.m_InputManager.m_IsRun)
        {
            if (m_playerCore.m_InputManager.m_IsCrouch) m_currentMoveSpeed = m_playerCore.m_crouchRunSpeed;
            else m_currentMoveSpeed = m_playerCore.m_runSpeed;
        }
        else if (m_playerCore.m_InputManager.m_IsCrouch) m_currentMoveSpeed = m_playerCore.m_crouchSpeed;

        // �̵� ���� ���
        Vector2 moveInput = m_playerCore.m_InputManager.m_MovementInput;
        m_currentAnimInput = Vector2.Lerp(m_currentAnimInput, moveInput, Time.deltaTime * 10f); //�ִϸ��̼� ���� ���� ������ ����

        //���� ����(transform�� ���� �������� ������ ���� �� ��ֶ�����)
        m_moveDirection = Vector3.Normalize(m_playerCore.transform.right * moveInput.x + m_playerCore.transform.forward * moveInput.y);
        

        //������(currentSpeed)���� ��ǥ��(targetSpeed)���� ��ȭ�ϴ� ���������� ���� �����ð��� ������ �ε巴�� �̾�������.SmoothDamp�� ���� �ε巴�� ��ȭ��Ŵ
        //var targetSpeed = Mathf.SmoothDamp(prevMoveSpeed, m_currentMoveSpeed, ref m_speedSmoothVelocity, m_speedSmoothTime);

        // ���鿡 ��� �ִ� ���
        if (m_isGrounded && m_currentVelocityY < 0)
        {
            m_currentVelocityY = 0f; // ��¦ ������ �����ϸ� ������
        }
        else if (!m_isGrounded)
        {
            // �߷� ����
            m_currentVelocityY -= 30f * Time.deltaTime;
        }

        Vector3 velocity = m_moveDirection * m_currentMoveSpeed + Vector3.up * m_currentVelocityY; //�ӵ��� �ΰ����� ������ ��/�� �׸��� ���� ���� ����Ͽ� �������� ��ģ��(�̷��� �ؾ� ������ currentVelocityY�� ���� ����� �� ����)

        m_playerCore.m_Rigidbody.MovePosition(m_playerCore.transform.position + velocity * Time.deltaTime); //�̵����� Rigidbody�� ����

        //============================================================== Transform �̵�
        /* //���ͽ��� �帣�� ��� ���� �̵��� ����
        m_moveDirection = (m_playerCore.transform.right * moveInput.x + m_playerCore.transform.forward * moveInput.y) * m_currentMoveSpeed;
        m_moveDirection.Normalize();
        // �̵� ����
        m_playerCore.transform.position += m_moveDirection * Time.deltaTime;*/
        //==============================================================
        //�̵� �ִϸ��̼�
        m_playerCore.m_AnimationManager.UpdateMovementAnimation(m_currentAnimInput);
    }

    // ȸ�� ó��
    public void HandleRotation()
    {
        // ȭ�� ȸ�� ���� ��
        if (m_playerCore.m_InputManager.m_IsStopCameraRot) return;
        
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
        float rotationSpeed = m_playerCore.m_InputManager.m_IsAim ? m_playerCore.m_rotationSpeed : m_playerCore.m_rotationAimSpeed;

        float maxDegrees = rotationSpeed * Time.deltaTime * m_playerCore.m_rotationDamping;

        Quaternion smoothedRotation = Quaternion.Slerp(currentRotation, targetRotation, maxDegrees);

        m_playerCore.m_Rigidbody.MoveRotation(smoothedRotation);
    }

    #endregion ======================================== Move & Rotation

    public void HandleInAir()
    {
        if (m_playerCore.m_InputManager.m_IsInAir)
        {
            m_playerCore.m_Rigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }

    //Locomotion�� SubFlags �ִϸ��̼� ����
    public void UpdateLocomotionFlagAnimation()
    {
        bool isCrouch = m_playerCore.m_StateFlagManager.HasLocomotionFlag(LocomotionSubFlags.Crouch);
        bool isRun = m_playerCore.m_StateFlagManager.HasLocomotionFlag(LocomotionSubFlags.Run);

        m_playerCore.m_AnimationManager.SetBool(m_locomotionFlagAniMap[LocomotionSubFlags.Crouch], isCrouch);
        m_playerCore.m_AnimationManager.SetBool(m_locomotionFlagAniMap[LocomotionSubFlags.Run], isRun);
    }

    //Locomotion�� MainState �ִϸ��̼� ���� - LocomotionBaseState���� ȣ��
    public void UpdateLocomotionMainStateAnimation(LocomotionMainState locomotionMainState, bool isPlay)
    {
        if (m_locomotionMainAniMap.ContainsKey(locomotionMainState))
        {
            m_playerCore.m_AnimationManager.SetBool(m_locomotionMainAniMap[locomotionMainState], isPlay);
        }
        else
        {
            Debug.LogError("�ִϸ��̼��� �������� �ʽ��ϴ�.");
        }
    }

    public void SetIsGrounded(bool isGrounded)
    {
        m_isGrounded = isGrounded;
    }
}