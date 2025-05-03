using DUS.Player.Weapon;
using UnityEngine;

public enum WeaponType
{
    None,       
    Melee,      // 근접
    Range,      // 원거리
    Magic,      // 그냥 이펙트나 판정 방식이 마법일 뿐임
    Heavy       // 중화기
}

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] WeaponDataSO m_weaponData;
    public WeaponDataSO m_WeaonData => m_weaponData;

    public virtual void Equip() => gameObject.SetActive(true);
    public virtual void Unequip() => gameObject.SetActive(false);
    public abstract void Attack(float delay);
    public abstract void Decay();
}
