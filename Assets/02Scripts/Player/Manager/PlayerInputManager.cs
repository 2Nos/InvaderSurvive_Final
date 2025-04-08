using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public Vector2 MovementInput { get; private set; }
    public bool IsMoving => MovementInput.sqrMagnitude > 0.01f;
    public Vector2 LookInput { get; private set; } //마우스 회전
    public bool IsAttacking { get; private set; } //마우스 좌클릭
    public bool IsAiming { get; private set; } //마우스 우클릭
    public bool IsSprinting { get; private set; } //왼쪽 쉬프트
    public bool IsCrouching { get; private set; } //왼쪽 컨트롤
    public bool IsJumping { get; private set; } //스페이스바
    public bool IsDodging { get; private set; } //F
    public bool IsUsingSkill { get; private set; } //QE
    public bool IsReloading { get; private set; } //R
    

    // InputSystem_Actions 클래스의 인스턴스
    private PlayerInputAC m_inputActions;

    private void Awake()
    {
        // InputSystem_Actions 인스턴스 생성
        m_inputActions = new PlayerInputAC();
    }

    private void OnEnable()
    {
        // Move 액션에 대한 콜백 등록
        m_inputActions.Player.Move.performed += OnMove;
        m_inputActions.Player.Move.canceled += OnMove;
        m_inputActions.Player.Look.performed += OnLook;
        m_inputActions.Player.Look.canceled += OnLook;
        m_inputActions.Player.Aim.performed += OnAim;
        m_inputActions.Player.Aim.canceled += OnAim;
        m_inputActions.Player.Sprint.performed += OnSprint;
        m_inputActions.Player.Sprint.canceled += OnSprint;
        m_inputActions.Player.Crouch.performed += OnCrouch;
        m_inputActions.Player.Crouch.canceled += OnCrouch;
        m_inputActions.Player.Jump.performed += OnJump;
        m_inputActions.Player.Jump.canceled += OnJump;
        m_inputActions.Player.Dodge.performed += OnDodge;
        m_inputActions.Player.Dodge.canceled += OnDodge;
        m_inputActions.Player.Skill.performed += OnSkill;
        m_inputActions.Player.Skill.canceled += OnSkill;


        // 액션 활성화
        m_inputActions.Enable();
    }

     private void OnDisable()
    {
        // 콜백 해제
        m_inputActions.Player.Move.performed -= OnMove;
        m_inputActions.Player.Move.canceled -= OnMove;
        m_inputActions.Player.Look.performed -= OnLook;
        m_inputActions.Player.Look.canceled -= OnLook;
        m_inputActions.Player.Aim.performed -= OnAim;
        m_inputActions.Player.Aim.canceled -= OnAim;
        m_inputActions.Player.Sprint.performed -= OnSprint;
        m_inputActions.Player.Sprint.canceled -= OnSprint;
        m_inputActions.Player.Crouch.performed -= OnCrouch;
        m_inputActions.Player.Crouch.canceled -= OnCrouch;
        // 액션 비활성화
        m_inputActions.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        IsAiming = context.ReadValueAsButton();
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        IsSprinting = context.ReadValueAsButton();
    }

    private void OnCrouch(InputAction.CallbackContext context)
    {
        IsCrouching = context.ReadValueAsButton();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        IsJumping = context.ReadValueAsButton();
    }

    private void OnDodge(InputAction.CallbackContext context)
    {
        IsDodging = context.ReadValueAsButton();
    }

    private void OnSkill(InputAction.CallbackContext context)
    {
        IsUsingSkill = context.ReadValueAsButton();
    }


}
