using UnityEngine;
using DUS.Player.Locomotion;

//ÂøÁö »óÅÂ
public class LandState : LocomotionStrategyState
{
    public LandState(PlayerCore playerCore) : base(playerCore){}
    protected override LocomotionMainState DetermineStateType() => LocomotionMainState.Land;
    protected override AniParmType SetAniParmType() => AniParmType.SetBool;

    public override void Enter()
    {
        base.Enter();
        m_DelayTime = 0;
    }

    public override void Update()
    {
        base.Update();
        CheckTransitionedNextAnimation("Land");
        if (!m_IsNextStateCheck) return;

        if (m_AnimationTime >= 0)
        {
            
            m_AnimationTime -= Time.deltaTime;
        }
        else
        {
            m_Locomotion.SetNextState(LocomotionMainState.Idle);
        }
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void UpdateMovement()
    {
       m_Locomotion.HandleRotation();

    }
}
