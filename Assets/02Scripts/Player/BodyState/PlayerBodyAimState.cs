using UnityEngine;

public class PlayerBodyAimState : PlayerBodyState
{
   public PlayerBodyAimState(PlayerLocomotion locomotion) : base(locomotion) { }

    public override void Enter()
    {
        m_animationManager.SetAiming(true);
        m_animationManager.SetSprinting(false);
        m_animationManager.SetCrouching(false);
        m_locomotion.GetBodyManager().SetAiming(true);
    }

    public override void Exit()
    {
        m_animationManager.SetAiming(false);
        m_locomotion.GetBodyManager().SetAiming(false);
    }

    public override void Update()
    {
        var inputManager = m_locomotion.GetInputManager();
        
        if (!inputManager.IsAiming)
        {
            m_locomotion.ChangeState(new PlayerBodyIdleState(m_locomotion));
            return;
        }

        if (inputManager.IsMoving)
        {
            m_locomotion.ChangeState(new PlayerBodyAimMoveState(m_locomotion));
            return;
        }

        m_locomotion.HandleRotation();
        m_animationManager.UpdateMovementAnimation(Vector3.zero, false);
    }
}
