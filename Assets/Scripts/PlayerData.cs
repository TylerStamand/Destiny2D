using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Multiplayer.Samples.BossRoom;

[System.Serializable]
public class PlayerData : ISessionPlayerData {

    public Inventory Inventory {get; set;} = new Inventory();
    public Guid PlayerID {get; set;}

    public bool IsConnected { get; set; }
    public ulong ClientID { get; set; }

    public void Reinitialize() {
        Debug.Log($"Reinitialized on {ClientID}");
    }

    public void AddItemToInventory(Item item) {
        Debug.Log("Adding item to inventory");
        Inventory.Items.Add(item);
        Debug.Log($"There are now {Inventory.Items.Count} items");
    }
}
