// ========================================250415
// 

// ========================================250410
// Locomotion, Combat에 메인 단일 상태가 필요한 메인인 상태들에 대해선 MainState로 관리
// IsAiming, IsSprinting, IsCrouching은 같은 복수가 가능한 State들은 SubFlags로 관리

// ========================================
using UnityEngine;
using System.Collections.Generic;
using DUS.Joystick;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MainStateAndSubFlagsManager))]
[RequireComponent(typeof(PlayerInputManager))] // InputManager컴포넌트를 종속성으로 PlayerCore있는 곳에 자동으로 추가

public class PlayerCore : MonoBehaviour
{
    #region ======================================== Module
    /*private static PlayerCore instance;
    public static PlayerCore Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerCore>();
                if (instance == null)
                {
                    Debug.LogError("PlayerCore instance not found in the scene.");
                }
            }
            return instance;
        }
    }*/
    public PlayerInputManager m_InputManager { get; private set; }
    public MainStateAndSubFlagsManager m_StateFlagManager { get; private set; }
    public PlayerAnimationManager m_AnimationManager { get; private set; }
    public CameraRigManager m_CameraManager { get; private set; }
    public PlayerLocomotion m_Locomotion { get; private set; }
    public PlayerCombat m_Combat { get; private set; }
    #endregion ======================================== /Module

    #region ======================================== Player Value - Locomotion
    [Header("[ PlayerCore Move ]")]
    //SetCurrentSpeed
    [Range(1, 10)] public float m_walkSpeed = 5f;
    [Range(1, 20)] public float m_runSpeed = 15f;
    [Range(1, 10)] public float m_crouchSpeed = 3f;
    [Range(1, 10)] public float m_crouchRunSpeed = 9f;
    [Range(20, 50)] public float m_sprintSpeed = 20f;
    [Range(1, 10)] public float m_DodgeSpeed = 5f;
    [Range(1, 10)] public float m_SlideSpeed = 5f;
    [Range(1, 10)] public float m_ClimbSpeed = 5f;
    [Range(1, 10)] public float m_WallRunSpeed = 5f;
    [Range(1, 10)] public float m_jumpSpeed = 5f;
    [Range(1, 100)] public float m_jumpUpForce = 5f;
    [Range(-10, 0.1f)] public float m_Gravity = -9.8f;
    public float m_CurrentSpeed { get; private set; } //현재 속도

    [Range(1, 50)] public float m_MaxVelocityY = 30;
    [Range(-50, -1)] public float m_MinVelocityY = -30;

    [Header("[ PlayerCore Rot ]")]
    [Range(1, 20)] public float m_rotationSpeed = 10;
    [Range(1, 60)] public float m_rotationDamping; //회전 감속
    #endregion ======================================== /Player Value Locomotion

    [Range(1, 50)] public float m_rotationAimSpeed; //에임 상태에서의 회전 속도
    public LayerMask m_GroundMask;
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

        m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }
    private void Start()
    {
        OnChangeColider(true);
        m_Locomotion.InitializeLocomotion();
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
