using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item {
    public float Damage {get; private set;}
    public float CoolDown {get; private set;}

    public WeaponItem(string name, float damage, float coolDown) : base(name) {
        Damage = damage;
        CoolDown = coolDown;
    }

}
