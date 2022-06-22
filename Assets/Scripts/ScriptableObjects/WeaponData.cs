using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;



[CreateAssetMenu(fileName ="WeaponData", menuName = "ScriptableObjects/WeaponData" )]
public class WeaponData : ItemData
{
    public Weapon WeaponPrefab;
    
    
    public MinMaxFloat Damage;
    public MinMaxFloat CoolDown;
    public MinMaxFloat ProjectileSpeed;

    public override Item CreateItem() {
        return new WeaponItem(Name, Damage.GetRandomValue(), CoolDown.GetRandomValue(), ProjectileSpeed.GetRandomValue());
    }
}
