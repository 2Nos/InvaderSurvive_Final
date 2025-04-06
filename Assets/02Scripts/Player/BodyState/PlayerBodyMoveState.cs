using UnityEngine;

public class PlayerBodyMoveState : PlayerBodyState
{
   public PlayerBodyMoveState(PlayerLocomotion locomotion) : base(locomotion) { }

    public override void Enter()
    {
        m_animationManager.SetAiming(false);
        m_animationManager.SetSprinting(false);
        m_animationManager.SetCrouching(false);
    }

    public override void Update()
    {
        var inputManager = m_locomotion.GetInputManager();
        
        if (!inputManager.IsMoving)
        {
            m_locomotion.ChangeState(new PlayerBodyIdleState(m_locomotion));
            return;
        }

        if (inputManager.IsAiming)
        {
            m_locomotion.ChangeState(new PlayerBodyAimMoveState(m_locomotion));
            return;
        }

        m_locomotion.HandleMovement();
        m_locomotion.HandleRotation();
        m_animationManager.UpdateMovementAnimation(m_locomotion.GetMoveDirection(), true);
    }
}
