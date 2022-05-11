using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerController : NetworkBehaviour { 

    [SerializeField] float moveForce;
    [SerializeField] float topSpeed;
    [SerializeField] MeleeWeapon weaponPrefab;
    [SerializeField] GameObject weaponSlot;
    [SerializeField] NetworkBehaviour networkParentPrefab; 
    new Rigidbody2D rigidbody;
    Animator animator;

    MeleeWeapon weapon;
    ulong weaponID;

    Vector2 direction;
    NetworkVariable<Vector2> moveInput;
    Vector2 oldMoveInput;

    void Awake() {
        direction = new Vector2(0, -1);
        moveInput = new NetworkVariable<Vector2>();
    }

    void Update() {
        if(IsLocalPlayer) {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AttackServerRpc(direction);
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        

        rigidbody.gravityScale = 0;
        if(IsLocalPlayer) {
            InitializePlayerServerRpc();
            EquipWeaponServerRpc(NetworkManager.Singleton.LocalClientId);
        }
        
    }

    void FixedUpdate() {
        if(IsLocalPlayer) {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            oldMoveInput.x = horizontal;
            oldMoveInput.y = vertical;
            UpdateMoveInputServerRpc(oldMoveInput);
            if (horizontal != 0 || vertical != 0)
            {
                direction = oldMoveInput;
            }

            //normalized to prevent moving quicker diagonally
            Vector2 inputMovement = new Vector2(horizontal, vertical).normalized;

            inputMovement.x *= moveForce * Time.deltaTime;
            inputMovement.y *= moveForce * Time.deltaTime;

            rigidbody.velocity = transform.right * inputMovement.x + transform.up * inputMovement.y;


        }
        else {
            oldMoveInput = moveInput.Value; 
        }

        animator.SetFloat("X", oldMoveInput.x);
        animator.SetFloat("Y", oldMoveInput.y);
       
    }


    [ServerRpc]
    void AttackServerRpc(Vector2 Direction) {
        weapon.AttackClientRpc(Direction);
    }

    [ServerRpc]
    void UpdateMoveInputServerRpc(Vector2 moveInput) {
        this.moveInput.Value = moveInput;
    }

    [ServerRpc]
    void InitializePlayerServerRpc() {
        GameObject weaponHolder = Instantiate(networkParentPrefab.gameObject);
        NetworkObject networkObject = weaponHolder.GetComponent<NetworkObject>();
        networkObject.Spawn();

        weaponHolder.transform.SetParent(transform);
        weaponHolder.transform.localPosition = Vector2.zero;

        InitializePlayerClientRpc(networkObject.NetworkObjectId);
    }

    [ServerRpc]
    void EquipWeaponServerRpc(ulong netID) {

        MeleeWeapon weapon = Instantiate(weaponPrefab);
        weapon.NetworkObject.Spawn();

        weapon.transform.SetParent(weaponSlot.transform);
        weapon.transform.localPosition = Vector2.zero;
        
        //Desyncs until moved in inspector
        
        weaponID = weapon.NetworkObjectId;

        EquipWeaponClientRpc(weaponID);
    }

    [ClientRpc]
    void EquipWeaponClientRpc(ulong itemNetID ) {
        NetworkObject netObj = NetworkManager.SpawnManager.SpawnedObjects[itemNetID];
        weapon = netObj.gameObject.GetComponent<MeleeWeapon>();
    }

    [ClientRpc]
    void InitializePlayerClientRpc(ulong weaponHolderID) {
        weaponSlot = NetworkManager.SpawnManager.SpawnedObjects[weaponHolderID].gameObject;
    }
}
