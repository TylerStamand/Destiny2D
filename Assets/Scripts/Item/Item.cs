using System;
using Unity.Collections;
using Unity.Netcode;
/// <summary>
/// Class holding Stats and a reference back to ItemData by ItemName
/// </summary>

[System.Serializable]
public class Item {

    public string ItemName;

    public string ItemID {get; private set;}


    public Item(string itemName) {
        ItemID = Guid.NewGuid().ToString(); 
        ItemName = itemName;
    }
    
    public ItemInfo GetItemInfo() {
        ItemInfo itemInfo = new ItemInfo {
            ItemID = new ForceNetworkSerializeByMemcpy<FixedString64Bytes>(ItemID),
            Name = new ForceNetworkSerializeByMemcpy<FixedString64Bytes>(ItemName),
            Description = new ForceNetworkSerializeByMemcpy<FixedString512Bytes>(GetDescription())
        };
        return itemInfo;
    }
 

    public virtual string GetDescription() {
        return $"This is {ItemName} with the id {ItemID}";
    }


    // public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
    //     serializer.SerializeValue(ref itemName);
        
    // }
}

