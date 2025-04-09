using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerAbilityManager))]
[RequireComponent(typeof(PlayerInventoryManager))]
[RequireComponent(typeof(PlayerBodyManager))]
[RequireComponent(typeof(PlayerInputManager))] // PlayerInputManager컴포넌트를 종속성으로 자동으로 추가
public class PlayerLocomotion : MonoBehaviour
{
    #region [PlayerSetting]
    [SerializeField] private float m_moveSpeed = 5f;
    [SerializeField] private float m_sprintSpeed = 8f;
    [SerializeField] private float m_crouchSpeed = 2.5f;
    
    #endregion

    #region [FlagManager]
    [SerializeField] private FlagManager m_flagManager;
    #endregion

    #region [Camera]
    private CameraManager m_cameraManager;
    #endregion

    #region  [PlayerConfig]
    private PlayerInputManager m_inputManager;
    private PlayerAnimationManager m_animationManager;
    private PlayerInventoryManager m_inventoryManager;
    private PlayerAbilityManager m_abilityManager;
    private PlayerState m_currentState;
    #endregion

    #region [Weapon]
    [SerializeField] private WeaponManager m_weaponManager;
    #endregion

    private Vector3 m_moveDirection;
    private float m_currentMoveSpeed;
    private bool m_isAir;            // 공중 상태(점프 / 낙하)
    private bool m_isGround;         // 착지 상태
    private bool m_isSliding;        //달리는 중 앉기
    private bool m_isClimbing;       //벽에서 F키 눌렀을 때?
    private bool m_isWallRunning;    //벽에서 달리기 상호 작용


    // Getter 메서드들 추가
    public PlayerInputManager GetInputManager() => m_inputManager;
    public PlayerAnimationManager GetAnimationManager() => m_animationManager;
    public PlayerInventoryManager GetInventoryManager() => m_inventoryManager;
    public PlayerAbilityManager GetAbilityManager() => m_abilityManager;
    public WeaponManager GetWeaponManager() => m_weaponManager;
    public FlagManager GetFlagManager() => m_flagManager;
    
    // 상태 관련
    public bool IsAir() => m_isAir;
    public bool IsGround() => m_isGround;
    public bool IsSliding() => m_isSliding;
    public bool IsClimbing() => m_isClimbing;
    public bool IsWallRunning() => m_isWallRunning;

    private void Awake()
    {
        m_inputManager = GetComponent<PlayerInputManager>();
        m_animationManager = GetComponentInChildren<PlayerAnimationManager>();
        m_inventoryManager = GetComponent<PlayerInventoryManager>();
        m_abilityManager = GetComponent<PlayerAbilityManager>();
        m_weaponManager = FindObjectOfType<WeaponManager>();
        m_cameraManager = Camera.main.gameObject.GetComponent<CameraManager>();
        m_flagManager = FindObjectOfType<FlagManager>();
        // 초기 상태 설정
        ChangeState(new IdleState(this));
    }


    private void Update()
    {
        m_currentState?.Update(); // 현재 상태가 있으면, 해당 State의 업데이트를 진행
    }

    // 상태 변경
    public void ChangeState(PlayerState nextState)
    {
        m_currentState?.Exit();
        m_currentState = nextState;
        m_currentState.Enter();
    }

    #region Movement
    // 이동 처리
    public void HandleMovement()
    {
        if (!m_inputManager.IsMoving)
        {
            m_moveDirection = Vector3.zero;
            m_currentMoveSpeed = 0f;
            m_animationManager.UpdateMovementAnimation(Vector3.zero);
            return;
        }

        // 이동 속도 결정
        m_currentMoveSpeed = m_inputManager.IsSprinting ? m_sprintSpeed :
                           m_inputManager.IsCrouching ? m_crouchSpeed :
                           m_moveSpeed;

        // 이동 방향 계산
        Vector2 moveInput = m_inputManager.MovementInput;
        m_moveDirection = (transform.right * moveInput.x + transform.forward * moveInput.y) * m_currentMoveSpeed;
        m_moveDirection.Normalize();
        // 이동 적용
        transform.position += m_moveDirection * Time.deltaTime;

        //이동 애니메이션
        m_animationManager.UpdateMovementAnimation(m_inputManager.MovementInput);
    }

    // 회전 처리
    public void HandleRotation()
    {
        // 나중에 적용
        /*Vector3 lookDirection = new Vector3(m_inputManager.LookInput.x, 0, m_inputManager.LookInput.y);

        if(lookDirection.sqrMagnitude > 0.01f)
        {
            Vector3 worldDirection = Camera.main.transform.TransformDirection(lookDirection);
            worldDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(worldDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }*/
        Vector3 playerEuler = transform.eulerAngles;
        playerEuler.y = m_cameraManager.m_yaw;
        transform.rotation = Quaternion.Euler(playerEuler);
    }
    #endregion


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground")) { m_isGround = true; }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground")) { m_isGround = false; }
    }
}
