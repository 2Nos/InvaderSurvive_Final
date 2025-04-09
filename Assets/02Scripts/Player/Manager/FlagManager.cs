using UnityEngine;
using System;

/// <summary>
/// 메인 이동 상태
/// 항상 단일(MainState들중 하나의, Flags 적용x) 상태로 유지 + Flags 복수개 선택
/// Moving + LocomotionFlags(Sprinting + Aming)
/// </summary>
public enum LocomotionMainState
{
    Idle = 0,           // Idle
    Moving = 1,         // 기본 이동
    InAir = 2,          // 공중 (점프/낙하)
    Sliding = 3,        // 슬라이딩
    Climbing = 4,       // 등반
    WallRunning = 5     // 벽 달리기
}

/// <summary>
/// Flags는 복수의 열거형 선택이 가능한 기능. 즉, 복수 상태가 가능
/// Flags는 2의 제곱수나 2의 제곱수 조합을 사용하여 선언해야함
/// None =0, Sprinting = 1, Croucning = 2, 4,8 이런식 보다는 쉬프트연)
/// </summary>
[Flags]
public enum LocomotionSubFlags
{
    None = 0,
    Sprinting = 1 << 1,         // 달리기
    Crouching = 1 << 2,         // 앉기
}

/// <summary>
/// 전투상태일 때의 메인 상태
/// </summary>
public enum CombatMainState
{
    None = 0,
    Shooting = 1,         //원거리 공격
    MeleeAttacking = 2,   //근접 공격
    UsingAbility = 3,     //스킬(능력) 사용
    UsingUltimate = 4     //궁극기 사용
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

/// <summary>
/// 순간적인 모든 상태에 영향이 가는 상태는 이렇게 따로 관리
/// </summary>
[Flags]
public enum ActionStateFlags
{
    None = 0,
    Dodging = 1 << 0,           // 구르기(회피기)
    Staggered = 1 << 1,         // 피격 경직 같은 상태
    Knockback = 1 << 2          // 넉백
}

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


public class FlagManager : MonoBehaviour
{
    #region ----------------------------- Locomotion
    //// Flags만 Setter Methods
    public LocomotionMainState LocomotionMain { get; set; } = LocomotionMainState.Idle;
    public LocomotionSubFlags LocomotionFlags { get; private set; } = LocomotionSubFlags.None;
    public void SetLocomotionFlag(LocomotionSubFlags flag) => LocomotionFlags |= flag;      // 해당 상태로 설정
    public bool HasLocomotionFlag(LocomotionSubFlags flag) => (LocomotionFlags & flag) != 0;
    public void ClearLocomotionFlag(LocomotionSubFlags flag) => LocomotionFlags &= ~flag;   // 상태 설정 제거
    #endregion --------------------------

    #region -------------------------- Combat
    public CombatMainState CombatMain { get; set; } = CombatMainState.None;
    public CombatSubFlags CombatFlags { get; private set; } = CombatSubFlags.None;
    public void SetCombatFlag(CombatSubFlags flag) => CombatFlags |= flag;
    public bool HasCombatFlag(CombatSubFlags flag) => (CombatFlags & flag) != 0;
    public void ClearCombatFlag(CombatSubFlags flag) => CombatFlags &= ~flag;
    #endregion --------------------------

    #region -------------------------- Action
    public ActionStateFlags ActionFlags { get; private set; } = ActionStateFlags.None;
    public void SetActionStateFlags(ActionStateFlags flag) => ActionFlags |= flag;
    public bool HasActionStateFlags(ActionStateFlags flag) => (ActionFlags & flag) != 0;
    public void ClearActionStateFlags(ActionStateFlags flag) => ActionFlags &= ~flag;
    #endregion --------------------------

    #region -------------------------- Interaction
    public InteractionFlags InteractionFlags { get; private set; } = InteractionFlags.None;
    public void SetInteractionFlag(InteractionFlags flag) => InteractionFlags |= flag;
    public bool HasInteractionFlag(InteractionFlags flag) => (InteractionFlags & flag) != 0;
    public void ClearInteractionFlag(InteractionFlags flag) => InteractionFlags &= ~flag;
    #endregion --------------------------
    
    // 전체 상태 출력
    public string DumpAllStates()
    {
        return $"Locomotion Main: {LocomotionMain}, Flags: {LocomotionFlags}\n" +
               $"Combat Main: {CombatMain}, Flags: {CombatFlags}\n" +
               $"ActionState Flags: {ActionFlags}\n" +
               $"Interaction Flags: {InteractionFlags}";
    }

    // 필요 시 개별 로그 메서드도 제공
    public void DebugPrintFlags()
    {
        Debug.Log(DumpAllStates());
    }
}