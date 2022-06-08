using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Multiplayer.Samples.BossRoom;

[System.Serializable]
public struct PlayerData : ISessionPlayerData {

    public Inventory Inventory {get; set;}
    public Guid PlayerID {get; set;}

    public bool IsConnected { get; set; }
    public ulong ClientID { get; set; }

    public void Reinitialize() {
        Debug.Log($"Reinitialized on {ClientID}");
    }

    public void AddItemToInventory(WeaponStats weapon) {
        Inventory.Items.Add(weapon);
    }
}
