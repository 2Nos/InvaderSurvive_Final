using Unity.Hierarchy;
using UnityEngine;
using System.Linq;

public class JumpState : LocomotionBaseState
{
    public JumpState(PlayerCore playerCore) : base(playerCore) { }
    public override LocomotionMainState DetermineStateType() => LocomotionMainState.Jump;
    public override AniParmType SetAniParmType() => AniParmType.SetTrigger;

    float m_delayCheckTime = 0.5f;

    public override void Enter()
    {
        base.Enter();
        ExecuteJump();
    }

    public override void Update()
    {
        base.Update();

        if(m_delayCheckTime >= 0)
        {
            m_delayCheckTime -= Time.deltaTime;
        }
        else
        {
            if (!m_Locomotion.m_IsGrounded)
            {
                m_Locomotion.ChangeMainState(LocomotionMainState.Idle);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        m_delayCheckTime = 0.5f;
    }

    public override void Movement()
    {
        //m_Locomotion.HandleGravityMovement();
    }

    public void ExecuteJump()
    {
        var key = m_Locomotion.m_MainStateMap.FirstOrDefault(x => x.Value == m_Locomotion.m_prevState).Key;
        switch (key)
        {
            case LocomotionMainState.Idle:
                m_PlayerCore.m_AnimationManager.SetParmTrigger("IdleJump");
                break;
            case LocomotionMainState.Move:
                m_PlayerCore.m_AnimationManager.SetParmTrigger("MoveJump");
                break;
        }
        m_PlayerCore.m_Rigidbody.AddForce(m_PlayerCore.transform.up * m_PlayerCore.m_jumpUpForce, ForceMode.Impulse); //점프 힘을 주기 위해 Rigidbody에 힘을 줌
        //m_NotGravity = true; //중력 무시 상태
    }
}
