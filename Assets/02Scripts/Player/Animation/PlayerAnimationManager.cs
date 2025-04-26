using UnityEngine;


public enum AniParmType
{
    None = 0,
    SetBool,
    SetTrigger,
    SetInt,
    SetFloat
}

[RequireComponent(typeof(Animator))]
public class PlayerAnimationManager : MonoBehaviour
{
    //Awake가 Locomotion보다 느려서 GetComponent 대신 직접 넣기(순서 바꾸는 방법은 있으나 일단 이렇게)
    public Animator m_Animator { get; private set; }

    private readonly int m_moveSpeedHashX = Animator.StringToHash("MoveDirectionX");
    private readonly int m_moveSpeedHashY = Animator.StringToHash("MoveDirectionY");
    public void SetParmBool(string name, bool value) => m_Animator.SetBool(name, value);
    public void SetParmTrigger(string name) => m_Animator.SetTrigger(name);
    public void SetParmFloat(string name, float value) => m_Animator.SetFloat(name, value);
    public void SetParmInt(string name, int value) => m_Animator.SetInteger(name, value);

    private void Awake()
    {
        if (m_Animator == null)
        {
            m_Animator = GetComponent<Animator>();
        }
        
    }
    public void UpdateMovementAnimation(Vector2 inputMovement)
    {
        m_Animator.SetFloat(m_moveSpeedHashX, inputMovement.x);
        m_Animator.SetFloat(m_moveSpeedHashY, inputMovement.y);
    }

    /* public void UpdateFlagAnimation(LocomotionSubFlags flag, bool active)
     {
         switch (flag)
         {
             case LocomotionSubFlags.Crouch:
                 SetParmBool("IsCrouch", active);
                 break;
             case LocomotionSubFlags.Run:
                 SetParmBool("IsRun", active);
                 break;
         }
     }*/

    public void CrossFadeAnimation(string animationName, float transitionDuration = 0.25f)
    {
        m_Animator.CrossFade(animationName, transitionDuration);
    }

}