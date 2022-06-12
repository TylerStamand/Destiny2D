using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;



[CreateAssetMenu(fileName ="WeaponData", menuName = "ScriptableObjects/WeaponData" )]
public class WeaponData : ItemData
{
    public Weapon WeaponPrefab;
    public float Damage = 0;
    public float CoolDown = 0;
    public float ProjectileSpeed = 0;

    public override Item CreateItem() {
        return new WeaponItem(Name, Damage, CoolDown);
    }
}
