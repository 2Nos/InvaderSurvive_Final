using UnityEngine;

//Idle, Moving, InAir, Sliding, Climbing 같은 상태는 메인으로
public abstract class PlayerMainState
{
    protected PlayerLocomotion m_locomotion;
    protected PlayerAnimationManager m_animationManager;
    protected PlayerInputManager m_inputManager;
    protected FlagManager m_flagManager;
    protected PlayerInventoryManager m_inventoryManager;
    protected PlayerAbilityManager m_abilityManager;
    protected WeaponManager m_weaponManager;

    // 추가: 기본은 None 반환
    protected virtual LocomotionSubFlags LocomotionFlag => LocomotionSubFlags.None;
    protected virtual CombatSubFlags CombatFlag => CombatSubFlags.None;
    protected virtual InteractionFlags InteractionFlag => InteractionFlags.None;

    //생성자(Constructor)와 상속 관계 PlayerMainState 상속시 각 상태들 생성자
    public PlayerMainState(PlayerLocomotion locomotion) 
    {
        m_locomotion = locomotion;
        m_animationManager = locomotion.GetAnimationManager();
        m_inputManager = locomotion.GetInputManager();
        m_flagManager = locomotion.GetFlagManager();
        m_inventoryManager = locomotion.GetInventoryManager();
        m_abilityManager = locomotion.GetAbilityManager();
        m_weaponManager = locomotion.GetWeaponManager();
    }
    public virtual void Enter(){}
    public virtual void Update()
    {
        HandleFlagStateChanges(); // 공통 플래그 체크 처리
    }
    public virtual void Exit() { }

    /// <summary>
    /// 반복되는 상태 조건들을 공통 처리
    /// 각 State의 Update에서 base.Update() 호출만 해도 됨
    /// </summary>
    protected virtual void HandleFlagStateChanges()
    {
        /*// 이동 중이 아닌 경우 이동 플래그 제거
        if (!m_inputManager.IsMoving)
            m_flagManager.UnsetLocomotionFlag(LocomotionSubFlags.Moving);
        else
            m_flagManager.SetLocomotionFlag(LocomotionSubFlags.Moving);

        // 스프린트 처리
        if (m_inputManager.IsSprinting)
            m_flagManager.SetLocomotionFlag(LocomotionSubFlags.Sprinting);
        else
            m_flagManager.UnsetLocomotionFlag(LocomotionSubFlags.Sprinting);

        // 앉기 처리
        if (m_inputManager.IsCrouching)
            m_flagManager.SetLocomotionFlag(LocomotionSubFlags.Crouching);
        else
            m_flagManager.UnsetLocomotionFlag(LocomotionSubFlags.Crouching);

        // 조준
        if (m_inputManager.IsAiming)
            m_flagManager.SetCombatFlag(CombatStateFlags.Aiming);
        else
            m_flagManager.UnsetCombatFlag(CombatStateFlags.Aiming);

        // 근접 공격
        *//*if (m_inputManager.IsMeleePressed || )
            m_flagManager.SetCombatFlag(CombatStateFlags.MeleeAttacking);
        else
            m_flagManager.UnsetCombatFlag(CombatStateFlags.MeleeAttacking);

        // 낙하/점프 감지 (예시, 실제 점프 처리 로직과 분리 필요)
        if (!m_locomotion.IsGround())
            m_flagManager.SetLocomotionFlag(LocomotionSubFlags.InAir);
        else
            m_flagManager.UnsetLocomotionFlag(LocomotionSubFlags.InAir);*/
    }

    /// <summary>
    /// 특정 Locomotion 상태가 켜져있는지 여부
    /// </summary>
    //protected bool IsLocomotion(LocomotionSubFlags state) => m_flagManager.LocomotionState.HasFlag(state);
   // protected bool IsCombat(CombatStateFlags state) => m_flagManager.CombatState.HasFlag(state);
   // protected bool IsInteraction(InteractionFlags state) => m_flagManager.InteractionState.HasFlag(state);
}
