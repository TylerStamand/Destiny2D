using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;   

public abstract class Weapon : NetworkBehaviour
{
    [field: SerializeField] public float Damage { get; private set; }
    [SerializeField] protected float coolDown = 1;

    public NetworkVariable<float> lastUseTime;
    
    public ulong ParentNetID;

    void Awake() {
        lastUseTime.Value = 0;
    }

    void OnValidate() {

        if (GetComponentInChildren<SpriteRenderer>() == null) {
            Debug.LogWarning("No SpriteRenderer for " + gameObject.name + " found");
        }
    }


    [ServerRpc]
    public virtual void AttackServerRpc(Direction direction) {
        if (lastUseTime.Value + coolDown < Time.time) {
            lastUseTime.Value = Time.time;
            AttackClientRpc(direction);
        }
    }

    [ClientRpc]
    protected virtual void AttackClientRpc(Direction direction) {
        Debug.LogWarning("No AttackClientRpc function defined on " + gameObject.name);
    }
}