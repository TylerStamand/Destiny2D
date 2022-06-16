using System;
using UnityEngine;
using Unity.Netcode;

[DefaultExecutionOrder(0)]
public class PlayerControllerServer : NetworkBehaviour, IDamageable {

    public PlayerID PlayerID {get; private set;}
    public NetworkVariable<Vector2> AnimatorMovement { get; private set; } = new NetworkVariable<Vector2>();
    public PlayerData playerData {get; private set;}

    NetworkVariable<float> health = new NetworkVariable<float>();

    PlayerControllerClient playerControllerClient;
    new Rigidbody2D rigidbody;
    Inventory inventory;

    Vector2 currentVelocity;
    
    void Awake() {
        playerControllerClient = GetComponent<PlayerControllerClient>();
        rigidbody = GetComponent<Rigidbody2D>();
        inventory = GetComponent<Inventory>();
        
    }


    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        if (!IsServer) {
            enabled = false;
            return;
        }

        

        

        health.Value = 5;

        GameObject spawnPoint = SpawnManager.Instance.GetSpawnLocation();
        playerControllerClient.SetSpawnClientRpc(spawnPoint.transform.position, new ClientRpcParams() { Send = new ClientRpcSendParams() { TargetClientIds = new[] { OwnerClientId } } });
    }


    public Guid GetPlayerGUID() {
        if(PlayerID == null) {
            PlayerID = Resources.Load<PlayerID>("Player/PlayerID");
        }
        if (PlayerID.ID == null) {
            PlayerID.ID = System.Guid.NewGuid();
        }
        return PlayerID.ID;
    }


    public void SetPlayerData(PlayerData playerData) {    
        this.playerData = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage) {

        health.Value -= damage;
    }




    public void PickUpItem(Item item) {
        if(!IsServer) return;
        
        Debug.Log($"Item to add {item.ItemName}");
        
        
        //Got an error saying this was null
        Debug.Log(playerData.PlayerID);
        
        inventory.AddItem(item);
    }

    [ServerRpc]
    public void UpdateAnimatorMovementServerRpc(Vector2 movement) {
        AnimatorMovement.Value = movement;
    }

    [ServerRpc] 
    public void DisplayInventoryServerRpc() {
        // if(playerData != null) {
        //     foreach(Item item in playerData.Inventory.Items) {
        //         Debug.Log(item.ItemName);
        //         WeaponItem weaponItem = (WeaponItem)item;
        //         Debug.Log(weaponItem.Damage);
        //     }
        // }
        ClientRpcParams clientParams = new ClientRpcParams();
        ClientRpcSendParams sendParams = new ClientRpcSendParams();
        sendParams.TargetClientIds = new ulong[] {OwnerClientId};
        clientParams.Send = sendParams;
        playerControllerClient.DisplayInventoryClientRpc(playerData, clientParams);
    }

    


}
