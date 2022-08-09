using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening;
using Destiny2D;

[RequireComponent(typeof(Collider2D))]
public class MeleeWeapon : Weapon {
    

    SpriteRenderer spriteRenderer;
    new Collider2D collider;


    void Awake() {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        spriteRenderer.enabled = false;
        collider.enabled = false;
    }

  

    public override void OnNetworkDespawn() {
        if(IsClient && transform.parent != null) {
            transform.parent.DOKill();
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (IsServer) {

            IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();
            if (damageable != null) {

                //Keeps Enemies from hitting each other
                if(!playerWeapon && collider.GetComponent<Enemy>() != null)
                    return;

                //Keeps Players from hitting each other
                if(playerWeapon && collider.GetComponent<PlayerControllerServer>() != null)
                    return;
                
                damageable.TakeDamageServerRpc(Damage.Value);
            }
        }
    }

    [ClientRpc]
    protected override void AttackClientRpc(Direction direction) {
        spriteRenderer.enabled = true;
        collider.enabled = true;

        if(transform.parent != null) {

            //Gets Angle from direction, then subtracts 90 degrees to make it a wider rotation
            transform.parent.eulerAngles = Utilities.GetAngleFromDirection(direction) - new Vector3(0, 0, 90);
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
