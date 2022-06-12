using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

[Serializable]
public struct MinMaxInt {
    public int MaxValue;
    public int MinValue;

    public MinMaxInt(int minValue, int maxValue) {
        MaxValue = maxValue;
        MinValue = minValue;
    }
}

[Serializable]
public struct MinMaxFloat {
    public float MaxValue;
    public float MinValue;

    public MinMaxFloat(float minValue, float maxValue) {
        MaxValue = maxValue;
        MinValue = minValue;
    }
}

[DefaultExecutionOrder(0)]
[RequireComponent(typeof(DropClient))]
public class DropServer : NetworkBehaviour {

    static MinMaxFloat yDropForce = new MinMaxFloat(1, 3);
    static MinMaxFloat xDropForce = new MinMaxFloat(-3, 3);
    static float yDropDistance = .3f;
    static float pickUpDelay = 3;
    static float timeBeforeGravityEffect = 2;

    DropClient client;
    Item item;

    Vector2 dropForce = Vector2.zero;
    float timeSpawned;

    float initialY;
    bool animationPlay;

    protected virtual void Awake() {
        initialY = transform.position.y;
        animationPlay = false;

    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        gameObject.layer = LayerMask.NameToLayer("NoPickUp");

        if (!IsServer) {
            enabled = false;
            return;
        }
        
        timeSpawned = Time.time;

        client = GetComponent<DropClient>();

        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody)) {
            SetDropForce();
            rigidbody.AddForce(dropForce, ForceMode2D.Impulse);
        }

        StartCoroutine(ActivatePlayerCollider());
    }

    void Update() {
        if(!IsSpawned) return;

        

        if (transform.position.y > initialY && timeSpawned < timeSpawned + timeBeforeGravityEffect) {
            initialY = transform.position.y;
        }
         
        else if (initialY - yDropDistance >= transform.position.y) {
           

            //keep from happening multiple times
            if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody)) {
                rigidbody.gravityScale = 0;
                rigidbody.velocity = Vector2.zero;
            }
            if (!animationPlay) {
                
                client.StartIdleAnimationClientRpc();
                animationPlay = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {

        if (collider.TryGetComponent<PlayerControllerServer>(out PlayerControllerServer player)) {
            Debug.Log(item.ItemName);
            player.PickUpItemServerRpc(item);
            NetworkObject.Despawn();
        }


    }

    public void SetItem (Item item) {

        this.item = item;
        client.SetItemClientRpc(item);
    }

    public void SetDropForce() {
        System.Random random = new System.Random();
        dropForce = new Vector2((float)random.NextDouble() * (xDropForce.MaxValue - xDropForce.MinValue) + xDropForce.MinValue, (float)random.NextDouble() * (yDropForce.MaxValue - yDropForce.MinValue) + yDropForce.MinValue);
    }


    IEnumerator ActivatePlayerCollider() {
        yield return new WaitForSeconds(pickUpDelay);
        gameObject.layer = LayerMask.NameToLayer("Drop");
    }

}
