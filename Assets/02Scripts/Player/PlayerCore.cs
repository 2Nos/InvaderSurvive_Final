// ========================================250415
// 

// ========================================250410
// Locomotion, Combat�� ���� ���� ���°� �ʿ��� ������ ���µ鿡 ���ؼ� MainState�� ����
// IsAiming, IsSprinting, IsCrouching�� ���� ������ ������ State���� SubFlags�� ����

// ========================================
using UnityEngine;
using System.Collections.Generic;
using DUS.Joystick;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MainStateAndSubFlagsManager))]
[RequireComponent(typeof(PlayerInputManager))] // InputManager������Ʈ�� ���Ӽ����� PlayerCore�ִ� ���� �ڵ����� �߰�

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
    public float m_CurrentSpeed { get; private set; } //���� �ӵ�

    [Range(1, 50)] public float m_MaxVelocityY = 30;
    [Range(-50, -1)] public float m_MinVelocityY = -30;

    [Header("[ PlayerCore Rot ]")]
    [Range(1, 20)] public float m_rotationSpeed = 10;
    [Range(1, 60)] public float m_rotationDamping; //ȸ�� ����
    #endregion ======================================== /Player Value Locomotion

    [Range(1, 50)] public float m_rotationAimSpeed; //���� ���¿����� ȸ�� �ӵ�
    public LayerMask m_GroundMask;
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
