using System.Threading.Tasks;
using UnityEngine;
using DUS.Player.Locomotion;

public class SlideState : LocomotionStrategyState
{
    public SlideState(PlayerCore playerCore) : base(playerCore) { }
    protected override LocomotionMainState DetermineStateType() => LocomotionMainState.Slide;
    protected override AniParmType SetAniParmType() => AniParmType.SetBool; //�����̵� ���¿� ������ Trigger�� �ʿ�

    bool ischeck;
    public  override void Enter()
    {
        base.Enter();
        m_PlayerCore.m_InputManager.SetFlagKey(false, false, true); // �����̵� ���¿����� Ű �Է��� ���� �ʵ��� ����
        m_PlayerCore.m_AnimationManager.SetParmTrigger("Slide");
        m_DelayTime = 3f;
    }

    public override void Update()
    {
        base.Update();
        
        CheckTransitionedNextAnimation("SlideStart");

        if (!m_IsNextStateCheck) return;
        if (m_AnimationTime >= 0)
        {
            m_AnimationTime -= Time.fixedDeltaTime;
            UpdateSliding();
        }
        else
        {
            m_Locomotion.SetNextState(LocomotionMainState.Idle);
        }

    }

    public override void Exit()
    {
        base.Exit();
        //m_Locomotion.ClearAllLocomotionFlags();
        m_PlayerCore.m_InputManager.SetFlagKey(false, false,false);
        m_PlayerCore.OnChangeColider(true);
    }

    public override void UpdateMovement()
    {
        m_Locomotion.HandleRotation();

        //m_Locomotion.HandleGravityMovement();
        //m_Locomotion.HandleSlideMovement();
    }

    public void UpdateSliding()
    {
        Vector3 velocity = m_PlayerCore.transform.forward * m_PlayerCore.m_SlideSpeed;
        m_PlayerCore.SetRigidVelocity(velocity);
    }
}
