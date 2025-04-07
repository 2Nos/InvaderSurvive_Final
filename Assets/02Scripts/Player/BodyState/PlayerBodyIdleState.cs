using UnityEngine;

public class PlayerBodyIdleState : PlayerBodyState
{
   public PlayerBodyIdleState(PlayerLocomotion locomotion) : base(locomotion) { }

    public override void Enter()
    {
        m_animationManager.SetAiming(false);
        m_animationManager.SetSprinting(false);
        m_animationManager.SetCrouching(false);
        m_animationManager.UpdateMovementAnimation(Vector3.zero, false);
    }

    public override void Update()
    {
        var inputManager = m_locomotion.GetInputManager();
        
        if (inputManager.IsMoving)
        {
            m_locomotion.ChangeState(new PlayerBodyMoveState(m_locomotion));
        }
        else if (inputManager.IsAiming)
        {
            m_locomotion.ChangeState(new PlayerBodyAimState(m_locomotion));
        }

        m_locomotion.HandleRotation();
    }
}
