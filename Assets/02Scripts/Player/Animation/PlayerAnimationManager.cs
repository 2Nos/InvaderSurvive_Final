using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public float m_AnimationTime = 0f; // 현재 애니메이션의 길이

    [HideInInspector]public bool m_IsJumpStart;
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

    // 애니메이션을 Set으로 동작 시켰어도 애니메이션은 1프레임 후에 동작이 이루어지기 때문에 
    // Enter에서 애니메이션을 동작 시켜도 이전 애니메이션의 시간으로 체크가 되기에 확실하게 넘어간 후 잡아주어야한다.
    // Enter에서 해보았지만 안됨 Update에서 그냥 체크하기로
    /*public async Task<float> CheckAnimationTime(string animationName)
    {
        // 애니메이션 상태가 바뀔 때까지 기다림 (동기화 위해 1프레임 대기)
        await Task.Yield();
        await Task.Delay(50); // 50ms 대기 추가
        AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);

        // 상태 이름이 일치하는 경우에만 애니메이션 길이를 리턴
        if (stateInfo.IsName(animationName))
        {
            return stateInfo.length;
        }
        return 0f; // 애니메이션이 아직 바뀌지 않았을 경우
    }*/

    public void CheckJumpUp()
    {
        m_IsJumpStart = true;
    }
}