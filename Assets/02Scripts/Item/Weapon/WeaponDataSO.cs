using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DUS.Player.Weapon
{
    //abstract 직접 인스턴스를 만드는 실수 방지
    public abstract class WeaponDataSO : ScriptableObject
    {
        public string weaponName;
        public WeaponType TypeData;
        public Sprite icon;
        public GameObject prefab;

        [Header("Stats")]
        public float damage;
        public float attackRate;
        public float attackDistance;
    }
}