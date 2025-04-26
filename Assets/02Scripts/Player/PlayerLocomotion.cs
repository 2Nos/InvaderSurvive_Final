//루터슈터 장르의 경우 물리 이동이 유리
//Movment 관련

using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

//TODO : 추후 상태 많아질 경우 LocomotionStateInfo 파일로 이동할 예정
#region ======================================== MainState And Flags
/// <summary>
/// 메인 이동 상태
/// 항상 단일(MainState들중 하나의, Flags 적용x) 상태로 유지 + Flags 복수개 선택
/// Move + m_LocomotionFlags(FalgButtonGroupManager + Aming)
/// </summary>
public enum LocomotionMainState
{
    Idle = 0,           // Idle
    Move = 1,           // 기본 이동
    Jump = 2,          // 점프
    InAir = 3,          // 공중 (점프/낙하)
    Land = 4,           // 착지
    Dodge = 5,          // 구르기(회피기)
    Slide = 6,          // 슬라이딩
    Climb = 7,          // 등반
    WallRun = 8,         // 벽 달리기
    Staggered = 9,         // 피격 경직 같은 상태
    Knockback = 10,         // 넉백
}

/// <summary>
/// Flags는 복수의 열거형 선택이 가능한 기능. 즉, 복수 상태가 가능
/// Flags는 2의 제곱수나 2의 제곱수 조합을 사용하여 선언해야함
/// None =0, FalgButtonGroupManager = 1, Croucning = 2, 4, 8 이런식 보다는 쉬프트연산)
/// </summary>
[Flags]
public enum LocomotionSubFlags
{
    None = 0,
    Run = 1 << 1,           // 달리기
    Crouch = 1 << 2,        // 앉기
    CrouchRun = 1 << 3      // 앉아서 달리기 
}

