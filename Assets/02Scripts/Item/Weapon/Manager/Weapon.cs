using UnityEngine;

public enum WeaponType
{
    None,
    Ranged,
    Melee,
    Magic,    // �׳� ����Ʈ�� ���� ����� ������ ����
    Heavy
}

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponType type;

    public WeaponType Type => type;

    public CombatMainState GetCombatStateOnUse()
    {
        //C# 8.0 �̻���� ����� �� �ִ� ������ switch ǥ����
        //return value switch
        //Case1 => Result1,
        //Case2 => Result2,
        //_ => DefaultResult // ��� ���̽��� �ش����� ���� ��
        return Type switch
        {
            WeaponType.Ranged => CombatMainState.Shooting,
            WeaponType.Melee => CombatMainState.MeleeAttacking,
            WeaponType.Heavy => CombatMainState.Shooting,
            WeaponType.Magic => CombatMainState.Shooting, // ���� ��� ����ź ��� Ÿ���̶��
            _ => CombatMainState.None
        };

        /*return type switch
        {
            WeaponType.Ranged => CombatMainState.Shooting,
            WeaponType.Melee when isCharging => CombatMainState.MeleeAttacking,
            WeaponType.Melee => CombatMainState.None, // �⺻ ���� ������ ���� ���� ����
            WeaponType.Magic when isUltimate => CombatMainState.None, // ���� ó��
            WeaponType.Magic => CombatMainState.UsingAbility,
            _ => CombatMainState.None
        };*/
    }

    public bool CanUseAbility => true; // ��� ���⿡�� Ability �ߵ� ����
    public AbilityMainState GetAbilityStateOnUse()
    {
        return AbilityMainState.UsingAbility;
    }

}