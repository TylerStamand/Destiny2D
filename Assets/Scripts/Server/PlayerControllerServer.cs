using System;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.IO;


[DefaultExecutionOrder(0)]
public class PlayerControllerServer : NetworkBehaviour, IDamageable {

    public NetworkVariable<Vector2> AnimatorMovement { get; private set; } = new NetworkVariable<Vector2>();

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


    public string GetPlayerID() {

        return PlayerPrefs.GetString("PlayerID");
    }

    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <param name="item"></param>
    public void PickUpItemServer(Item item) {
        if(!IsServer) return;
        
        Debug.Log($"Item to add {item.ItemName}");
        
        inventory.AddItemServer(item);
    }


    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <returns></returns>
    public PlayerSaveData GetSaveDataServer() {
        return new PlayerSaveData() {
            PlayerID = GetPlayerID(), 
            Items = inventory.GetItemListServer()
        };
    }

    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <param name="saveData"></param>
    public void SetSaveDataServer(PlayerSaveData saveData) {
        inventory.SetItemListServer(saveData.Items);
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage) {

        health.Value -= damage;
    }


    [ServerRpc]
    public void DropItemServerRpc(ItemInfo itemInfo) {
        string itemID  = itemInfo.ItemID.Value.ToString();
        
        Debug.Log($"Item to remove {itemID}");
        
        Item itemToDrop = inventory.GetItemServer(itemID);
        
        inventory.RemoveItemServer(itemID);

        DropServer dropPrefab = ResourceManager.DropPrefab;
       
        //Set parent for a second then remove it to keep it in right scene, might be better to have a set parent for drops
        DropServer dropInstance = Instantiate(dropPrefab, transform.position, Quaternion.identity);
        dropInstance.SetItem(itemToDrop);
        dropInstance.NetworkObject.Spawn();

    }

    [ServerRpc]
    public void UpdateAnimatorMovementServerRpc(Vector2 movement) {
        AnimatorMovement.Value = movement;
    }
 


}
