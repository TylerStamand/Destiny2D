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
    WeaponHolder weaponHolder;
    Vector2 currentVelocity;

    
    void Awake() {
        playerControllerClient = GetComponent<PlayerControllerClient>();
        rigidbody = GetComponent<Rigidbody2D>();
        inventory = GetComponent<Inventory>();
        weaponHolder = GetComponent<WeaponHolder>();
        
    }


    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        if (!IsServer) {
            enabled = false;
            return;
        }

        inventory.OnWeaponChange += weaponHolder.EquipWeaponServer;
        // weaponHolder.OnInitializedServer += () => weaponHolder.EquipWeaponServer(inventory.Weapon);

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
            Items = inventory.GetItemListServer(),
            Weapon = inventory.CurrentWeapon  
        };
    }

    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <param name="saveData"></param>
    public void SetSaveDataServer(PlayerSaveData saveData) {
        inventory.SetInventoryServer(saveData.Items, saveData.Weapon);

        //This is called because when the inventory sets the weapon, weapon holder hasn't initialized yet
        //so it needs to be done manually
        weaponHolder.OnInitializedServer += () => weaponHolder.EquipWeaponServer(saveData.Weapon);
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

        DropServer dropPrefab = ResourceManager.Instance.DropPrefab;
       
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
