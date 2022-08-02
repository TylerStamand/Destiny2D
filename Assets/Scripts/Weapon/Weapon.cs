using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;   

public abstract class Weapon : NetworkBehaviour
{
    public NetworkVariable<float> Damage { get; private set; } = new NetworkVariable<float>(1);
    protected NetworkVariable<float> coolDown = new NetworkVariable<float>(1);
    protected NetworkVariable<float> lastUseTime = new NetworkVariable<float>(0);
    
    public ulong ParentNetID;

    protected bool playerWeapon;

    bool initialized = false;

    void OnValidate() {

        if (GetComponentInChildren<SpriteRenderer>() == null) {
            Debug.LogWarning("No SpriteRenderer for " + gameObject.name + " found");
        }
    }

    public virtual void InitializeWeaponServer(WeaponStats weaponStats, bool playerWeapon) {
        if(!IsServer) return;
        
        Damage.Value = weaponStats.Damage;
        coolDown.Value = weaponStats.CoolDown;
        initialized = true;
        this.playerWeapon = playerWeapon;
    }


    [ServerRpc]
    public virtual void AttackServerRpc(Direction direction) {
        if(!initialized){
            Debug.LogError("Weapon not initialized, please initialize before using weapon");
            return;
        }

        if (lastUseTime.Value + coolDown.Value < Time.time) {
            lastUseTime.Value = Time.time;
            AttackClientRpc(direction);
        }
    }

    [ClientRpc]
    protected virtual void AttackClientRpc(Direction direction) {
        Debug.LogWarning("No AttackClientRpc function defined on " + gameObject.name);
    }
}