//Movment 관련
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

    private float m_speedSmoothVelocity;  //ref 이동값의 변화 속도
    private float m_speedSmoothTime;  //플레이어의 움직임 속도값 변화를 부드럽게 해주는 지연값

    private float m_currentVelocityY;

    //생성자 - PlayerCore에서 New로 생성
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

    // 이동 처리
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
        // 이동 속도 결정
        m_currentMoveSpeed = m_playerCore.m_InputManager.IsSprinting ? m_playerCore.m_sprintSpeed :
                           m_playerCore.m_InputManager.IsCrouching ? m_playerCore.m_crouchSpeed :
                           m_playerCore.m_moveSpeed;

        // 이동 방향 계산
        Vector2 moveInput = m_playerCore.m_InputManager.MovementInput;

        //뱡향 선정(transform의 로컬 방향으로 움직임 제어 및 노멀라이즈)
        m_moveDirection = Vector3.Normalize(m_playerCore.transform.right * moveInput.x + m_playerCore.transform.forward * moveInput.y);

        //원래값(currentSpeed)에서 목표값(targetSpeed)으로 변화하는 직전까지의 값에 지연시간을 적용해 부드럽게 이어지도록.SmoothDamp는 값을 부드럽게 변화시킴
        var targetSpeed = Mathf.SmoothDamp(prevMoveSpeed, m_currentMoveSpeed, ref m_speedSmoothVelocity, m_speedSmoothTime);

        //m_currentVelocityY += Physics.gravity.y * Time.deltaTime; //시간에 따라 중력값만큼 밑으로 계속 떨어지게 설정
        m_currentVelocityY = m_playerCore.m_Rigidbody.linearVelocity.y;
        var velocity = m_moveDirection * targetSpeed + Vector3.up * m_currentVelocityY; //속도를 두가지로 나누어 앞/옆 그리고 위로 따로 계산하여 마지막에 합친것(이렇게 해야 점프때 currentVelocityY로 따로 계산할 수 있음)

        m_playerCore.m_Rigidbody.linearVelocity = velocity;

        //============================================================== Transform 이동
        /* //루터슈터 장르의 경우 물리 이동이 유리
        m_moveDirection = (m_playerCore.transform.right * moveInput.x + m_playerCore.transform.forward * moveInput.y) * m_currentMoveSpeed;
        m_moveDirection.Normalize();
        // 이동 적용
        m_playerCore.transform.position += m_moveDirection * Time.deltaTime;*/
        //==============================================================
        //이동 애니메이션
        m_playerCore.m_AnimationManager.UpdateMovementAnimation(m_playerCore.m_InputManager.MovementInput);
    }

    // 회전 처리
    public void HandleRotation()
    {
        // 나중에 적용
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