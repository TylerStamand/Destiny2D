using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;

[DefaultExecutionOrder(1)]
public class PlayerControllerClient : NetworkBehaviour {

    [SerializeField] float moveSpeed;
    [SerializeField] float unitCollisionDistance;
    [SerializeField] ContactFilter2D contactFilter;
    [SerializeField] GameObject InventoryUI;
    [SerializeField] GameObject HUDUI;

    new Rigidbody2D rigidbody;
    new Collider2D collider;
    Animator animator;
    PlayerControllerServer playerControllerServer;
    WeaponHolder weaponHolder;

    
    ulong weaponID;

    Vector2 direction;
    Vector2 moveInput;



    GameObject displayObject;
    ClientDisplay currentDisplay;

    bool InMenu = false;
    bool initialized = false;

    void Awake() {
        playerControllerServer = GetComponent<PlayerControllerServer>();
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        weaponHolder = GetComponent<WeaponHolder>();
        direction = new Vector2(0, -1);

       

    }


    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        if (!IsClient) {
            enabled = false;
            return;
        }

        if (IsLocalPlayer) {
            rigidbody.gravityScale = 0;
            ChangeDisplay(ClientDisplay.HUD);
            Instantiate(ResourceManager.Instance.CameraFollowerPrefab, transform);

        }

    }


    void Update() {

        

        if (IsLocalPlayer) {

            if (Input.GetKeyDown(KeyCode.Space)) {
                weaponHolder.UseWeapon(Utilities.DirectionFromVector2(direction));
            }

            if(Input.GetKeyDown(KeyCode.I)) {
                if(currentDisplay == ClientDisplay.Inventory) {
                    ChangeDisplay(ClientDisplay.HUD);
                }
                else {
                    ChangeDisplay(ClientDisplay.Inventory);
                }
            }

            
        }
    }

    void FixedUpdate() {
        if (IsLocalPlayer) {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            moveInput.x = horizontal;
            moveInput.y = vertical;

            playerControllerServer.UpdateAnimatorMovementServerRpc(moveInput);

            if (horizontal != 0 || vertical != 0) {
                direction = moveInput;
            }

            //normalized to prevent moving quicker diagonally
            Vector2 movement = new Vector2(horizontal, vertical).normalized;

            RaycastHit2D[] results = new RaycastHit2D[1];
            
          
            int numOfHits = collider.Cast(movement, contactFilter, results, moveSpeed * Time.deltaTime * Time.deltaTime);
            
            if(numOfHits > 0) {
                movement = Vector2.zero;
            }
            else {
                movement.x *= moveSpeed * Time.deltaTime;
                movement.y *= moveSpeed * Time.deltaTime;

            }

            

            rigidbody.velocity = transform.right * movement.x + transform.up * movement.y;


        }


        //Until blend tree bug for network animator is fixed, this needs to run on all clients       
        animator.SetFloat("X", playerControllerServer.AnimatorMovement.Value.x);
        animator.SetFloat("Y", playerControllerServer.AnimatorMovement.Value.y);
    }

    public override void OnDestroy() {
        Destroy(displayObject);
    }


    /// <summary>
    /// Change the client UI
    /// </summary>
    /// <param name="display"></param>
    private void ChangeDisplay(ClientDisplay display) {
        

        if(currentDisplay == display) {
            return;
        }
        
        currentDisplay = display;
        
        InMenu = false;

        if(displayObject != null) {
            Destroy(displayObject);
        }


        switch(display) {
            case ClientDisplay.None:
                break;
            case ClientDisplay.HUD:
                displayObject = Instantiate(HUDUI);
                break;
            case ClientDisplay.Inventory:
                displayObject = Instantiate(InventoryUI);
                InMenu = true;
                break;
        }

    }



    [ClientRpc]
    public void SetSpawnClientRpc(Vector2 position, ClientRpcParams rpcParams) {
        ClientNetworkTransform clientTransform = GetComponent<ClientNetworkTransform>();
        clientTransform.Interpolate = false;
        transform.position = position;
        clientTransform.Interpolate = true;
    }


   

    


}


enum ClientDisplay {
    None, HUD, Inventory, Settings
}