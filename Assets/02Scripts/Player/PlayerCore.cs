using UnityEngine;
using UnityEngine.Windows;

public class PlayerCore : MonoBehaviour
{
    private InputManager m_InputManager;
    private MainStateAndSubFlagsManager m_FlagManager;
    private PlayerAnimationManager m_AnimationManager;
    
    private Rigidbody m_Rigidbody;
    private CapsuleCollider m_CapsuleCollider;

    public PlayerAnimationManager GetAnimationManager() => m_AnimationManager;
    public MainStateAndSubFlagsManager GetFlagManager() => m_FlagManager;
    public InputManager GetInputManager() => m_InputManager;

    private void Awake()
    {
        m_FlagManager = new MainStateAndSubFlagsManager();
        m_AnimationManager = GetComponent<PlayerAnimationManager>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_CapsuleCollider = GetComponent<CapsuleCollider>();
        m_InputManager = new InputManager(); // 또는 직접 주입
    }
}
