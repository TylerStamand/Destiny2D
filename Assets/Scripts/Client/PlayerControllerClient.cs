using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;

[DefaultExecutionOrder(1)]
public class PlayerControllerClient : NetworkBehaviour {

    [SerializeField] float moveSpeed;

    new Rigidbody2D rigidbody;
    Animator animator;

    
    ulong weaponID;

    Vector2 direction;
    Vector2 moveInput;


    PlayerControllerServer playerControllerServer;
    WeaponHolder weaponHolder;
    
    bool InInventory = false;
    bool initialized = false;

    void Awake() {
        playerControllerServer = GetComponent<PlayerControllerServer>();
        rigidbody = GetComponent<Rigidbody2D>();
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
            //Server rpc calls delayed by a fraction of a second.
            //StartCoroutine(Initialize());


        }

    }

    void Update() {

        if (IsLocalPlayer) {

            if (Input.GetKeyDown(KeyCode.Space)) {
                weaponHolder.UseWeapon(Utilities.DirectionFromVector2(direction));
            }


            if(Input.GetKeyDown(KeyCode.I)) {
                DisplayInventory();
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

            movement.x *= moveSpeed * Time.deltaTime;
            movement.y *= moveSpeed * Time.deltaTime;

            rigidbody.velocity = transform.right * movement.x + transform.up * movement.y;


        }


        //Until blend tree bug for network animator is fixed, this needs to run on all clients       
        animator.SetFloat("X", playerControllerServer.AnimatorMovement.Value.x);
        animator.SetFloat("Y", playerControllerServer.AnimatorMovement.Value.y);
    }

    IEnumerator Initialize() {
        yield return new WaitForSeconds(.001f);
    }

    private void DisplayInventory() {
        if(!InInventory){
            SceneManager.LoadSceneAsync("Inventory", LoadSceneMode.Additive);
            InInventory = true;
        }

        else {
            SceneManager.UnloadSceneAsync("Inventory");
            InInventory = false;
        }
    }


    [ClientRpc]
    public void SetSpawnClientRpc(Vector2 position, ClientRpcParams rpcParams) {
        transform.position = position;
    }


   

    


}
