// ========================================250415
// 

// ========================================250410
// Locomotion, Combat에 메인 단일 상태가 필요한 메인인 상태들에 대해선 MainState로 관리
// IsAiming, IsSprinting, IsCrouching은 같은 복수가 가능한 State들은 SubFlags로 관리

// ========================================
using DUS.Player.Locomotion;
using UnityEngine;
using System.Collections.Generic;
using DUS.Joystick;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MainStateAndSubFlagsManager))]
[RequireComponent(typeof(PlayerInputManager))] // InputManager컴포넌트를 종속성으로 PlayerCore있는 곳에 자동으로 추가

public class PlayerCore : MonoBehaviour
{
    #region ======================================== Module
    public PlayerInputManager m_InputManager { get; private set; }
    public PlayerLocomotion m_Locomotion { get; private set; }
    public PlayerCombat m_Combat { get; private set; }
    public MainStateAndSubFlagsManager m_StateFlagManager { get; private set; }
    public PlayerAnimationManager m_AnimationManager { get; private set; }
    public CameraRigManager m_CameraManager { get; private set; }
    #endregion ======================================== /Module

    #region ======================================== Player Value - Locomotion
    [Header("[ Move ]")]
    [Range(1, 10), SerializeField] float m_walkSpeed = 5f;
    public float m_WalkSpeed => m_walkSpeed;
    [Range(1, 20),SerializeField] float m_runSpeed = 15f;
    public float m_RunSpeed => m_runSpeed;

    [Range(1, 10), SerializeField] float m_crouchSpeed = 3f;
    public float m_CrouchSpeed => m_crouchSpeed;

    [Range(1, 10), SerializeField] float m_crouchRunSpeed = 9f;
    public float m_CrouchRunSpeed => m_crouchRunSpeed;

    [Range(20, 50), SerializeField] float m_sprintSpeed = 20f;
    public float m_SprintSpeed => m_sprintSpeed;

    [Range(1, 10), SerializeField] float m_dodgeSpeed = 5f;
    public float m_DodgeSpeed => m_dodgeSpeed;

    [Range(1, 10), SerializeField] float m_slideSpeed = 5f;
    public float m_SlideSpeed => m_slideSpeed;

    [Range(1, 10), SerializeField] float m_climbSpeed = 5f;
    public float m_ClimbSpeed => m_climbSpeed;

    [Range(1, 10), SerializeField] float m_wallRunSpeed = 5f;
    public float m_WallRunSpeed => m_wallRunSpeed;

    [Header("[ Jump ]")]
    [Range(5, 20), SerializeField] float m_jumpForce = 8f;
    public float m_JumpForce => m_jumpForce;

    [Range(0.1f, 1.0f), SerializeField] float m_jumpForwardSpeedPercent = 0.6f;
    public float m_JumpForwardSpeedPercent => m_jumpForwardSpeedPercent;
    //[SerializeField] public float m_JumpDuration = 1f;
    //[SerializeField] public AnimationCurve m_JumpCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f); // m_JumpDuration 동안 1 → 0 로 곡선 변환

    [Header("[ Gravity ]")]
    [Range(1, 10),SerializeField] float m_addGravity = 1;
    public float m_AddGravity => m_addGravity;

    [SerializeField] LayerMask m_groundMask;
    public LayerMask m_GroundMask => m_groundMask;

    [Range(1, 50), SerializeField] float m_maxVelocityY = 30;
    public float m_MaxVelocityY => m_maxVelocityY;

    [Range(-50, -1),SerializeField] float m_minVelocityY = -30;
    public float m_MinVelocityY => m_minVelocityY;

    [Header("[ PlayerCore Rot ]")]
    [Range(1, 20),SerializeField] float m_rotationSpeed = 10;
    public float m_RotationSpeed => m_rotationSpeed;
    //[Range(1, 60)] public float m_rotationDamping; //회전 감속
    #endregion ======================================== /Player Value Locomotion

    #region ======================================== Player Value - Combat
    [Range(1, 50)] float m_rotationAimSpeed; //에임 상태에서의 회전 속도
    public float m_RotationAimSpeed => m_rotationAimSpeed;

    #endregion ======================================== /Player Value Combat
    public Rigidbody m_Rigidbody { get; private set; }
    public CapsuleCollider[] m_CapsuleCollider { get; private set; }
    public float m_CurrentRotSpeed { get; private set; }

    //받아오는 순서가 중요
    private void Awake()
    {
        //instance = this;
        //m_Locomotion 생성자 전에 먼저 Get으로 모듈을 찾아놓고 있어야함


        m_InputManager = GetComponent<PlayerInputManager>(); // 또는 직접 주입
        m_AnimationManager = GetComponentInChildren<PlayerAnimationManager>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_CapsuleCollider = GetComponents<CapsuleCollider>();
        m_CameraManager = FindObjectOfType<CameraRigManager>();
        m_StateFlagManager = GetComponent<MainStateAndSubFlagsManager>();

        m_groundMask = LayerMask.GetMask("Ground");
        m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous; //리지드바디 감지 모드 변경
        Application.targetFrameRate = 300; //Fixed 프레임 변경

        //생성자 this를 넣지 않는 이유는 생명주기에 의한 내부 코드 복잡도 발생있기에 Initialize로 다음에 처리
        m_Locomotion = new PlayerLocomotion(this);
        //m_Combat = new PlayerCombat(this);


    }
    private void Start()
    {
        OnChangeColider(true);
        m_Locomotion.InitializeLocomotionAtStart();
        m_CurrentRotSpeed = m_RotationSpeed;
        //m_Rigidbody.transform.position = Vector3.zero;
    }

    public void FixedUpdate()
    {
        m_Locomotion?.FixedUpdate();
        //m_Combat?.FixedUpdate();
    }
    private void Update()
    {
        m_Locomotion?.Update();
        m_Combat?.Update();
    }

    private void LateUpdate()
    {
        m_Locomotion?.LateUpdate();
    }

    //TODO : Combat에서 경직이나 특정 메인 상태에 준하는 행위의 경우
    //Locomotion의 메인 상태 멈출 수 있게하는 내용들 정리
    public void CanMove()
    {
        
    }

    public void CanRot()
    {

    }


    #region ======================================== Set Player Value - Locomotion

    public void OnChangeColider(bool isOrigin)
    {
        m_CapsuleCollider[0].enabled = isOrigin;
        m_CapsuleCollider[1].enabled = !isOrigin; //슬라이드 할 때의 영역
    }
    #endregion ======================================== /Set Player Value - Locomotion
}
