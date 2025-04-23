// ========================================250415
// 

// ========================================250410
// Locomotion, Combat�� ���� ���� ���°� �ʿ��� ������ ���µ鿡 ���ؼ� MainState�� ����
// IsAiming, IsSprinting, IsCrouching�� ���� ������ ������ State���� SubFlags�� ����

// ========================================
using UnityEngine;
using DUS.Joystick;
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MainStateAndSubFlagsManager))]
[RequireComponent(typeof(PlayerInputManager))] // InputManager������Ʈ�� ���Ӽ����� PlayerCore�ִ� ���� �ڵ����� �߰�
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

    [Tooltip("���� �ӵ�"),Range(5, 30)] public float m_MaxFallingSpeed = 30;

        [Header("[ Player Rot ]")]
    [Range(1, 20)] public float m_rotationSpeed;
    [Range(1, 50)]public float m_rotationAimSpeed; //���� ���¿����� ȸ�� �ӵ�
    [Range(1, 60)] public float m_rotationDamping; //ȸ�� ����
    #endregion ======================================== /Player Value

    public Rigidbody m_Rigidbody { get; private set; }
    public CapsuleCollider m_CapsuleCollider{  get; private set; }

    //�޾ƿ��� ������ �߿�
    private void Awake()
    {
        //m_Locomotion ������ ���� ���� Get���� ����� ã�Ƴ��� �־����
        m_InputManager = GetComponent<PlayerInputManager>(); // �Ǵ� ���� ����
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
