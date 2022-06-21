using System;
/// <summary>
/// Class holding Stats and a reference back to ItemData by ItemName
/// </summary>


public class Item {

    public string ItemName;

    public string ItemID {get; private set;}

    //Item Type

    public bool IsStackAble {get; }

    public Item(string itemName) {
        ItemID = Guid.NewGuid().ToString(); 
        ItemName = itemName;
    }

    public virtual string GetDescription() {
        return $"This is {ItemName} with the id {ItemID}";
    }

    // public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
    //     serializer.SerializeValue(ref itemName);
        
    // }
}

// Item Types enum