using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;
using Unity.Netcode.Components;

[DefaultExecutionOrder(1)]
public class PlayerControllerClient : NetworkBehaviour {

    [SerializeField] float moveSpeed;
    [SerializeField] WeaponData weaponData;

    new Rigidbody2D rigidbody;
    Animator animator;

    
    ulong weaponID;

    Vector2 direction;
    Vector2 moveInput;


    PlayerControllerServer playerControllerServer;
    WeaponHolder weaponHolder;

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
            StartCoroutine(Initialize());


        }

    }

    void Update() {

        if (IsLocalPlayer) {

            if (Input.GetKeyDown(KeyCode.Space)) {
                weaponHolder.UseWeapon(Utilities.DirectionFromVector2(direction));
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
        if (!initialized) {
            Debug.Log("Initializing Player Weapon");
            weaponHolder.EquipWeaponServerRpc(NetworkObjectId, NetworkManager.Singleton.LocalClientId, weaponData.Name);
        }
    }

    [ClientRpc]
    public void SetSpawnClientRpc(Vector2 position, ClientRpcParams rpcParams) {
        transform.position = position;
    }

    


}
