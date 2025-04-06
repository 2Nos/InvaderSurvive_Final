using UnityEngine;

public class PlayerBodySprintState : PlayerBodyState
{
    public PlayerBodySprintState(PlayerLocomotion locomotion) : base(locomotion) { }

    public override void Enter()
    {
        m_animationManager.SetAiming(false);
        m_animationManager.SetSprinting(true);
        m_animationManager.SetCrouching(false);
    }

    public override void Exit()
    {
        m_animationManager.SetSprinting(false);
    }

    public override void Update()
    {
        var inputManager = m_locomotion.GetInputManager();
        
        if (!inputManager.IsSprinting || !inputManager.IsMoving)
        {
            m_locomotion.ChangeState(new PlayerBodyMoveState(m_locomotion));
            return;
        }

        m_locomotion.HandleMovement();
        m_locomotion.HandleRotation();
        m_animationManager.UpdateMovementAnimation(m_locomotion.GetMoveDirection(), true);
    }
}
