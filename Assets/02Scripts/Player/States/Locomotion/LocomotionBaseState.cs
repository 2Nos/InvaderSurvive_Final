using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class LocomotionBaseState
{
    protected PlayerCore m_PlayerCore;

    //playerCore �ѱ�� ���� ������, ��� �� �Ű������� ���� �����ڰ� �ڵ� ������ ���� �ʱ⿡ ���� ������ �ۼ��������.
    protected LocomotionBaseState(PlayerCore playerCore)
    {
        m_PlayerCore = playerCore;
    }

    protected abstract LocomotionMainState GetMainState();

    public virtual void Enter()
    {
        m_PlayerCore.m_StateFlagManager.m_LocomotionMain = GetMainState();
        //�ִϸ��̼� �Ķ���� On
        if (SetAnimationBoolName() != null)
            m_PlayerCore.m_AnimationManager.SetBool(SetAnimationBoolName(), true);

    }

    public virtual void Update()
    {
        Movement();
    }

    public virtual void Exit()
    {
        //�ִϸ��̼� �Ķ���� Off
        if (SetAnimationBoolName() != null && m_PlayerCore != null) 
            m_PlayerCore.m_AnimationManager.SetBool(SetAnimationBoolName(), false);
    }

    protected void Movement()
    {
        m_PlayerCore.m_Locomotion.HandleMove();
        m_PlayerCore.m_Locomotion.HandleRotation();
    }


    public virtual LocomotionBaseState CheckTransition() => null;

   // protected abstract LocomotionMainState GetMainState();
    protected abstract string SetAnimationBoolName();
}