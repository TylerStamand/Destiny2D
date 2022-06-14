using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
/// <summary>
/// Class holding Stats and a reference back to ItemData by ItemName
/// </summary>

[System.Serializable]
public class Item : INetworkSerializable{
    private string itemName;
    public string ItemName => itemName;

    public Item(string itemName) {
        this.itemName = itemName;
    }

    public Item() {
        itemName = null;
    }

    public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref itemName);
        
    }
}
