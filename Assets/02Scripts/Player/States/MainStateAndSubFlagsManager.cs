using UnityEngine;
using System;

//[MainState And Flags]
#region Locomotion
/// <summary>
/// 메인 이동 상태
/// 항상 단일(MainState들중 하나의, Flags 적용x) 상태로 유지 + Flags 복수개 선택
/// Move + m_LocomotionFlags(FalgButtonGroupManager + Aming)
/// </summary>
public enum LocomotionMainState
{
    Idle = 0,           // Idle
    Move = 1,           // 기본 이동
    InAir = 2,          // 공중 (점프/낙하)
    Land = 3,           // 착지
    Slide = 4,          // 슬라이딩
    Climb = 5,          // 등반
    WallRun = 6         // 벽 달리기
}

/// <summary>
/// Flags는 복수의 열거형 선택이 가능한 기능. 즉, 복수 상태가 가능
/// Flags는 2의 제곱수나 2의 제곱수 조합을 사용하여 선언해야함
/// None =0, FalgButtonGroupManager = 1, Croucning = 2, 4,8 이런식 보다는 쉬프트연)
/// </summary>
[Flags]
public enum LocomotionSubFlags
{
    None = 0,
    Run = 1 << 1,         // 달리기
    Crouch = 1 << 2,         // 앉기
}

/// <summary>
/// 순간적인 모든 상태에 영향이 가는 상태는 이렇게 따로 관리
/// </summary>
[Flags]
public enum ActionStateFlags
{
    None = 0,
    Dodge = 1 << 0,           // 구르기(회피기)
    Staggered = 1 << 1,         // 피격 경직 같은 상태
    Knockback = 1 << 2          // 넉백
}
#endregion

#region ======================================== Combat
/// <summary>
/// 전투상태일 때의 메인 상태
/// </summary>
public enum CombatMainState
{
    None = 0,
    Shooting = 1,         //원거리 공격
    MeleeAttacking = 2,   //근접 공격
}

/// <summary>
/// 메인 전투상태 + Flags 복수개
/// </summary>
[Flags]
public enum CombatSubFlags
{
    None = 0,
    Aming = 1 << 0,            // 조준
    Reloading = 1 << 1,        // 재장전
    WeaponSwapping = 1 << 2,   // 무기 교체
    ChargingMelee = 1 << 3,    // 근접 차지 공격 준비
    ExecutingMelee = 1 << 4    // 근접 처형 (피니시)
}

public enum AbilityMainState
{
    None = 0,
    UsingAbility = 1,
    UsingUltimate = 2
}
#endregion ======================================== /Combat


#region ======================================== Interaction
/// <summary>
/// 상호작용 상태
/// </summary>
[Flags]
public enum InteractionFlags
{
    None = 0,
    LootingItem = 1 << 0,    // 아이템 획득
    OpeningChest = 1 << 1,   // 상자 열기
    Trading = 1 << 2,        // 거래
    Crafting = 1 << 3,       // 제작
    Upgrading = 1 << 4       // 장비 강화
}
#endregion ======================================== /Interaction

public class MainStateAndSubFlagsManager : MonoBehaviour
{
    public LocomotionMainState m_LocomotionMain
    {
        get => m_locomotionMain;
        set => m_locomotionMain = value;
    }

    public CombatMainState m_CombatMain
    {
        get => m_combatMain;
        set => m_combatMain = value;
    }
    public AbilityMainState m_AbilityMain
    {
        get => m_abilityMain;
        set => m_abilityMain = value;
    }
    public ActionStateFlags m_ActionFlags
    {
        get => m_actionFlags;
        set => m_actionFlags = value;
    }
    public InteractionFlags m_InteractionFlags
    {
        get => m_interactionFlags;
        set => m_interactionFlags = value;
    }

    //[선언]
    #region ======================================== Locomotion
    //// Flags만 Setter Methods
    private LocomotionMainState m_locomotionMain { get; set; } = LocomotionMainState.Idle;
    private LocomotionSubFlags m_LocomotionFlags = LocomotionSubFlags.None;
    public void SetLocomotionFlag(LocomotionSubFlags flag) => m_LocomotionFlags |= flag;      // 해당 상태로 설정
    public bool HasLocomotionFlag(LocomotionSubFlags flag) => (m_LocomotionFlags & flag) != 0;
    public void ClearLocomotionFlag(LocomotionSubFlags flag) => m_LocomotionFlags &= ~flag;   // 상태 설정 제거
    #endregion ======================================== /Locomotion

    #region ======================================== Combat
    private CombatMainState m_combatMain = CombatMainState.None;
    private CombatSubFlags m_CombatFlags = CombatSubFlags.None;
    public void SetCombatFlag(CombatSubFlags flag) => m_CombatFlags |= flag;
    public bool HasCombatFlag(CombatSubFlags flag) => (m_CombatFlags & flag) != 0;
    public void ClearCombatFlag(CombatSubFlags flag) => m_CombatFlags &= ~flag;
    #endregion ======================================== /Combat

    #region ======================================== Ability
    private AbilityMainState m_abilityMain { get; set; } = AbilityMainState.None;
    #endregion ======================================== /Ability

    #region ======================================== Action
    private ActionStateFlags m_actionFlags = ActionStateFlags.None;
    public void SetActionStateFlags(ActionStateFlags flag) => m_actionFlags |= flag;
    public bool HasActionStateFlags(ActionStateFlags flag) => (m_actionFlags & flag) != 0;
    public void ClearActionStateFlags(ActionStateFlags flag) => m_actionFlags &= ~flag;
    #endregion ======================================== /Action

    #region ======================================== Interaction
    private InteractionFlags m_interactionFlags = InteractionFlags.None;
    public void SetInteractionFlag(InteractionFlags flag) => m_interactionFlags |= flag;
    public bool HasInteractionFlag(InteractionFlags flag) => (m_interactionFlags & flag) != 0;
    public void ClearInteractionFlag(InteractionFlags flag) => m_interactionFlags &= ~flag;
    #endregion ======================================== /Interaction

    // 전체 상태 출력
    public string DumpAllStates()
    {
        return $"m_Locomotion Main: {m_LocomotionMain}, Flags: {m_LocomotionFlags}\n" +
               $"m_Combat Main: {m_CombatMain}, Flags: {m_CombatFlags}\n" +
               $"ActionState Flags: {m_ActionFlags}\n" +
               $"Interaction Flags: {m_InteractionFlags}";
    }

    // 필요 시 개별 로그 메서드도 제공
    public void DebugPrintFlags()
    {
        Debug.Log(DumpAllStates());
    }
}