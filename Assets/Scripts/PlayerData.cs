using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Multiplayer.Samples.BossRoom;

[System.Serializable]
public class PlayerData : ISessionPlayerData {

    public Inventory Inventory {get; set;}
    public Guid PlayerID {get; set;}

    public bool IsConnected { get; set; }
    public ulong ClientID { get; set; }

    public void Reinitialize() {
        Debug.Log($"Reinitialized on {ClientID}");
    }

    public void AddItemToInventory(Item item) {
        Inventory.Items.Add(item);
    }
}
