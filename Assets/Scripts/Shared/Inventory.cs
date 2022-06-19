using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class Inventory : NetworkBehaviour {
    
    NetworkList<ItemInfo> itemInfoList;

    public Dictionary<string, Item> Items {get; private set;} = new Dictionary<string, Item>();

    bool itemsDirty = true;

    void Awake() {
        itemInfoList = new NetworkList<ItemInfo>();

    }

    public override void OnNetworkSpawn() {
    } 

    public NetworkList<ItemInfo> GetItemInfoList() {
        return itemInfoList;
    }


    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public Item GetItem(string itemID) {
        if(!IsServer) return null;
        return Items[itemID];
    }


    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item) {
        if(!IsServer) return;
        Items.Add(item.ItemID, item);

        //Add to info list
        ItemInfo itemInfo = new ItemInfo {
            ItemID = new ForceNetworkSerializeByMemcpy<FixedString64Bytes>(item.ItemID),
            Name = new ForceNetworkSerializeByMemcpy<FixedString64Bytes>(item.ItemName),
            Description = new ForceNetworkSerializeByMemcpy<FixedString512Bytes>(item.GetDescription())
        };

        itemInfoList.Add(itemInfo);

        Debug.Log("Added Item to inventory");
    }

    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(string itemID) {
        if(!IsServer) return;

        Items.Remove(itemID);

        foreach(ItemInfo itemInfo in itemInfoList) {
            if(itemInfo.ItemID.Value.ToString() == itemID) {
                itemInfoList.Remove(itemInfo);
                break;
            }
        }

        Debug.Log("Removed Item in inventory");

    }


    [ServerRpc] 
    void RebuildInfoListServerRpc() {
        itemInfoList.Clear();
        foreach (string itemID in Items.Keys) {
            ItemInfo itemInfo = new ItemInfo {
                ItemID = new ForceNetworkSerializeByMemcpy<FixedString64Bytes>(itemID),
                Name = new ForceNetworkSerializeByMemcpy<FixedString64Bytes>(Items[itemID].ItemName),
                Description = new ForceNetworkSerializeByMemcpy<FixedString512Bytes>(Items[itemID].GetDescription())
            };

            itemInfoList.Add(itemInfo);
        }
        itemsDirty = false;
    }


   
}
