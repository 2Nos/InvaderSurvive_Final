//루터슈터 장르의 경우 물리 이동이 유리
//Movment 관련

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
    // Move 설정
    private float m_currentVelocityY = 0;
    private Vector2 m_currentAnimInput = Vector2.zero;

    public bool m_IsGrounded { get; private set; }

    public bool m_IsProgress = false;
    //생성자 - PlayerCore에서 New로 생성
    public PlayerLocomotion(PlayerCore core)
    {
        m_PlayerCore = core;
        m_currentState = new IdleState(this);

        //m_PlayerLocomotion.m_locomotion이 아직 생성되지 않았으므로 PlayerCore에서 생성 후 Enter()를 호출해야함
        //m_currentState.Enter();
    }

    //애니메이션 및 상태를 Dictionary로 관리하며 전달
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
    /// PlayerCore에서 Start에서 호출
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
        if(newState == null || newState == m_currentState) return; //null인 경우는 상태가 없다는 것, 즉 Idle상태로 돌아감
        //LocomotionBaseState에서 실질적으로Locomotion이 참조가되지 않음

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

        // 이동 속도 결정
        if (m_PlayerCore.m_InputManager.m_IsRun_LocoF)
        {
            if (m_PlayerCore.m_InputManager.m_IsCrouch_LocoF) m_currentMoveSpeed = m_PlayerCore.m_crouchRunSpeed;
            else m_currentMoveSpeed = m_PlayerCore.m_runSpeed;
        }
        else if (m_PlayerCore.m_InputManager.m_IsCrouch_LocoF) m_currentMoveSpeed = m_PlayerCore.m_crouchSpeed;

        // 이동 방향 계산
        Vector2 moveInput = m_PlayerCore.m_InputManager.m_MovementInput;
        m_currentAnimInput = Vector2.Lerp(m_currentAnimInput, moveInput, Time.deltaTime * 10f); //애니메이션 동작 연결 보간을 위한

        //뱡향 선정(transform의 로컬 방향으로 움직임 제어 및 노멀라이즈)
        m_moveDirection = Vector3.Normalize(m_PlayerCore.transform.right * moveInput.x + m_PlayerCore.transform.forward * moveInput.y);    
        
        Vector3 velocity = m_moveDirection * m_currentMoveSpeed; //속도를 두가지로 나누어 앞/옆 그리고 위로 따로 계산하여 마지막에 합친것(이렇게 해야 점프때 currentVelocityY로 따로 계산할 수 있음)

        m_PlayerCore.m_Rigidbody.MovePosition(m_PlayerCore.transform.position + velocity * Time.deltaTime); //이동값을 Rigidbody에 적용

        //이동 애니메이션
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

    //현재 함수 내용 InAir로 이동
    //중력 사용하고 있으므로 일단은 주석처리
    private float m_IgnoreGroundCheckTime = 1f;
    private float m_GroundCheckTimer = 0f;
    /// <summary>
    /// 점프 및 중력 처리
    /// </summary>
    public void UpdateVerticalMovement()
    {
        // TODO : 위쪽 벽 충돌 처리

        /*// 1. Ground Check 타이머 감소
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

                    // 바닥 뚫는 것 방지
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
        // 3. 중력 적용
        if (!m_IsGrounded)
        {
            m_currentVelocityY += m_PlayerCore.m_Gravity * Time.deltaTime;

            //TODO : 낙하 속도 제한
            m_currentVelocityY = Mathf.Clamp(m_currentVelocityY, -m_PlayerCore.m_MaxFallingSpeed, m_PlayerCore.m_jumpForce);
        }

        // 4. 이동 적용
        Vector3 velocity = new Vector3(0, m_currentVelocityY, 0);
        m_PlayerCore.m_Rigidbody.MovePosition(m_PlayerCore.transform.position + velocity * Time.deltaTime);*/
    }

    /// <summary>
    /// Jump키 눌렸을 때 LocomotionBase의 CheckLocomotion에서 호출;
    /// </summary>
    public void ExecuteJump()
    {
        m_PlayerCore.m_Rigidbody.AddForce(m_PlayerCore.transform.up * m_PlayerCore.m_jumpForce, ForceMode.Impulse); //점프 힘을 주기 위해 Rigidbody에 힘을 줌
        //m_currentVelocityY = m_PlayerCore.m_jumpForce;
        //m_GroundCheckTimer = m_IgnoreGroundCheckTime; // 점프 직후 바닥 체크 무시!
    }

    public void HandleRotation()
    {
        // 화면 회전 중지 시
        if (m_PlayerCore.m_InputManager.m_IsStopCameraRot) return;
        
        // 현재 플레이어 방향
        Quaternion currentRotation = m_PlayerCore.m_Rigidbody.transform.rotation;

        // 기본적으로 바라볼 방향은 플레이어의 전방
        Vector3 targetForward = m_PlayerCore.m_Rigidbody.transform.forward;

        // 에임 중일 경우, 카메라 방향(에임 위치)으로 회전
        if (m_PlayerCore.m_InputManager.m_IsAim)
        {
            //현재의 에임 위치를 가져옴
            Vector3 aimPos = m_PlayerCore.m_CameraManager.UpdateAimTargetPos();
            aimPos.y = m_PlayerCore.transform.position.y; //캐릭터가 x,z축(위,아래로)은 돌아가지 않도록 설정하기 위해 높이 동일하게 유지

            // 조준 위치로의 방향
            Vector3 aimDirection = (aimPos - m_PlayerCore.transform.position).normalized;

            // 회전힘 발생 시
            if (aimDirection.sqrMagnitude > 0.01f)
            {
                targetForward = aimDirection;
            }
        }
        else // 기본 상태
        {
            float cameraYaw = m_PlayerCore.m_CameraManager.m_MouseX;
            Quaternion targetRot = Quaternion.Euler(0f, cameraYaw, 0f);
            targetForward = targetRot * Vector3.forward;
        }
        
        //현재 바라볼 대상으로의 로테이션 값, Slerp를 위해 저장
        Quaternion targetRotation = Quaternion.LookRotation(targetForward); 

        // 일반상태와 에임 상태에 따라 회전 속도 조절
        float rotationSpeed = m_PlayerCore.m_InputManager.m_IsAim ? m_PlayerCore.m_rotationSpeed : m_PlayerCore.m_rotationAimSpeed;

        float maxDegrees = rotationSpeed * Time.deltaTime * m_PlayerCore.m_rotationDamping;

        Quaternion smoothedRotation = Quaternion.Slerp(currentRotation, targetRotation, maxDegrees);

        m_PlayerCore.m_Rigidbody.MoveRotation(smoothedRotation);
    }
    #endregion ======================================== /Move & Rotation

    //Locomotion의 SubFlags 애니메이션 관리
    public void UpdateLocomotionFlagAnimation()
    {
        bool isCrouch = m_PlayerCore.m_StateFlagManager.HasLocomotionFlag(LocomotionSubFlags.Crouch);
        bool isRun = m_PlayerCore.m_StateFlagManager.HasLocomotionFlag(LocomotionSubFlags.Run);

        m_PlayerCore.m_AnimationManager.SetBool(m_locomotionFlagAniMap[LocomotionSubFlags.Crouch], isCrouch);
        m_PlayerCore.m_AnimationManager.SetBool(m_locomotionFlagAniMap[LocomotionSubFlags.Run], isRun);
    }

    //Locomotion의 MainState 애니메이션 관리 - LocomotionBaseState에서 호출
    public void UpdateLocomotionMainStateAnimation(LocomotionMainState locomotionMainState, bool isPlay)
    {
        //TODO : 애니메이션 세팅을 위한 Enum을 따로 만들어서 관리
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

            //SetTrigger도 동작
            if (locomotionMainState == LocomotionMainState.InAir)
            {
                if (!isPlay) return;
                m_PlayerCore.m_AnimationManager.SetTrigger("IsJump");
            }
        }
        else
        {
            Debug.LogError("애니메이션이 존재하지 않습니다.");
        }

        
    }
}