using DUS.Player.Weapon;
using UnityEngine;

public enum WeaponType
{
    None,       
    Melee,      // ����
    Range,      // ���Ÿ�
    Magic,      // �׳� ����Ʈ�� ���� ����� ������ ����
    Heavy       // ��ȭ��
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
