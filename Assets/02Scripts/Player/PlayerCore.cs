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
    [Range(1, 10)] public float m_walkSpeed = 5f;
    [Range(1, 20)] public float m_runSpeed = 15f;
    [Range(1, 10)] public float m_crouchSpeed = 3f;
    [Range(1, 10)] public float m_crouchRunSpeed = 9f;
    [Range(20, 50)] public float m_sprintSpeed = 20f;
    [Range(1, 10)] public float m_DodgeSpeed = 5f;
    [Range(1, 10)] public float m_SlideSpeed = 5f;
    [Range(1, 10)] public float m_ClimbSpeed = 5f;
    [Range(1, 10)] public float m_WallRunSpeed = 5f;

    [Header("[ Jump ]")]
    [Range(1, 100)] public float m_JumpForce = 10f;
    [SerializeField] public float m_JumpDuration = 1f;
    [SerializeField] public AnimationCurve m_JumpCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f); // m_JumpDuration 동안 1 → 0 로 곡선 변환

    [Header("[ Gravity ]")]
    [Range(1, 10)] public float m_AddGravity = 1.5f;
    public LayerMask m_GroundMask;
    public float m_CurrentSpeed { get; private set; } //현재 속도

    [Range(1, 50)] public float m_MaxVelocityY = 30;
    [Range(-50, -1)] public float m_MinVelocityY = -30;

    [Header("[ PlayerCore Rot ]")]
    [Range(1, 20)] public float m_rotationSpeed = 10;
    [Range(1, 60)] public float m_rotationDamping; //회전 감속
    #endregion ======================================== /Player Value Locomotion

    #region ======================================== Player Value - Combat
    [Range(1, 50)] public float m_rotationAimSpeed; //에임 상태에서의 회전 속도

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

        //생성자 this를 넣지 않는 이유는 생명주기에 의한 내부 코드 복잡도 발생있기에 Initialize로 다음에 처리
        m_Locomotion = new PlayerLocomotion(this);
        //m_Combat = new PlayerCombat(this);

        m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous; //리지드바디 감지 모드 변경
        Application.targetFrameRate = 300; //Fixed 프레임 변경
    }
    private void Start()
    {
        OnChangeColider(true);
        m_Locomotion.InitializeLocomotionAtStart();
        m_CurrentSpeed = m_walkSpeed;
        m_CurrentRotSpeed = m_rotationSpeed;
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


    #region ======================================== Set Player Value - Locomotion
    public void SetCurrentMoveSpeed(float speed)
    {
        m_CurrentSpeed = speed;
    }
    public void SetCurrentRotSpeed(float speed)
    {
        m_CurrentRotSpeed = speed;
    }

    public void SetRigidVelocity(Vector3 velocity)
    {
        m_Rigidbody.linearVelocity = velocity;
    }
    public void OnChangeColider(bool isOrigin)
    {
        m_CapsuleCollider[0].enabled = isOrigin;
        m_CapsuleCollider[1].enabled = !isOrigin; //슬라이드 할 때의 영역
    }

    public void SetRigidVelocityY(float velocityY)
    {
        Vector3 velocity = m_Rigidbody.linearVelocity;
        velocity.y = velocityY;
        m_Rigidbody.linearVelocity = velocity;
    }
    #endregion ======================================== /Set Player Value - Locomotion
}
