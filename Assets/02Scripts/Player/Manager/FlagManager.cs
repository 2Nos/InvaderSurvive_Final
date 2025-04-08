using UnityEngine;
using System;


//상태 플래그를 비트 연산으로 처리하며 Flags는 다수의 열거형 선택이 가능
/// <summary>
/// 메인 상태이며 해당 동작중에도 다른 동작이 가능
/// </summary>
[Flags]
public enum LocomotionStateFlags
{
    None = 0,
    Moving = 1 << 0,          // 기본 이동
    Sprinting = 1 << 1,       // 달리기
    Crouching = 1 << 2,       // 앉기
    InAir = 1 << 4,          // 공중 (점프/낙하)
    Sliding = 1 << 5,        // 슬라이딩
    WallRunning = 1 << 6,    // 벽 달리기
    Climbing = 1 << 7        // 등반
}

/// <summary>
/// 특수 이동, 해당 동작 중 다른 동작 X
/// </summary>
[Flags]
public enum SpecialLocomotionState
{
    None = 0,
    Dodging = 1 << 1,        // 구르기
    Vaulting = 1 << 2,
    Grappling = 1 << 3
}

// 전투 상태
[Flags]
public enum CombatStateFlags
{
    None = 0,
    Aiming = 1 << 0,           // 조준
    Shooting = 1 << 1,         // 발사 (원거리)
    Reloading = 1 << 2,        // 재장전
    WeaponSwapping = 1 << 3,   // 무기 교체
    UsingAbility = 1 << 4,     // 특수 능력 사용
    UsingUltimate = 1 << 5,    // 궁극기 사용

    MeleeAttacking = 1 << 6,   // 근접 공격 (일반)
    ChargingMelee = 1 << 7,    // 근접 차지 공격 준비
    ExecutingMelee = 1 << 8    // 근접 처형 (피니시)
}

// 상호작용 상태
[Flags]
public enum InteractionStateFlags
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
    public LocomotionStateFlags LocomotionState { get; private set; }
    public CombatStateFlags CombatState { get; private set; }
    public InteractionStateFlags InteractionState { get; private set; }

    // ---------------- Locomotion ----------------
    //이런 식이면 if (m_flagManager.HasLocomotionFlag(LocomotionStateFlags.Crouching)) 등으로 깔끔하게 확인 가능
    public void SetLocomotionFlag(LocomotionStateFlags flag) => LocomotionState |= flag;   //변경
    public void UnsetLocomotionFlag(LocomotionStateFlags flag) => LocomotionState &= ~flag;  //제거
    public bool HasLocomotionFlag(LocomotionStateFlags flag) => (LocomotionState & flag) != 0;  //확인

    // ---------------- Combat ----------------
    public void SetCombatFlag(CombatStateFlags flag) => CombatState |= flag;
    public void UnsetCombatFlag(CombatStateFlags flag) => CombatState &= ~flag;
    public bool HasCombatFlag(CombatStateFlags flag) => (CombatState & flag) != 0;


    // ---------------- Interaction ----------------
    public void SetInteractionFlag(InteractionStateFlags flag) => InteractionState |= flag;
    public void UnsetInteractionFlag(InteractionStateFlags flag) => InteractionState &= ~flag;
    public bool HasInteractionFlag(InteractionStateFlags flag) => (InteractionState & flag) != 0;


    // 전체 상태 출력
    public string DumpAllStates()
    {
        return $"[Locomotion: {LocomotionState}]\n[Combat: {CombatState}]\n[Interaction: {InteractionState}]";
    }

    // 필요 시 개별 로그 메서드도 제공
    public void DebugPrintFlags()
    {
        Debug.Log(DumpAllStates());
    }
}