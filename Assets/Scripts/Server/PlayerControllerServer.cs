using System;
using UnityEngine;
using Unity.Netcode;

[DefaultExecutionOrder(0)]
public class PlayerControllerServer : NetworkBehaviour, IDamageable {

    public PlayerID PlayerID {get; private set;}
    public NetworkVariable<Vector2> AnimatorMovement { get; private set; } = new NetworkVariable<Vector2>();

    PlayerControllerClient playerControllerClient;
    new Rigidbody2D rigidbody;

    NetworkVariable<float> health = new NetworkVariable<float>();

    PlayerData playerData;
    Vector2 currentVelocity;
    
    void Awake() {
        playerControllerClient = GetComponent<PlayerControllerClient>();
        rigidbody = GetComponent<Rigidbody2D>();

        
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
        Debug.Log($"Is server {IsServer}");
        Debug.Log($"Is client {IsClient}");
        Debug.Log($"Is spawned { NetworkObject.IsSpawned }");
        
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
        
        playerData.AddItemToInventory(item);
    }

    [ServerRpc]
    public void UpdateAnimatorMovementServerRpc(Vector2 movement) {
        AnimatorMovement.Value = movement;
    }

    [ServerRpc] 
    public void DisplayInventoryServerRpc() {
        if(playerData != null) {
            foreach(Item item in playerData.Inventory.Items) {
                Debug.Log(item.ItemName);
                WeaponItem weaponItem = (WeaponItem)item;
                Debug.Log(weaponItem.Damage);
            }
        }
    }

    


}
