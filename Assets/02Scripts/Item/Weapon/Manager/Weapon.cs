using UnityEngine;

public enum WeaponType
{
    None,
    Ranged,
    Melee,
    Magic,    // 그냥 이펙트나 판정 방식이 마법일 뿐임
    Heavy
}

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponType type;

    public WeaponType Type => type;

    public CombatMainState GetCombatStateOnUse()
    {
        //C# 8.0 이상부터 사용할 수 있는 간결한 switch 표현식
        //return value switch
        //Case1 => Result1,
        //Case2 => Result2,
        //_ => DefaultResult // 모든 케이스에 해당하지 않을 때
        return Type switch
        {
            WeaponType.Ranged => CombatMainState.Shooting,
            WeaponType.Melee => CombatMainState.MeleeAttacking,
            WeaponType.Heavy => CombatMainState.Shooting,
            WeaponType.Magic => CombatMainState.Shooting, // 예를 들어 마법탄 쏘는 타입이라면
            _ => CombatMainState.None
        };

        /*return type switch
        {
            WeaponType.Ranged => CombatMainState.Shooting,
            WeaponType.Melee when isCharging => CombatMainState.MeleeAttacking,
            WeaponType.Melee => CombatMainState.None, // 기본 근접 공격은 별도 상태 없음
            WeaponType.Magic when isUltimate => CombatMainState.None, // 따로 처리
            WeaponType.Magic => CombatMainState.UsingAbility,
            _ => CombatMainState.None
        };*/
    }

    public bool CanUseAbility => true; // 모든 무기에서 Ability 발동 가능
    public AbilityMainState GetAbilityStateOnUse()
    {
        return AbilityMainState.UsingAbility;
    }

}