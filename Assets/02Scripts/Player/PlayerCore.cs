// ========================================250415
// 

// ========================================250410
// Locomotion, Combat�� ���� ���� ���°� �ʿ��� ������ ���µ鿡 ���ؼ� MainState�� ����
// IsAiming, IsSprinting, IsCrouching�� ���� ������ ������ State���� SubFlags�� ����

// ========================================
using DUS.Player.Locomotion;
using UnityEngine;
using System.Collections.Generic;
using DUS.Joystick;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MainStateAndSubFlagsManager))]
[RequireComponent(typeof(PlayerInputManager))] // InputManager������Ʈ�� ���Ӽ����� PlayerCore�ִ� ���� �ڵ����� �߰�

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
    [SerializeField] public AnimationCurve m_JumpCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f); // m_JumpDuration ���� 1 �� 0 �� � ��ȯ

    [Header("[ Gravity ]")]
    [Range(1, 10)] public float m_AddGravity = 1.5f;
    public LayerMask m_GroundMask;
    public float m_CurrentSpeed { get; private set; } //���� �ӵ�

    [Range(1, 50)] public float m_MaxVelocityY = 30;
    [Range(-50, -1)] public float m_MinVelocityY = -30;

    [Header("[ PlayerCore Rot ]")]
    [Range(1, 20)] public float m_rotationSpeed = 10;
    [Range(1, 60)] public float m_rotationDamping; //ȸ�� ����
    #endregion ======================================== /Player Value Locomotion

    #region ======================================== Player Value - Combat
    [Range(1, 50)] public float m_rotationAimSpeed; //���� ���¿����� ȸ�� �ӵ�

    #endregion ======================================== /Player Value Combat
    public Rigidbody m_Rigidbody { get; private set; }
    public CapsuleCollider[] m_CapsuleCollider { get; private set; }
    public float m_CurrentRotSpeed { get; private set; }

    //�޾ƿ��� ������ �߿�
    private void Awake()
    {
        //instance = this;
        //m_Locomotion ������ ���� ���� Get���� ����� ã�Ƴ��� �־����
        m_InputManager = GetComponent<PlayerInputManager>(); // �Ǵ� ���� ����
        m_AnimationManager = GetComponentInChildren<PlayerAnimationManager>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_CapsuleCollider = GetComponents<CapsuleCollider>();
        m_CameraManager = FindObjectOfType<CameraRigManager>();

        m_StateFlagManager = GetComponent<MainStateAndSubFlagsManager>();

        //������ this�� ���� �ʴ� ������ �����ֱ⿡ ���� ���� �ڵ� ���⵵ �߻��ֱ⿡ Initialize�� ������ ó��
        m_Locomotion = new PlayerLocomotion(this);
        //m_Combat = new PlayerCombat(this);

        m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous; //������ٵ� ���� ��� ����
        Application.targetFrameRate = 300; //Fixed ������ ����
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
        m_CapsuleCollider[1].enabled = !isOrigin; //�����̵� �� ���� ����
    }

    public void SetRigidVelocityY(float velocityY)
    {
        Vector3 velocity = m_Rigidbody.linearVelocity;
        velocity.y = velocityY;
        m_Rigidbody.linearVelocity = velocity;
    }
    #endregion ======================================== /Set Player Value - Locomotion
}
