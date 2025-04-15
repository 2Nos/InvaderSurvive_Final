using UnityEngine;
using DUS.Joystick;


[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInputManager))] // InputManager컴포넌트를 종속성으로 PlayerCore있는 곳에 자동으로 추가
public class PlayerCore : MonoBehaviour
{
    /*#region SingleTon
    private static PlayerCore instance;
    public static PlayerCore Instance
    {
        get { return instance; }
        set
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerCore>();
            }
        }
    }
    #endregion*/

    #region ======================================== Module
    public PlayerLocomotion m_Locomotion { get; private set; }
    public PlayerCombat m_Combat { get; private set; }

    public PlayerInputManager m_InputManager { get; private set; }
    public DUS.Joystick.Joystick m_Joystick { get; private set; } // 안드로이드 조이스틱
    public MainStateAndSubFlagsManager m_StateFlagManager { get; private set; }
    public PlayerAnimationManager m_AnimationManager { get; private set; }
    public CameraManager m_CameraManager { get; private set;}

    #endregion ======================================== /Module

    #region ======================================== Player Value
    [Header("Player Value")]
    public float m_moveSpeed; //{ get { return m_moveSpeed; } private set { if (value == 0) m_moveSpeed = 2.0f; } }
    public float m_sprintSpeed; //{ get { return m_sprintSpeed; } private set { if (value == 0) m_sprintSpeed = 8.0f; } }
    public float m_crouchSpeed; //{ get { return m_crouchSpeed; } private set { if (value == 0) m_crouchSpeed = 2.5f; } }

    //Rotation 스피드 설정의 경우 Camera에서 조절 끝
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
        m_CameraManager = FindObjectOfType<CameraManager>();

        m_StateFlagManager = new MainStateAndSubFlagsManager();

        m_Locomotion = new PlayerLocomotion(this);
        m_Combat = new PlayerCombat(this);
    }

    private void Update()
    {
        m_Locomotion?.Update();
        m_Combat?.Update();
    }
}
