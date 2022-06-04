using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;


// Use a resource system like in slashndash to reference the scriptable object with enums to send over the server


[CreateAssetMenu(fileName ="WeaponData", menuName = "ScriptableObjects/Weapon" )]
public class WeaponData : ScriptableObject
{
    public static readonly string DefaultName = "Weapon Name";


    public string Name = DefaultName;
    public Sprite Sprite;
    public Weapon WeaponPrefab;
    public float Damage = 0;
    public float CoolDown = 0;
    public float ProjectileSpeed = 0;


}