using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DropServer : NetworkBehaviour {

    [SerializeField] float yDropDistance = 1;
    [SerializeField] float pickUpDelay = 1;


    Vector2 dropForce = Vector2.zero;

    float initialY;

    bool pickUpAble;

    void Awake() {
        initialY = transform.position.y;
        pickUpAble = false;
    
    }

    public override void OnNetworkSpawn()
    {
        gameObject.layer = LayerMask.NameToLayer("NoPickUp");
        base.OnNetworkSpawn();
        if(!IsServer) {
            enabled = false;
            return;
        }
        if(TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody)) {
            rigidbody.AddForce(dropForce, ForceMode2D.Impulse);
        }
        StartCoroutine(ActivatePlayerCollider());
    }

    void Update() {
        if(initialY - yDropDistance >= transform.position.y) {
            if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody))
            {
                rigidbody.gravityScale = 0;
                rigidbody.velocity = Vector2.zero;
            }   
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        Debug.Log("TriggerEnter");
        Debug.Log(collider.name);
        if(collider.TryGetComponent<PlayerControllerServer>(out PlayerControllerServer player)) {
            PickUpAction();
            NetworkObject.Despawn();
        }
        
        
    }

    public void SetDropForce(Vector2 dropForce) {
        this.dropForce = dropForce;
    }

    protected virtual void PickUpAction() {
        Debug.Log("Picked Up");
    }

    IEnumerator ActivatePlayerCollider() {
        Debug.Log("Coroutine Start");
        yield return new WaitForSeconds(pickUpDelay);
        gameObject.layer = LayerMask.NameToLayer("Drop");
        Debug.Log("Coroutine End");
    }

}
