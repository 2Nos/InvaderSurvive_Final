using UnityEngine;

public class PlayerSubState : MonoBehaviour
{
    protected PlayerLocomotion m_locomotion;
    protected PlayerAnimationManager m_animationManager;
    protected PlayerInputManager m_inputManager;
    protected FlagManager m_flagManager;
    protected PlayerInventoryManager m_inventoryManager;
    protected PlayerAbilityManager m_abilityManager;
    protected WeaponManager m_weaponManager;

    public PlayerSubState(PlayerLocomotion locomotion)
    {
        m_locomotion = locomotion;
        m_animationManager = locomotion.GetAnimationManager();
        m_inputManager = locomotion.GetInputManager();
        m_flagManager = locomotion.GetFlagManager();
        m_inventoryManager = locomotion.GetInventoryManager();
        m_abilityManager = locomotion.GetAbilityManager();
        m_weaponManager = locomotion.GetWeaponManager();
    }
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }

    protected virtual void HandleFlagStateChanges()
    {
        // 이동 중이 아닌 경우 이동 플래그 제거
        if (!m_inputManager.IsAiming)
            m_flagManager.UnsetLocomotionFlag(LocomotionStateFlags.Moving);
        else
            m_flagManager.SetLocomotionFlag(LocomotionStateFlags.Moving);

        // 스프린트 처리
        if (m_inputManager.IsSprinting)
            m_flagManager.SetLocomotionFlag(LocomotionStateFlags.Sprinting);
        else
            m_flagManager.UnsetLocomotionFlag(LocomotionStateFlags.Sprinting);

        // 앉기 처리
        if (m_inputManager.IsCrouching)
            m_flagManager.SetLocomotionFlag(LocomotionStateFlags.Crouching);
        else
            m_flagManager.UnsetLocomotionFlag(LocomotionStateFlags.Crouching);

        // 조준
        if (m_inputManager.IsAiming)
            m_flagManager.SetCombatFlag(CombatStateFlags.Aiming);
        else
            m_flagManager.UnsetCombatFlag(CombatStateFlags.Aiming);

        // 근접 공격
        /*if (m_inputManager.IsMeleePressed || )
            m_flagManager.SetCombatFlag(CombatStateFlags.MeleeAttacking);
        else
            m_flagManager.UnsetCombatFlag(CombatStateFlags.MeleeAttacking);*/

        // 낙하/점프 감지 (예시, 실제 점프 처리 로직과 분리 필요)
        if (!m_locomotion.IsGround())
            m_flagManager.SetLocomotionFlag(LocomotionStateFlags.InAir);
        else
            m_flagManager.UnsetLocomotionFlag(LocomotionStateFlags.InAir);
    }
}