//TODO: 나중에 필요할지 모르니 남겨둠
/*public ActionStateFlags m_ActionFlags
{
   get => m_actionFlags;
   set => m_actionFlags = value;
}
/// <summary>
/// 순간적인 모든 상태에 영향이 가는 상태는 이렇게 따로 관리
/// </summary>
[Flags]
public enum ActionStateFlags
{
   None = 0,
   Dodge = 1 << 0,           // 구르기(회피기)
   Staggered = 1 << 1,         // 피격 경직 같은 상태
   Knockback = 1 << 2          // 넉백
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
    // Main State (단일 상태)
    //private LocomotionMainState m_CurrentMainState { get; set; } = LocomotionMainState.Idle; // 각 State에서 DoTransitionState()함수를 통해 변환
    //private LocomotionMainState m_PrevMainState { get; set; } = LocomotionMainState.Idle;

    // Flags (복수개 상태 적용)
    private LocomotionSubFlags m_LocomotionFlags { get; set; } = LocomotionSubFlags.None;
    public void SetLocomotionFlag(LocomotionSubFlags flag) => m_LocomotionFlags |= flag;      // 해당 상태로 설정
    public bool HasLocomotionFlag(LocomotionSubFlags flag) => (m_LocomotionFlags & flag) != 0;
    public void ClearLocomotionFlag(LocomotionSubFlags flag) => m_LocomotionFlags &= ~flag;   // 상태 설정 제거
    public void ClearAllLocomotionFlags() => m_LocomotionFlags = LocomotionSubFlags.None; // 모든 상태 제거

    //TODO: 나중에 필요할지 모르니 남겨둠 (Dodge같은 액션들을 차후 확장성을 위해 분류 필요할지도 모름)
    /*#region ======================================== Action
   private ActionStateFlags m_actionFlags = ActionStateFlags.None;
   public void SetActionStateFlags(ActionStateFlags flag) => m_actionFlags |= flag;
   public bool HasActionStateFlags(ActionStateFlags flag) => (m_actionFlags & flag) != 0;
   public void ClearActionStateFlags(ActionStateFlags flag) => m_actionFlags &= ~flag;
   #endregion ======================================== /Action*/
    #endregion ======================================== /GetSetFlags

    #region ======================================== Dictionary Map
    // 메모리 절약을 위한 상태 캐시, 상태 전환 시 사용(미리 new를 하지 않는 이유는 static 변수로 선언하여 m_PlayerCore를 넘겨줘야함)
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
    #endregion ======================================== /애니메이션 Map

    #region ======================================== State 관리
    private LocomotionBaseState m_currentState;
    private LocomotionBaseState m_changeMainState;
    public LocomotionBaseState m_prevState { get; private set; }
    #endregion ======================================== /State 관리

    // Move 설정
    private Vector2 m_currentAniInput = Vector2.zero;
    private float m_currentVelocityY = 0;
    private float m_prevSpeed = 0;

    public bool m_IsGrounded { get; private set; }
    public bool m_IsNoCheckGround { get; private set; } = false; //중력 무시 상태
    //public bool m_IsInAir { get; private set; } = false; //공중 상태인지 체크
    //public bool m_IsLand { get; private set; } = false; //착지 상태인지 체크

    //private Dictionary<LocomotionMainState, LocomotionBaseState> m_MainStateMap = new();

    // PlayerCore에서 Start에서 호출
    public void InitializeLocomotion()
    {
        // 상태 객체를 생성할 때 PlayerCore를 주입
        m_MainStateMap[LocomotionMainState.Idle] = new IdleState(m_playerCore);
        m_MainStateMap[LocomotionMainState.Move] = new MoveState(m_playerCore);
        m_MainStateMap[LocomotionMainState.Jump] = new JumpState(m_playerCore);
        m_MainStateMap[LocomotionMainState.InAir] = new InAirState(m_playerCore);
        m_MainStateMap[LocomotionMainState.Land] = new LandState(m_playerCore);
        m_MainStateMap[LocomotionMainState.Slide] = new SlideState(m_playerCore);
        m_MainStateMap[LocomotionMainState.Climb] = new ClimbState(m_playerCore);
        m_MainStateMap[LocomotionMainState.WallRun] = new WallRunState(m_playerCore);

        m_currentState = m_MainStateMap[LocomotionMainState.Idle]; //초기화 상태는 Idle로 설정
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
        m_ChangeCreateState = Create(locomotionMainState); //이걸로 Update에서 상태를 체크함
    }*/

    #region ======================================== State 관리
    public void SwithcCurrentState(LocomotionBaseState newState)
    {
        if (newState == m_prevState) return; 
        
        m_currentState?.Exit();
        m_prevState = m_currentState; //이전 상태 저장
        m_currentState = newState;
        m_currentState?.Enter();
        Debug.Log($"Current State: {m_currentState}");
    }

    //각 상태에서 호출
    public void ChangeMainState(LocomotionMainState state)
    {
        m_changeMainState = m_MainStateMap[state];
    }

    #endregion ======================================== /상태 전환 관리

    #region ======================================== Movement 관리

    /// <summary>
    /// 지면 체크
    /// </summary>
    public void UpdateCheckGround()
    {
        if(m_IsNoCheckGround) return; //중력 무시 상태일 경우

        Vector3 centerRay = m_playerCore.transform.position;
        Vector3 forwardRay = m_playerCore.transform.position + m_playerCore.transform.forward * 0.3f + m_playerCore.transform.up * 0.3f;
        Vector3 rayDir = -m_playerCore.transform.up;

        Debug.DrawRay(centerRay, rayDir * 10f, Color.red, 0.1f);
        Debug.DrawRay(forwardRay, rayDir * 10f, Color.red, 0.1f);

        // 현재는 평평 땅만 체크
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

        //TODO : 천장 체크
        //TODO : 계단 및 내리막길 체크
    }

    /// <summary>
    /// FixedUpdate에서 호출
    /// 이동, 회전, 중력, 점프는 각 상태에서 호출
    /// </summary>
    public void HandleMove()
    {
        // 1.초기화
        Vector2 moveInput = m_playerCore.m_InputManager.m_MovementInput;    //입력값
        Vector3 m_moveDirection = m_playerCore.transform.right * moveInput.x + m_playerCore.transform.forward * moveInput.y; //방향
        m_moveDirection.Normalize(); //정규화

        // //애니메이션 블렌드 트리동작 연결 보간(Input 0.7, 1 이런식으로 들어오기에 끊기는 동작 개선)
        m_currentAniInput = Vector2.Lerp(m_currentAniInput, moveInput, Time.deltaTime * 10f);

        // 3. 이동 중일 때
        // 3.1. 이동 속도 결정은 각 상태에서 Set로 설정(스피드 종류가 많은 관계로)
        float currentSpeed = m_playerCore.m_CurrentSpeed;
        
        currentSpeed = Mathf.Lerp(m_prevSpeed, currentSpeed, Time.deltaTime * 10f); //이전 스피드에서 현재 스피드로 보간
        m_prevSpeed = currentSpeed;
        Vector3 velocity = m_moveDirection * currentSpeed;

        // 3.2 이동값 적용 (이런류의 게임은 단순 좌표보다는 물리엔진이용)
        m_playerCore.SetRigidVelocity(velocity);

        //이동 애니메이션
        m_playerCore.m_AnimationManager.UpdateMovementAnimation(m_currentAniInput);
    }

    public void HandleRotation()
    {
        // 화면 회전 중지 시
        if (m_playerCore.m_InputManager.m_IsStopBodyRot) return;

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
        float rotationSpeed = m_playerCore.m_CurrentRotSpeed; //Aim 상태에서 스피드 다루기

        // 회전 속도 조절
        //float maxDegrees = rotationSpeed * Time.deltaTime * m_playerCore.m_rotationDamping; //m_rotationDamping : 감속 주는것

        Quaternion smoothedRotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed);

        m_playerCore.m_Rigidbody.MoveRotation(smoothedRotation);
    }

    private bool m_NotGravity; //중력 무시 상태

    public void HandleGravityMovement()
    {
        //중력 무시 상태일 경우
        if (m_NotGravity || m_IsGrounded) return;

        // TODO : 위쪽 벽 충돌 처리
        // 1. 중력값 계산
        m_currentVelocityY += m_playerCore.m_Gravity * Time.deltaTime;

        // 2. 낙하시 속도 제한
        m_currentVelocityY = Mathf.Clamp(m_currentVelocityY, m_playerCore.m_MinVelocityY, m_playerCore.m_MaxVelocityY);

        // 3. 중력 적용
        m_playerCore.SetRigidVelocityY(m_currentVelocityY);
    }


    #endregion ======================================== /Move & Rotation

    #region ======================================== Animation 관리

    //Enter과 Exit에서 한번만 불러와도 되는 애니메이션, 값을 하나만 받아오는 케이스만 사용
    public void SetMainStateAnimation(LocomotionMainState locomotionMainState, AniParmType setAniParmType, bool isPlay, float value = 0)
    {
        //TODO : 애니메이션 세팅을 위한 Enum을 따로 만들어서 관리
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
    //Locomotion의 SubFlags 애니메이션 관리
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
        else Debug.LogError("애니메이션 이름이 잘못되었습니다: ");
    }*/
    #endregion ======================================== /애니메이션 호출
}