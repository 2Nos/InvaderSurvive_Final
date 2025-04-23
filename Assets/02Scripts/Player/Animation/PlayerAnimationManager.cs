using UnityEngine;


public enum SetType
{
    SetBool,
    SetTrigger,
    SetFloat
}

[RequireComponent(typeof(Animator))]
public class PlayerAnimationManager : MonoBehaviour
{
    //Awake�� Locomotion���� ������ GetComponent ��� ���� �ֱ�(���� �ٲٴ� ����� ������ �ϴ� �̷���)
    [SerializeField] Animator m_animator;

    private readonly int m_moveSpeedHashX = Animator.StringToHash("MoveDirectionX");
    private readonly int m_moveSpeedHashY = Animator.StringToHash("MoveDirectionY");
    public void SetBool(string name, bool value) => m_animator.SetBool(name, value);
    public void SetTrigger(string name) => m_animator.SetTrigger(name);
    public void SetFloat(string name, float value) => m_animator.SetFloat(name, value);

    private void Awake()
    {
        if (m_animator == null)
        {
            Debug.LogError("Animator not assigned in PlayerAnimationManager.");
        }
    }
    public void UpdateMovementAnimation(Vector2 inputMovement)
    {
        m_animator.SetFloat(m_moveSpeedHashX, inputMovement.x);
        m_animator.SetFloat(m_moveSpeedHashY, inputMovement.y);
    }

    public void UpdateFlagAnimation(LocomotionSubFlags flag, bool active)
    {
        switch (flag)
        {
            case LocomotionSubFlags.Crouch:
                SetBool("IsCrouch", active);
                break;
            case LocomotionSubFlags.Run:
                SetBool("IsRun", active);
                break;
        }
    }

    public void CrossFadeAnimation(string animationName, float transitionDuration = 0.25f)
    {
        m_animator.CrossFade(animationName, transitionDuration);
    }
}