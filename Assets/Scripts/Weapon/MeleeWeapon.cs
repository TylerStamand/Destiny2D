using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening;



public class MeleeWeapon : NetworkBehaviour {
    [field: SerializeField] public float Damage { get; private set; }
    [SerializeField] float coolDown = 5;

    public NetworkVariable<float> lastUseTime;

    public GameObject Parent { get; private set; }

    SpriteRenderer spriteRenderer;
    new Collider2D collider;


    void Awake() {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        spriteRenderer.enabled = false;
        collider.enabled = false;
        lastUseTime.Value = 0;
    }

    public override void OnNetworkDespawn() {
        if(IsClient && transform.parent != null) {
            transform.parent.DOKill();
        }
    }

    [ClientRpc]
    public void SetParentClientRpc(ulong parentNetID) {
        Debug.Log("SetParentClient " + NetworkManager.Singleton.LocalClientId);
        Parent = NetworkManager.SpawnManager.SpawnedObjects[parentNetID].gameObject;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), Parent.GetComponent<Collider2D>());
    }

    [ServerRpc]
    public void AttackServerRpc(Direction direction) {
        if (lastUseTime.Value + coolDown < Time.time) {
            lastUseTime.Value = Time.time;
            AttackClientRpc(direction);
        }
    }

    

    void OnTriggerEnter2D(Collider2D collider) {
        if (IsServer) {

            IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();
            if (damageable != null) {
                damageable.TakeDamageServerRpc(Damage);
            }
        }
    }

    [ClientRpc]
    void AttackClientRpc(Direction direction) {
        spriteRenderer.enabled = true;
        collider.enabled = true;

        if(transform.parent != null) {
            transform.parent.eulerAngles = Utilities.GetAngleFromDirection(direction);
            transform.parent.DORotate(new Vector3(0, 0, transform.parent.eulerAngles.z + 179), .3f).onComplete +=
                () => {
                    transform.parent.eulerAngles = Utilities.GetAngleFromDirection(direction);
                    spriteRenderer.enabled = false;
                    collider.enabled = false;
                };
        }
        else {
            Debug.LogWarning("Weapon parent is not assigned, will not animate");
        }

    }




}
