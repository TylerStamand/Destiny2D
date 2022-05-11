using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening;
public class MeleeWeapon : NetworkBehaviour
{
    SpriteRenderer spriteRenderer;

    void Awake() {
        Debug.Log("Awake");
        
    }

    [ClientRpc]
    public void AttackClientRpc(Vector2 Direction) {
            spriteRenderer.enabled = true;
            transform.parent.eulerAngles = GetAngleFromDirection(Direction);
            transform.parent.DORotate(new Vector3(0, 0, transform.parent.eulerAngles.z + 179), .3f).onComplete += 
                () => {
                    transform.parent.eulerAngles = GetAngleFromDirection(Direction);
                    spriteRenderer.enabled = false;
                };
    
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("Network Spawn");
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    Vector3 GetAngleFromDirection(Vector3 Direction) {
        Vector3 eulerAngles = new Vector3();
        if(Direction.x != 0) {
            if(Direction.x > 0) {
                eulerAngles.z = 180;
            } else {
                eulerAngles.z = 0;
            }
            // returns early for defaulting to x axis when both are non 0
            return eulerAngles;
        }
        if(Direction.y != 0) {
            if(Direction.y > 0) {
                eulerAngles.z = -90;
            } else {
                eulerAngles.z = 90;
            }
        }
        return eulerAngles;
    }


}
