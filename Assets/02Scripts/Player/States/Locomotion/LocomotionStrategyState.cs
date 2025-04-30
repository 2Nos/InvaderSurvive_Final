using UnityEngine;
using DUS.Player.Locomotion;

public abstract class LocomotionStrategyState
{
    protected PlayerCore m_PlayerCore;
    protected PlayerLocomotion m_Locomotion;
    protected Animator m_Aniamtor;
    #region ======================================== ������ & Interface & ����

    public LocomotionStrategyState(PlayerCore playerCore)
    {
        m_PlayerCore = playerCore;
        m_Locomotion = playerCore.m_Locomotion;
        m_Aniamtor = m_PlayerCore.m_AnimationManager.m_Animator;
    }

    // TODO : �߻� �Լ��� ���� Interface�� �з�
    protected abstract LocomotionMainState DetermineStateType();
    protected abstract AniParmType SetAniParmType();

    private LocomotionMainState m_ThisState;
    private AniParmType m_AniParmType;

    protected bool m_isCheckStop = false;

    // =====�ִϸ��̼��� ���̿� �̸� ���� Ȯ�� �� �ش� �ִϸ��̼� ���� ������ ������ �ð� DelayTime���� ����
    protected float m_AnimationTime;
    protected bool m_IsNextStateCheck;
    protected float m_DelayTime;

    #endregion ======================================== /������ & Interface & ����

    // Enter, Exit, Update, FixedUpdate�� ���� ����(Strategy) ������� ����
    #region ======================================== Update, FixedUpdate, Enter, Exit

    public void InitializeIdle()
    {
        //m_Locomotion.m_StateUtility.AllClearFlags();
        //m_Locomotion.InitializeVelocity();
    }

    public virtual void Enter()
    {
        // TODO : ���� ���� ���� �ʿ� �� ���
        m_ThisState = DetermineStateType();
        m_AniParmType = SetAniParmType();

        m_Locomotion.m_StateUtility.SetMainStateAnimation(m_ThisState, m_Aniamtor, m_AniParmType, true); //�ִϸ��̼� �Ķ���� �� ���� (����)
    }

    public virtual void FixedUpdate()
    {
        // 1. UpdateMovement
        UpdateMovement();
    }

    public virtual void Update()
    {
        // 2. base.Update�� ���� üũ - �� ���¿����� ��ȯ���� ������ ���� ����
        //CheckLocomotion();

        // 3. Flags �ִϸ��̼� ó��
        //HandleCheckFlags(isCheckStop);
    }

    public virtual void Exit()
    {
        //�ִϸ��̼� �Ķ���Ͱ� ���� (����)
        m_Locomotion.m_StateUtility.SetMainStateAnimation(m_ThisState, m_Aniamtor, m_AniParmType, false);

        m_AnimationTime = 0;
        m_IsNextStateCheck = false;
    }

    // ���º� UpdateMovement ������ 
    public virtual void UpdateMovement()
    {
        m_Locomotion.HandleMove();
        m_Locomotion.HandleRotation();
        // m_Locomotion.HandleGravityMovement();
    }

    #endregion ======================================== /Update, FixedUpdate, Enter, Exit

    protected void HandleCheckFlags(LocomotionSubFlags checkFlags, bool isCheck, bool isAllNoCheck = false)
    {
        if (isAllNoCheck) return;

        if(isCheck)
            m_Locomotion.m_StateUtility.SetLocomotionFlag(checkFlags, m_Aniamtor);
        else
            m_Locomotion.m_StateUtility.RemoveLocomotionFlag(checkFlags, m_Aniamtor);
    }

    #region ======================================== ���� ��ȯ ó�� ����
    // ���� �켱���� ó��
    public LocomotionStrategyState? IsAction()
    {
        // ���� �켱����: ������, �ǰ� ���� �� ���� ActionState �켱

        /*if (m_PlayerLocomotion.m_StateFlagManager.HasActionStateFlags(ActionStateFlags.Dodge))
        {
            return new DodgeState(m_PlayerLocomotion);
        }
        if (m_PlayerLocomotion.m_StateFlagManager.HasActionStateFlags(ActionStateFlags.Staggered))
        {
            return new StaggeredState(m_PlayerLocomotion);
        }*/
        return null;
    }
    AnimatorClipInfo[] clips;
    //Enter���� ���� �ִϸ��̼��̸����� ������ ���̸� �����ϴ� ���� �߻�
    //�̸� Enter���� �񵿱� ó���� �غ������� �˵Ǽ� �ᱹ Update���� �ϱ��
    public void CheckTransitionedNextAnimation(string currentAniName)
    {
        if (m_IsNextStateCheck) return;

        if (!m_IsNextStateCheck)
        {
            clips = m_PlayerCore.m_AnimationManager.m_Animator.GetCurrentAnimatorClipInfo(0);
            if (clips == null || clips[0].clip.name != currentAniName) m_IsNextStateCheck = false;
            else m_IsNextStateCheck = true;

            if (m_AnimationTime <= 0 && m_IsNextStateCheck)
            {
                m_AnimationTime = clips[0].clip.length + m_DelayTime;
            }
        }
    }
    #endregion ======================================== /���� ��ȯ ó�� ����

}
