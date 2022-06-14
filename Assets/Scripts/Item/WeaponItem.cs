using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponItem : Item, INetworkSerializable {

    private float damage;
    private float coolDown;

    public float Damage {get => damage; private set => damage = value;}
    public float CoolDown {get => coolDown; private set => coolDown = value; }

    public WeaponItem(string name, float damage, float coolDown) : base(name) {
        Damage = damage;
        CoolDown = coolDown;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        base.NetworkSerialize(serializer);
        serializer.SerializeValue(ref damage);
        serializer.SerializeValue(ref coolDown);
    }
}
