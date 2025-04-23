// ========================================250415
// 

// ========================================250410
// Locomotion, Combat에 메인 단일 상태가 필요한 메인인 상태들에 대해선 MainState로 관리
// IsAiming, IsSprinting, IsCrouching은 같은 복수가 가능한 State들은 SubFlags로 관리

// ========================================
using UnityEngine;
using DUS.Joystick;
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MainStateAndSubFlagsManager))]
[RequireComponent(typeof(PlayerInputManager))] // InputManager컴포넌트를 종속성으로 PlayerCore있는 곳에 자동으로 추가
public class PlayerCore : MonoBehaviour
{
    #region ======================================== Module
    public PlayerInputManager m_InputManager { get; private set; }
    public MainStateAndSubFlagsManager m_StateFlagManager { get; private set; }
    public PlayerAnimationManager m_AnimationManager { get; private set; }
    public CameraRigManager m_CameraManager { get; private set; }
    public PlayerLocomotion m_Locomotion { get; private set; }
    public PlayerCombat m_Combat { get; private set; }
    public LayerMask m_GroundMask;


    #endregion ======================================== /Module

    #region ======================================== Player Value
    [Header("[ Player Move ]")]
    [Range(1, 10)] public float m_moveSpeed = 5f;
    [Range(1, 20)] public float m_runSpeed = 15f;
    [Range(1, 10)] public float m_crouchSpeed = 3f;
    [Range(1, 10)] public float m_crouchRunSpeed = 9f;
    [Range(20, 50)] public float m_sprintSpeed = 20f;
    [Range(1, 100)] public float m_jumpForce = 5f;


    [Range(-10, 0.1f)] public float m_Gravity = -9.8f;

    [Tooltip("낙하 속도"),Range(5, 30)] public float m_MaxFallingSpeed = 30;

        [Header("[ Player Rot ]")]
    [Range(1, 20)] public float m_rotationSpeed;
    [Range(1, 50)]public float m_rotationAimSpeed; //에임 상태에서의 회전 속도
    [Range(1, 60)] public float m_rotationDamping; //회전 감속
    #endregion ======================================== /Player Value

    public Rigidbody m_Rigidbody { get; private set; }
    public CapsuleCollider m_CapsuleCollider{  get; private set; }

    //받아오는 순서가 중요
    private void Awake()
    {
        //m_Locomotion 생성자 전에 먼저 Get으로 모듈을 찾아놓고 있어야함
        m_InputManager = GetComponent<PlayerInputManager>(); // 또는 직접 주입
        m_AnimationManager = GetComponentInChildren<PlayerAnimationManager>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_CapsuleCollider = GetComponent<CapsuleCollider>();
        m_CameraManager = FindObjectOfType<CameraRigManager>();

        m_StateFlagManager = GetComponent<MainStateAndSubFlagsManager>();

        m_Locomotion = new PlayerLocomotion(this);
        m_Combat = new PlayerCombat(this);

        m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }
    private void Start()
    {
        m_Locomotion.InitState();
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
}
