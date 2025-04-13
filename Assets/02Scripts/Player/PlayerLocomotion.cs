//Movment ����
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerLocomotion
{
    public PlayerCore m_playerCore;
    private LocomotionBaseState m_currentState;

    private Vector3 m_moveDirection;

    private float m_currentMoveSpeed;

    private float m_speedSmoothVelocity;  //ref �̵����� ��ȭ �ӵ�
    private float m_speedSmoothTime;  //�÷��̾��� ������ �ӵ��� ��ȭ�� �ε巴�� ���ִ� ������

    private float m_currentVelocityY;

    //������ - PlayerCore���� New�� ����
    public PlayerLocomotion(PlayerCore core)
    {
        m_playerCore = core;
        m_currentState = new IdleState(m_playerCore);
        m_currentState.Enter();
    }

    public void Update()
    {
        m_currentState?.Update();
        /*LocomotionBaseState newState = m_currentState.CheckTransitions();
        if (newState != null)
        {
            ChangeState(newState);
        }*/
    }

    public void ChangeState(LocomotionBaseState newState)
    {
        m_currentState?.Exit();
        m_currentState = newState;
        m_currentState?.Enter();
    }

    // �̵� ó��
    public void HandleMove()
    {
        if (!m_playerCore.m_InputManager.IsMoving)
        {
            m_moveDirection = Vector3.zero;
            m_currentMoveSpeed = 0f;
            m_playerCore.m_Rigidbody.linearVelocity = Vector3.zero;
            m_playerCore.m_AnimationManager.UpdateMovementAnimation(Vector3.zero);
            return;
        }
        var prevMoveSpeed = m_currentMoveSpeed;
        // �̵� �ӵ� ����
        m_currentMoveSpeed = m_playerCore.m_InputManager.IsSprinting ? m_playerCore.m_sprintSpeed :
                           m_playerCore.m_InputManager.IsCrouching ? m_playerCore.m_crouchSpeed :
                           m_playerCore.m_moveSpeed;

        // �̵� ���� ���
        Vector2 moveInput = m_playerCore.m_InputManager.MovementInput;

        //���� ����(transform�� ���� �������� ������ ���� �� ��ֶ�����)
        m_moveDirection = Vector3.Normalize(m_playerCore.transform.right * moveInput.x + m_playerCore.transform.forward * moveInput.y);

        //������(currentSpeed)���� ��ǥ��(targetSpeed)���� ��ȭ�ϴ� ���������� ���� �����ð��� ������ �ε巴�� �̾�������.SmoothDamp�� ���� �ε巴�� ��ȭ��Ŵ
        var targetSpeed = Mathf.SmoothDamp(prevMoveSpeed, m_currentMoveSpeed, ref m_speedSmoothVelocity, m_speedSmoothTime);

        //m_currentVelocityY += Physics.gravity.y * Time.deltaTime; //�ð��� ���� �߷°���ŭ ������ ��� �������� ����
        m_currentVelocityY = m_playerCore.m_Rigidbody.linearVelocity.y;
        var velocity = m_moveDirection * targetSpeed + Vector3.up * m_currentVelocityY; //�ӵ��� �ΰ����� ������ ��/�� �׸��� ���� ���� ����Ͽ� �������� ��ģ��(�̷��� �ؾ� ������ currentVelocityY�� ���� ����� �� ����)

        m_playerCore.m_Rigidbody.linearVelocity = velocity;

        //============================================================== Transform �̵�
        /* //���ͽ��� �帣�� ��� ���� �̵��� ����
        m_moveDirection = (m_playerCore.transform.right * moveInput.x + m_playerCore.transform.forward * moveInput.y) * m_currentMoveSpeed;
        m_moveDirection.Normalize();
        // �̵� ����
        m_playerCore.transform.position += m_moveDirection * Time.deltaTime;*/
        //==============================================================
        //�̵� �ִϸ��̼�
        m_playerCore.m_AnimationManager.UpdateMovementAnimation(m_playerCore.m_InputManager.MovementInput);
    }

    // ȸ�� ó��
    public void HandleRotation()
    {
        // ���߿� ����
        /*Vector3 lookDirection = new Vector3(m_InputManager.LookInput.x, 0, m_InputManager.LookInput.y);

        if(lookDirection.sqrMagnitude > 0.01f)
        {
            Vector3 worldDirection = Camera.main.transform.TransformDirection(lookDirection);
            worldDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(worldDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }*/
        Vector3 playerEuler = m_playerCore.transform.eulerAngles;
        playerEuler.y = m_playerCore.m_CameraManager.m_yaw;
        m_playerCore.transform.rotation = Quaternion.Euler(playerEuler);
    }
}