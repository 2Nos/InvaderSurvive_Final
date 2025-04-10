using System.Collections.Generic;
using System.Linq;

public abstract class LocomotionStateBase
{
    protected PlayerCore m_Core;

    public virtual void Enter(PlayerCore core)
    {
        m_Core = core;
        m_Core.GetFlagManager().m_LocomotionMain = GetMainState();

        string animBool = GetEnterAnimationBoolName();
        if (!string.IsNullOrEmpty(animBool))
            m_Core.GetAnimationManager().SetBool(animBool, true);
    }

    public virtual void Exit()
    {
        string animBool = GetEnterAnimationBoolName();
        if (!string.IsNullOrEmpty(animBool))
            m_Core.GetAnimationManager().SetBool(animBool, false);
    }

    public virtual void Update() { }
    public virtual LocomotionStateBase CheckTransition() => null;

    protected abstract LocomotionMainState GetMainState();
    protected virtual string GetEnterAnimationBoolName() => null;
}