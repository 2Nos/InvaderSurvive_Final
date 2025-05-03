using DUS.Player.Weapon;
using UnityEngine;

public class RangedWeaponDataSO : WeaponDataSO
{
    public float m_ReloadTime;      // 장전 시간
    public int m_CurrentAmmo;       // 현재 탄약
    public int m_MaxAmmo;           // 최대 탄약
    public int m_Magazine;          // 찬창
}