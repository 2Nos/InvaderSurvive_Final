using UnityEngine;
public class SlideState : LocomotionBaseState
{
    public SlideState(PlayerCore playerCore) : base(playerCore) { }

    public override LocomotionMainState DetermineStateType() => LocomotionMainState.Slide;

    public override AniParmType SetAniParmType() => AniParmType.SetBool;

    float m_slideTime;
    public override void Enter()
    {
        base.Enter();
        m_PlayerCore.m_AnimationManager.SetParmTrigger("Slide");
        m_PlayerCore.m_InputManager.SetFlagKey(false, false,true); // 슬라이드 상태에서는 키 입력을 받지 않도록 설정
        m_PlayerCore.OnChangeColider(false);
        m_slideTime = 3f;
    }

    public override void Update()
    {
        base.Update();
        if (m_slideTime >= 0)
        {
            m_slideTime -= Time.fixedDeltaTime;
            UpdateSliding();
        }
        else
        {
            m_Locomotion.ChangeMainState(LocomotionMainState.Idle);
        }
    }

    public override void Exit()
    {
        base.Exit();
        m_Locomotion.ClearAllLocomotionFlags();
        m_PlayerCore.m_InputManager.SetFlagKey(false, false,false);
        m_PlayerCore.OnChangeColider(true);
        m_slideTime = 2f;
    }

    public override void Movement()
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
