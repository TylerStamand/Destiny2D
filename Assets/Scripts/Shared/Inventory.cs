using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using System.Linq;

public class Inventory : NetworkBehaviour {
    
    NetworkList<ItemInfo> itemInfoList = new NetworkList<ItemInfo>();  

    public Dictionary<string, Item> Items {get; private set;} = new Dictionary<string, Item>();

    bool itemsDirty = true;

    public NetworkList<ItemInfo> GetItemInfoList() {
        if(itemsDirty) {
            itemInfoList.Clear();
            foreach (string itemID in Items.Keys) {
                ItemInfo itemInfo = new ItemInfo {
                    ItemID = itemID,
                    Name = Items[itemID].ItemName,
                    Description = Items[itemID].GetDescription()
                };
                
                itemInfoList.Add(itemInfo);
            }
            itemsDirty = false;
        }
        return itemInfoList;
    }

    public void AddItem(Item item) {
        if(!IsServer) return;
        Items.Add(item.ItemID, item);
        itemsDirty = true;
    }  


    // public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {

        
    //     Item[] items = null; 

    //     int length = 0;

    //     if(!serializer.IsWriter) {
    //         items = Items.ToArray();
    //         length = items.Length;
    //     }

    //     serializer.SerializeValue(ref length);

    //     if(serializer.IsReader) {
    //         items = new Item[length];
    //     }

    //     for(int i = 0; i < length; i++) {
    //         serializer.SerializeValue(ref items[i]);
    //     }

    //     if(serializer.IsReader) {
    //         Items = new List<Item>(items);
    //     }
    // }
}
