using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening;
public class MeleeWeapon : NetworkBehaviour
{
    public GameObject Parent {get; private set;}
    SpriteRenderer spriteRenderer;
    new Collider2D collider;
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        spriteRenderer.enabled = false;
        collider.enabled = false;
    }

    [ClientRpc]
    public void SetParentClientRpc(ulong parentNetID) {
        Parent = NetworkManager.SpawnManager.SpawnedObjects[parentNetID].gameObject;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), Parent.GetComponent<Collider2D>());
        Debug.Log(Parent.name + parentNetID);
    }

    public void Attack(Vector2 Direction) {
        spriteRenderer.enabled = true;
        collider.enabled = true;
        transform.parent.eulerAngles = GetAngleFromDirection(Direction);
        transform.parent.DORotate(new Vector3(0, 0, transform.parent.eulerAngles.z + 179), .3f).onComplete += 
            () => {
                transform.parent.eulerAngles = GetAngleFromDirection(Direction);
                spriteRenderer.enabled = false;
                collider.enabled = false;
            };
    
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(IsServer) {

            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if(player != null) {
                player.TakeDamageServerRpc(player.NetworkObjectId, 1);
            }
        }
    }

    [ClientRpc]
    public void AttackClientRpc(Vector2 Direction) {
        Attack(Direction);
    }

    public override void OnNetworkSpawn()
    {
      
        
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
