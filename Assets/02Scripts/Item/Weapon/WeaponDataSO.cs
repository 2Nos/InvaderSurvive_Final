using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DUS.Player.Weapon
{
    //abstract ���� �ν��Ͻ��� ����� �Ǽ� ����
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