using UnityEngine;

public class PlayerBodyCrouchState : PlayerBodyState
{
    public PlayerBodyCrouchState(PlayerLocomotion locomotion) : base(locomotion) { }

    public override void Enter()
    {
        m_animationManager.SetAiming(false);
        m_animationManager.SetSprinting(false);
        m_animationManager.SetCrouching(true);
    }

    public override void Exit()
    {
        m_animationManager.SetCrouching(false);
    }

    public override void Update()
    {
        var inputlocomotion = m_locomotion.GetInputManager();
        
        if (!inputlocomotion.IsCrouching)
        {
            m_locomotion.ChangeState(new PlayerBodyMoveState(m_locomotion));
            return;
        }

        m_locomotion.HandleMovement();
        m_locomotion.HandleRotation();
        m_animationManager.UpdateMovementAnimation(m_locomotion.GetMoveDirection(), true);
    }
}
