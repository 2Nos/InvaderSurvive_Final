using UnityEngine;
using DUS.Player.Locomotion;

public abstract class LocomotionStrategyState
{
    protected PlayerCore m_PlayerCore;
    protected PlayerLocomotion m_Locomotion;
    protected Animator m_Aniamtor;
    #region ======================================== 생성자 & Interface & 변수

    public LocomotionStrategyState(PlayerCore playerCore)
    {
        m_PlayerCore = playerCore;
        m_Locomotion = playerCore.m_Locomotion;
        m_Aniamtor = m_PlayerCore.m_AnimationManager.m_Animator;
    }

    // TODO : 추상 함수들 차후 Interface로 분류
    protected abstract LocomotionMainState DetermineStateType();
    protected abstract AniParmType SetAniParmType();

    private LocomotionMainState m_ThisState;
    private AniParmType m_AniParmType;

    protected bool m_isCheckStop = false;

    // =====애니메이션의 길이와 이를 진입 확인 및 해당 애니메이션 구간 나누어 지속할 시간 DelayTime으로 조절
    protected float m_AnimationTime;
    protected bool m_IsNextStateCheck;
    protected float m_DelayTime;

    #endregion ======================================== /생성자 & Interface & 변수

    // Enter, Exit, Update, FixedUpdate는 전략 패턴(Strategy) 방식으로 구현
    #region ======================================== Update, FixedUpdate, Enter, Exit

    public void InitializeIdle()
    {
        //m_Locomotion.m_StateUtility.AllClearFlags();
        //m_Locomotion.InitializeVelocity();
    }

    public virtual void Enter()
    {
        // TODO : 상태 전한 지연 필요 시 사용
        m_ThisState = DetermineStateType();
        m_AniParmType = SetAniParmType();

        m_Locomotion.m_StateUtility.SetMainStateAnimation(m_ThisState, m_Aniamtor, m_AniParmType, true); //애니메이션 파라미터 값 적용 (시작)
    }

    public virtual void FixedUpdate()
    {
        // 1. UpdateMovement
        UpdateMovement();
    }

    public virtual void Update()
    {
        // 2. base.Update로 상태 체크 - 각 상태에서의 변환상태 생성에 의한 보류
        //CheckLocomotion();

        // 3. Flags 애니메이션 처리
        //HandleCheckFlags(isCheckStop);
    }

    public virtual void Exit()
    {
        //애니메이션 파라미터값 적용 (종료)
        m_Locomotion.m_StateUtility.SetMainStateAnimation(m_ThisState, m_Aniamtor, m_AniParmType, false);

        m_AnimationTime = 0;
        m_IsNextStateCheck = false;
    }

    // 상태별 UpdateMovement 재정의 
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

    #region ======================================== 상태 전환 처리 관리
    // 상태 우선순위 처리
    public LocomotionStrategyState? IsAction()
    {
        // 공통 우선순위: 구르기, 피격 상태 등 전역 ActionState 우선

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
    //Enter에서 이전 애니메이션이름으로 들어와져 길이를 못구하는 문제 발생
    //이를 Enter에서 비동기 처리도 해보았지만 알되서 결국 Update에서 하기로
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
    #endregion ======================================== /상태 전환 처리 관리

}
