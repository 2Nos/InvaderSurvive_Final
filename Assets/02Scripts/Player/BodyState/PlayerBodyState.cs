using UnityEngine;

public abstract class PlayerBodyState
{
    protected PlayerAnimationManager m_animationManager;
    protected PlayerLocomotion m_locomotion;
    // Start is called once before the first execution of Update after the MonoBehaviour is create

    public PlayerBodyState(PlayerLocomotion locomotion)
    {
        m_locomotion = locomotion;
        m_animationManager = locomotion.GetAnimationManager();
    }

    public virtual void Enter() { }
    public virtual void FixedUpdate() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
