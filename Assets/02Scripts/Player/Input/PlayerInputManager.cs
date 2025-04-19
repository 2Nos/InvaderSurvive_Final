// ========================================250417
// 플랫폼 입력 처리에 대한 완전한 관리는 PlayerInputManager에서 처리

// ========================================
using UnityEngine;
using UnityEngine.InputSystem;
using DUS.Joystick;
using UnityEditor.XR;
using Unity.VisualScripting;

public enum InputType
{
    Keyboard,
    Android,
}

public class PlayerInputManager : MonoBehaviour
{
    public InputType m_InputType;
    public StateButtonGroupManager m_FlagButtonManager;
    public MainStateAndSubFlagsManager m_StateFlagManager;

    // Move
    public Vector2 m_MovementInput { get; private set; }

    public bool m_IsMove => m_MovementInput.sqrMagnitude > 0.01f;

    public Vector2 m_LookInput { get; private set; }        //Mouse_Rot(DeltaValue)
    public bool m_IsAttacking { get; private set; }         //Mouse_L
    public bool m_IsAim { get; private set; }               //Mouse_R
    public bool m_IsRun { get; private set; }               //Left Shift
    public bool m_IsCrouch { get; private set; }            // Left Ctrl
    public bool m_IsInAir { get; private set; }             //Space bar
    public bool m_IsDodging { get; private set; }           // C
    public bool m_IsUsingSkill { get; private set; }        //QE , Ability
    public bool m_IsReloading { get; private set; }         //R
    public bool m_IsInteraction { get; private set; }       //F 대부분의 상호작용
    public bool m_IsChangeLeftView { get; private set; }    //에임 좌우 반전
    public bool m_IsStopCameraRot { get; private set; }     //카메라 회전 중지

    // InputSystem_Actions 클래스의 인스턴스
    private PlayerInputAC m_inputActions;

    private void Awake()
    {
        // InputSystem_Actions 인스턴스 생성
        m_inputActions = new PlayerInputAC();

        if (m_InputType != InputType.Android)
        {
            Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 잠금
        }
    }

    #region ======================================== PC, GamePad

    private void OnEnable()
    {
        if (m_InputType == InputType.Android) return;
        // m_IsMove 액션에 대한 콜백 등록
        m_inputActions.Player.Move.performed += OnMove;
        m_inputActions.Player.Move.canceled += OnMove;

        m_inputActions.Player.Look.performed += OnLook;
        m_inputActions.Player.Look.canceled += OnLook;

        m_inputActions.Player.Aim.performed += OnAim;
        m_inputActions.Player.Aim.canceled += OnAim;

        m_inputActions.Player.Run.performed += OnRun;
        m_inputActions.Player.Run.canceled += OnRun;

        m_inputActions.Player.Crouch.performed += OnCrouch;
        //m_inputActions.Player.Crouch.canceled += OnCrouch;

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

        m_inputActions.Player.StopCameraRot.performed += OnStopCameraRot;
        m_inputActions.Player.StopCameraRot.canceled += OnStopCameraRot;

        m_inputActions.Player.ChangeLeftView.performed += OnChangeLeftView;
        m_inputActions.Player.ChangeLeftView.canceled += OnChangeLeftView;

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

        m_inputActions.Player.Run.performed -= OnRun;
        m_inputActions.Player.Run.canceled -= OnRun;

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

        m_inputActions.Player.StopCameraRot.performed -= OnStopCameraRot;
        m_inputActions.Player.StopCameraRot.canceled -= OnStopCameraRot;

        m_inputActions.Player.ChangeLeftView.performed -= OnChangeLeftView;
        m_inputActions.Player.ChangeLeftView.performed -= OnChangeLeftView;
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
        m_IsAim = context.ReadValueAsButton();
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        m_IsRun = context.ReadValueAsButton();

        if(m_IsRun)
        {
            m_StateFlagManager.SetLocomotionFlag(LocomotionSubFlags.Run);
        }
        else
        {
            m_StateFlagManager.ClearLocomotionFlag(LocomotionSubFlags.Run);
        }
    }

    private void OnCrouch(InputAction.CallbackContext context)
    {
        m_IsCrouch = !m_IsCrouch;

        if (m_IsCrouch)
        {
            m_StateFlagManager.SetLocomotionFlag(LocomotionSubFlags.Crouch);
        }
        else
        {
            m_StateFlagManager.ClearLocomotionFlag(LocomotionSubFlags.Crouch);
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        m_IsInAir = context.ReadValueAsButton();
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

    private void OnStopCameraRot(InputAction.CallbackContext context)
    {
        m_IsStopCameraRot = context.ReadValueAsButton();
    }

    private void OnChangeLeftView(InputAction.CallbackContext context)
    {
        m_IsChangeLeftView = context.ReadValueAsButton();
    }

    #endregion ======================================== PC, GamePad

    #region ======================================== Android

    public void SetMovementAndLookInput(Vector2 inputDir, JoystickType joystickType)
    {
        switch (joystickType)
        {
            case JoystickType.Move:
                m_MovementInput = inputDir * 2;
                break;

            case JoystickType.Rotate:
                m_LookInput = inputDir * 2;
                break;
        }
    }

    #region ======================================== InputFlags

    public void SetIsRunInput(bool isRun)
    {
        m_IsRun = isRun;
        switch (m_IsRun)
        {
            case true:
                m_StateFlagManager.SetLocomotionFlag(LocomotionSubFlags.Run);
                break;

            case false:
                m_StateFlagManager.ClearLocomotionFlag(LocomotionSubFlags.Run);
                break;
        }
    }

    public void SetIsCrouchInput(bool isCrouch)
    {
        m_IsCrouch = isCrouch;
        switch (m_IsCrouch)
        {
            case true:
                m_StateFlagManager.SetLocomotionFlag(LocomotionSubFlags.Crouch);
                break;

            case false:
                m_StateFlagManager.ClearLocomotionFlag(LocomotionSubFlags.Crouch);
                break;
        }
    }

    #endregion ======================================== Flags

    #region ======================================== LocomotionMainState

    public void SetIsInAirInput(bool isJump)
    {
        m_IsInAir = isJump;
    }

    public void SetIsDodgeInput(bool isDodge)
    {
        m_IsDodging = isDodge;
    }

    #endregion ======================================== LocomotionMainState

    #region ======================================== CombatMainState

    public void SetIsAttackInput(bool isAttack)
    {
        if (m_StateFlagManager.m_CombatMain == CombatMainState.Shooting)
        {
            m_IsAttacking = isAttack;
            //m_StateFlagManager.SetCombatFlag(.Aming);
        }
        else if (m_StateFlagManager.m_CombatMain == CombatMainState.MeleeAttacking)
        {
            m_IsAttacking = isAttack;
            //m_StateFlagManager.SetCombatFlag(CombatSubFlags.MeleeAttacking);
        }
        else
            m_IsAttacking = isAttack;
    }

    #endregion ======================================== CombatMainState

    public void SetIsSkillInput(bool isSkill)
    {
        m_IsUsingSkill = isSkill;
    }

    public void SetIsReloadInput(bool isReload)
    {
        m_IsReloading = isReload;
    }

    public void SetIsInteractionInput(bool isInteraction)
    {
        m_IsInteraction = isInteraction;
    }

    public void SetIsAimInput(bool isAim)
    {
        m_IsAim = isAim;
    }

    #endregion ======================================== Android
}