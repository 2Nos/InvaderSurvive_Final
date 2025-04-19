//Movment 관련
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;

public class PlayerLocomotion
{
    public PlayerCore m_playerCore;
    private LocomotionBaseState m_currentState;
    
    // Move 설정
    float m_speedSmoothVelocity;  //ref 이동값의 변화 속도
    float m_speedSmoothTime = 0.1f;  //플레이어의 움직임 속도값 변화를 부드럽게 해주는 지연값
    float m_currentVelocityY;
    private Vector2 m_currentAnimInput = Vector2.zero;

    public bool m_isGrounded { get; private set; }

    public bool m_GetIsGrounded() => m_isGrounded;

    //생성자 - PlayerCore에서 New로 생성
    public PlayerLocomotion(PlayerCore core)
    {
        m_playerCore = core;
        m_currentState = new IdleState(m_playerCore);

        //m_playerCore.m_locomotion이 아직 생성되지 않았으므로 PlayerCore에서 생성 후 Enter()를 호출해야함
        //m_currentState.Enter();
    }

    //애니메이션 및 상태를 Dictionary로 관리하며 전달
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


        // 이동 속도 결정
        if (m_playerCore.m_InputManager.m_IsRun)
        {
            if (m_playerCore.m_InputManager.m_IsCrouch) m_currentMoveSpeed = m_playerCore.m_crouchRunSpeed;
            else m_currentMoveSpeed = m_playerCore.m_runSpeed;
        }
        else if (m_playerCore.m_InputManager.m_IsCrouch) m_currentMoveSpeed = m_playerCore.m_crouchSpeed;

        // 이동 방향 계산
        Vector2 moveInput = m_playerCore.m_InputManager.m_MovementInput;
        m_currentAnimInput = Vector2.Lerp(m_currentAnimInput, moveInput, Time.deltaTime * 10f); //애니메이션 동작 연결 보간을 위한

        //뱡향 선정(transform의 로컬 방향으로 움직임 제어 및 노멀라이즈)
        m_moveDirection = Vector3.Normalize(m_playerCore.transform.right * moveInput.x + m_playerCore.transform.forward * moveInput.y);
        

        //원래값(currentSpeed)에서 목표값(targetSpeed)으로 변화하는 직전까지의 값에 지연시간을 적용해 부드럽게 이어지도록.SmoothDamp는 값을 부드럽게 변화시킴
        //var targetSpeed = Mathf.SmoothDamp(prevMoveSpeed, m_currentMoveSpeed, ref m_speedSmoothVelocity, m_speedSmoothTime);

        // 지면에 닿아 있는 경우
        if (m_isGrounded && m_currentVelocityY < 0)
        {
            m_currentVelocityY = 0f; // 살짝 음수로 유지하면 안정적
        }
        else if (!m_isGrounded)
        {
            // 중력 적용
            m_currentVelocityY -= 30f * Time.deltaTime;
        }

        Vector3 velocity = m_moveDirection * m_currentMoveSpeed + Vector3.up * m_currentVelocityY; //속도를 두가지로 나누어 앞/옆 그리고 위로 따로 계산하여 마지막에 합친것(이렇게 해야 점프때 currentVelocityY로 따로 계산할 수 있음)

        m_playerCore.m_Rigidbody.MovePosition(m_playerCore.transform.position + velocity * Time.deltaTime); //이동값을 Rigidbody에 적용

        //============================================================== Transform 이동
        /* //루터슈터 장르의 경우 물리 이동이 유리
        m_moveDirection = (m_playerCore.transform.right * moveInput.x + m_playerCore.transform.forward * moveInput.y) * m_currentMoveSpeed;
        m_moveDirection.Normalize();
        // 이동 적용
        m_playerCore.transform.position += m_moveDirection * Time.deltaTime;*/
        //==============================================================
        //이동 애니메이션
        m_playerCore.m_AnimationManager.UpdateMovementAnimation(m_currentAnimInput);
    }

    // 회전 처리
    public void HandleRotation()
    {
        // 화면 회전 중지 시
        if (m_playerCore.m_InputManager.m_IsStopCameraRot) return;
        
        // 현재 플레이어 방향
        Quaternion currentRotation = m_playerCore.m_Rigidbody.transform.rotation;

        // 기본적으로 바라볼 방향은 플레이어의 전방
        Vector3 targetForward = m_playerCore.m_Rigidbody.transform.forward;

        // 에임 중일 경우, 카메라 방향(에임 위치)으로 회전
        if (m_playerCore.m_InputManager.m_IsAim)
        {
            //현재의 에임 위치를 가져옴
            Vector3 aimPos = m_playerCore.m_CameraManager.UpdateAimTargetPos();
            aimPos.y = m_playerCore.transform.position.y; //캐릭터가 x,z축(위,아래로)은 돌아가지 않도록 설정하기 위해 높이 동일하게 유지

            // 조준 위치로의 방향
            Vector3 aimDirection = (aimPos - m_playerCore.transform.position).normalized;

            // 회전힘 발생 시
            if (aimDirection.sqrMagnitude > 0.01f)
            {
                targetForward = aimDirection;
            }
        }
        else // 기본 상태
        {
            float cameraYaw = m_playerCore.m_CameraManager.m_MouseX;
            Quaternion targetRot = Quaternion.Euler(0f, cameraYaw, 0f);
            targetForward = targetRot * Vector3.forward;
        }
        

        //현재 바라볼 대상으로의 로테이션 값, Slerp를 위해 저장
        Quaternion targetRotation = Quaternion.LookRotation(targetForward); 

        // 일반상태와 에임 상태에 따라 회전 속도 조절
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

    //Locomotion의 SubFlags 애니메이션 관리
    public void UpdateLocomotionFlagAnimation()
    {
        bool isCrouch = m_playerCore.m_StateFlagManager.HasLocomotionFlag(LocomotionSubFlags.Crouch);
        bool isRun = m_playerCore.m_StateFlagManager.HasLocomotionFlag(LocomotionSubFlags.Run);

        m_playerCore.m_AnimationManager.SetBool(m_locomotionFlagAniMap[LocomotionSubFlags.Crouch], isCrouch);
        m_playerCore.m_AnimationManager.SetBool(m_locomotionFlagAniMap[LocomotionSubFlags.Run], isRun);
    }

    //Locomotion의 MainState 애니메이션 관리 - LocomotionBaseState에서 호출
    public void UpdateLocomotionMainStateAnimation(LocomotionMainState locomotionMainState, bool isPlay)
    {
        if (m_locomotionMainAniMap.ContainsKey(locomotionMainState))
        {
            m_playerCore.m_AnimationManager.SetBool(m_locomotionMainAniMap[locomotionMainState], isPlay);
        }
        else
        {
            Debug.LogError("애니메이션이 존재하지 않습니다.");
        }
    }

    public void SetIsGrounded(bool isGrounded)
    {
        m_isGrounded = isGrounded;
    }
}