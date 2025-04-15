using UnityEngine;
using UnityEngine.InputSystem;
using DUS.Joystick;

public enum InputType
{
    Keyboard,
    Android,
}

public class PlayerInputManager : MonoBehaviour
{
    public InputType m_InputType;

    // Move
    public Vector2 m_MovementInput { get; private set; }
    public bool m_IsMoving => m_MovementInput.sqrMagnitude > 0.01f;


    public Vector2 m_LookInput { get; private set; }      //Mouse_Rot(DeltaValue)
    public bool m_IsAttacking { get; private set; }       //Mouse_L
    public bool m_IsAiming { get; private set; }          //Mouse_R
    public bool m_IsSprinting { get; private set; }       //Left Shift
    public bool m_IsCrouching { get; private set; }       //C
    public bool m_IsJumping { get; private set; }         //Space bar
    public bool m_IsDodging { get; private set; }         //Left Ctrl
    public bool m_IsUsingSkill { get; private set; }      //QE , Ability
    public bool m_IsReloading { get; private set; }       //R
    public bool m_IsInteraction { get; private set; }     //F 대부분의 상호작용

    // InputSystem_Actions 클래스의 인스턴스
    private PlayerInputAC m_inputActions;
    private void Awake()
    {
        // InputSystem_Actions 인스턴스 생성
        m_inputActions = new PlayerInputAC();
    }

    private void OnEnable()
    {
        if (m_InputType == InputType.Android) return;
        // m_IsMoving 액션에 대한 콜백 등록
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

        m_inputActions.Player.Attack.performed += OnAttack;
        m_inputActions.Player.Attack.canceled += OnAttack;

        m_inputActions.Player.Dodge.performed += OnDodge;
        m_inputActions.Player.Dodge.canceled += OnDodge;

        m_inputActions.Player.Skill.performed += OnSkill;
        m_inputActions.Player.Skill.canceled += OnSkill;

        m_inputActions.Player.Reload.performed += OnReload;
        m_inputActions.Player.Reload.canceled += OnReload;

        // 액션 활성화
        m_inputActions.Enable();
    }

     private void OnDisable()
    {
        if (m_InputType == InputType.Android) return;
        // 콜백 해제
        m_inputActions.Player.Move.performed -= OnMove;
        m_inputActions.Player.Move.canceled -= OnMove;

        m_inputActions.Player.Look.performed -= OnLook;
        m_inputActions.Player.Look.canceled -= OnLook;

        m_inputActions.Player.Aim.performed -= OnAim;
        m_inputActions.Player.Aim.canceled -= OnAim;

        m_inputActions.Player.Sprint.performed -= OnSprint;
        m_inputActions.Player.Sprint.canceled -= OnSprint;

        m_inputActions.Player.Crouch.performed -= OnJump;
        m_inputActions.Player.Crouch.canceled -= OnJump;

        m_inputActions.Player.Jump.performed -= OnCrouch;
        m_inputActions.Player.Jump.canceled -= OnCrouch;

        m_inputActions.Player.Attack.performed -= OnAttack;
        m_inputActions.Player.Attack.canceled -= OnAttack;

        m_inputActions.Player.Skill.performed -= OnSkill;
        m_inputActions.Player.Skill.canceled -= OnSkill;

        m_inputActions.Player.Reload.performed -= OnReload;
        m_inputActions.Player.Reload.canceled -= OnReload;

        // 액션 비활성화
        m_inputActions.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        m_MovementInput = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        m_LookInput = context.ReadValue<Vector2>();
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        m_IsAiming = context.ReadValueAsButton();
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        m_IsSprinting = context.ReadValueAsButton();
    }

    private void OnCrouch(InputAction.CallbackContext context)
    {
        m_IsCrouching = context.ReadValueAsButton();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        m_IsJumping = context.ReadValueAsButton();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        m_IsAttacking = context.ReadValueAsButton();
    }

    private void OnDodge(InputAction.CallbackContext context)
    {
        m_IsDodging = context.ReadValueAsButton();
    }

    private void OnSkill(InputAction.CallbackContext context)
    {
        m_IsUsingSkill = context.ReadValueAsButton();
    }
    private void OnReload(InputAction.CallbackContext context)
    {
        m_IsReloading = context.ReadValueAsButton();
    }

    public void SetInput(Vector2 inputDir, JoystickType joystickType)
    {
       switch(joystickType)
        {
            case JoystickType.Move:
                m_MovementInput = inputDir*2;
                break;
            case JoystickType.Rotate:
                m_LookInput = inputDir*2;
                break;
        }
    }

}
