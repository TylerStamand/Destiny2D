using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[DefaultExecutionOrder(1)]
public class PlayerControllerClient : NetworkBehaviour {

    [SerializeField] float moveSpeed;

    new Rigidbody2D rigidbody;
    Animator animator;

    MeleeWeapon weapon;
    ulong weaponID;

    Vector2 direction;
    Vector2 moveInput;

    
    PlayerControllerServer playerControllerServer;    

    bool initialized = false;

    void Awake() {
        playerControllerServer = GetComponent<PlayerControllerServer>();
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        direction = new Vector2(0, -1);     
    }

    //YOU CANT PUT SERVER RPC CALLS DIRECTLY HERE WITHOUT IT BEING ON THE SAME SCRIPT, SO STUPID
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(!IsOwner || !IsClient) {
            enabled = false;
            return;
        } 

        if (IsLocalPlayer)
        {
            rigidbody.gravityScale = 0;

            //Server rpc calls delayed by a fraction of a second.
            StartCoroutine(Initialize());
            
   
        }
   
    }

    void Update()
    {
        
        if (IsLocalPlayer)
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerControllerServer.AttackServerRpc(direction);
            }
        }
    }

    void FixedUpdate() {
        if (IsLocalPlayer)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            moveInput.x = horizontal;
            moveInput.y = vertical;
          
            if (horizontal != 0 || vertical != 0)
            {
                direction = moveInput;
            }

            //normalized to prevent moving quicker diagonally
            Vector2 movement = new Vector2(horizontal, vertical).normalized;

            movement.x *= moveSpeed * Time.deltaTime;
            movement.y *= moveSpeed * Time.deltaTime;

            rigidbody.velocity = transform.right * movement.x + transform.up * movement.y;


            animator.SetFloat("X", moveInput.x);
            animator.SetFloat("Y", moveInput.y);

        }
    }

    IEnumerator Initialize() {
        yield return new WaitForSeconds(.001f);
        if (!initialized)
        {
            playerControllerServer.InitializePlayerServerRpc(NetworkManager.Singleton.LocalClientId);
            playerControllerServer.EquipWeaponServerRpc(NetworkObjectId, NetworkManager.Singleton.LocalClientId);
        }
    }

    [ClientRpc]
    public void SetSpawnClientRpc(Vector2 position, ClientRpcParams rpcParams)
    {
        transform.position = position;
    }

    [ClientRpc]
    public void EquipWeaponClientRpc(ulong itemNetID)
    {
        NetworkObject netObj = NetworkManager.SpawnManager.SpawnedObjects[itemNetID];
        weapon = netObj.gameObject.GetComponent<MeleeWeapon>();
        weapon.transform.localPosition = Vector2.zero;

    }

    [ClientRpc]
    public void InitializePlayerClientRpc(ulong weaponHolderID)
    {
        GameObject weaponSlot = NetworkManager.SpawnManager.SpawnedObjects[weaponHolderID].gameObject;
        weaponSlot.transform.localPosition = Vector2.zero;
    }


}
