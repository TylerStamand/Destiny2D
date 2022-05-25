using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

[Serializable]
public struct MinMaxInt
{
    public int MaxValue;
    public int MinValue;

    public MinMaxInt(int minValue, int maxValue) {
        MaxValue = maxValue;
        MinValue = minValue;
    }
}

[Serializable]
public struct MinMaxFloat
{
    public float MaxValue;
    public float MinValue;

    public MinMaxFloat(float minValue, float maxValue)
    {
        MaxValue = maxValue;
        MinValue = minValue;
    }
}


public class DropServer : NetworkBehaviour {

    static MinMaxFloat yDropForce = new MinMaxFloat(1, 5);
    static MinMaxFloat xDropForce = new MinMaxFloat(-5, 5);
    [SerializeField] float yDropDistance = 1;
    [SerializeField] float pickUpDelay = 1;

    DropClient client;
    Vector2 dropForce = Vector2.zero;

    float initialY;

    bool animationPlay;

    void Awake() {
        initialY = transform.position.y;
        animationPlay = false;
    
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        
        gameObject.layer = LayerMask.NameToLayer("NoPickUp");
        
        if(!IsServer) {
            enabled = false;
            return;
        }

        client = GetComponent<DropClient>();

        if(TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody)) {
            SetDropForce();
            rigidbody.AddForce(dropForce, ForceMode2D.Impulse);
        }
        StartCoroutine(ActivatePlayerCollider());
    }

    void Update() {
        if(transform.position.y > initialY) {
            initialY = transform.position.y;
        }

        else if(initialY - yDropDistance >= transform.position.y) {
            if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody))
            {
                rigidbody.gravityScale = 0;
                rigidbody.velocity = Vector2.zero;
            }   
            if(!animationPlay) {
                client.StartIdleAnimationClientRpc();
                animationPlay = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        
        if(collider.TryGetComponent<PlayerControllerServer>(out PlayerControllerServer player)) {
            PickUpAction();
            NetworkObject.Despawn();
        }
        
        
    }

    public void SetDropForce() {
        System.Random random = new System.Random();
        dropForce = new Vector2((float)random.NextDouble() * (xDropForce.MaxValue - xDropForce.MinValue) + xDropForce.MinValue, (float)random.NextDouble() * (yDropForce.MaxValue - yDropForce.MinValue) + yDropForce.MinValue);
    }

    protected virtual void PickUpAction() {
        Debug.Log("Picked Up");
    }

    IEnumerator ActivatePlayerCollider() {
        yield return new WaitForSeconds(pickUpDelay);
        gameObject.layer = LayerMask.NameToLayer("Drop");
    }

}
