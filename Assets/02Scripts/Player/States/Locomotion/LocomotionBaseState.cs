using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class LocomotionBaseState
{
    protected PlayerCore m_PlayerCore;

    //playerCore 넘기기 위한 생성자, 상속 시 매개변수로 인해 생성자가 자동 생성이 되지 않기에 받은 곳에서 작성해줘야함.
    protected LocomotionBaseState(PlayerCore playerCore)
    {
        m_PlayerCore = playerCore;
    }

    protected abstract LocomotionMainState GetMainState();

    public virtual void Enter()
    {
        m_PlayerCore.m_StateFlagManager.m_LocomotionMain = GetMainState();
        //애니메이션 파라미터 On
        if (SetAnimationBoolName() != null)
            m_PlayerCore.m_AnimationManager.SetBool(SetAnimationBoolName(), true);

    }

    public virtual void Update()
    {
        Movement();
    }

    public virtual void Exit()
    {
        //애니메이션 파라미터 Off
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