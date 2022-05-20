using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening;

public class MeleeWeapon : NetworkBehaviour
{
    [field: SerializeField] public float Damage {get; private set;}
    [SerializeField] float coolDown = 5;
    
    public NetworkVariable<float> lastUseTime;

    public GameObject Parent {get; private set;}

    SpriteRenderer spriteRenderer;
    new Collider2D collider;


    void Awake() {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        spriteRenderer.enabled = false;
        collider.enabled = false;
        lastUseTime.Value = 0;
    }

    [ClientRpc]
    public void SetParentClientRpc(ulong parentNetID) {
        Parent = NetworkManager.SpawnManager.SpawnedObjects[parentNetID].gameObject;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), Parent.GetComponent<Collider2D>());
        Debug.Log(Parent.name + parentNetID);
    }

    [ServerRpc]
    public void AttackServerRpc(Vector2 Direction) {
        if(lastUseTime.Value + coolDown < Time.time) {
            lastUseTime.Value = Time.time;
            AttackClientRpc(Direction);
        }
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

    void OnTriggerEnter2D(Collider2D collider) {
        if(IsClient) {

            IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();
            if(damageable != null) {
                damageable.TakeDamageServerRpc(Damage);
            }
        }
    }

    [ClientRpc]
    public void AttackClientRpc(Vector2 Direction) {
        Attack(Direction);
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
