using UnityEngine;

[RequireComponent(typeof(PlayerBodyManager))]
[RequireComponent(typeof(PlayerInputManager))] // PlayerInputManager컴포넌트를 종속성으로 자동으로 추가
public class PlayerLocomotion : MonoBehaviour
{

    [SerializeField] private float m_moveSpeed = 5f;
    [SerializeField] private float m_sprintSpeed = 8f;
    [SerializeField] private float m_crouchSpeed = 2.5f;

    private PlayerInputManager m_inputManager;
    private PlayerBodyManager m_bodyManager;
    private PlayerAnimationManager m_animationManager;
    private PlayerBodyState m_currentState;

    private Vector3 m_moveDirection;
    private float m_currentMoveSpeed;

  // Getter 메서드들 추가
    public PlayerInputManager GetInputManager() => m_inputManager;
    public PlayerBodyManager GetBodyManager() => m_bodyManager;
    public Vector3 GetMoveDirection() => m_moveDirection;
    public float GetCurrentMoveSpeed() => m_currentMoveSpeed;
    public PlayerAnimationManager GetAnimationManager() => m_animationManager;

     private void Awake()
    {
        m_inputManager = GetComponent<PlayerInputManager>();
        m_bodyManager = GetComponent<PlayerBodyManager>(); // GetComponent 사용
        m_animationManager = GetComponentInChildren<PlayerAnimationManager>();
        // 초기 상태 설정
        ChangeState(new PlayerBodyIdleState(this));
    }

    // Update is called once per frame
    private void Update()
    {
        m_currentState?.Update();
    }

    public void ChangeState(PlayerBodyState nextState)
    {
        m_currentState?.Exit();
        m_currentState = nextState;
        m_currentState.Enter();
    }

    public void HandleMovement()
    {
         if (!m_inputManager.IsMoving)
        {
            m_moveDirection = Vector3.zero;
            m_currentMoveSpeed = 0f;
            return;
        }

        // 이동 속도 결정
        m_currentMoveSpeed = m_inputManager.IsSprinting ? m_sprintSpeed :
                           m_inputManager.IsCrouching ? m_crouchSpeed :
                           m_moveSpeed;

        // 이동 방향 계산
        Vector2 moveInput = m_inputManager.MovementInput;
        m_moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized * m_currentMoveSpeed;

        // 이동 적용
        transform.position += m_moveDirection * Time.deltaTime;

        // 하체 회전 업데이트
        m_bodyManager.UpdateLowerBodyRotation(moveInput);
    }

    public void HandleRotation()
    {
        if (m_inputManager.IsAiming)
        {
            m_bodyManager.UpdateUpperBodyRotation(m_inputManager.LookInput);
        }
    }

}
