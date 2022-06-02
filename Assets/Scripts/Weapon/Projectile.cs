using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    float speed;
    float damage;
    ulong parentID;
    bool isPlayerClient;

    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable)) {
            
            //Checks if collided with the user of weapon, and returns early if so
            if(collider.gameObject.GetComponent<NetworkObject>().NetworkObjectId == parentID) {
                return;
            }

            if(isPlayerClient) {
                damageable.TakeDamageServerRpc(damage);
            }
        }

        Destroy(gameObject);
    }

    public void Initialize(float speed, float damage, ulong parentNetID, bool isPlayerClient) {
        this.speed = speed;
        this.damage = damage;
        this.parentID = parentNetID;
        this.isPlayerClient = isPlayerClient;

        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
    }
}
