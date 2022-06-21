using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class Inventory : NetworkBehaviour {
    
    NetworkList<ItemInfo> itemInfoList;

    Dictionary<string, Item> Items {get; set;} = new Dictionary<string, Item>();

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
    public Item GetItemServer(string itemID) {
        if(!IsServer) return null;
        return Items[itemID];
    }


    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <returns></returns>
    public List<Item> GetItemListServer() {
        
        if(!IsServer) return null;
        return Items.Values.ToList();
    }

    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <param name="items"></param>

    public void SetItemListServer(List<Item> items) {
        if(!IsServer) return;
        foreach(Item item in items) {
            Items.Add(item.ItemID, item);
            AddItemToInfoList(item);
        }
    }

    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <param name="item"></param>
    public void AddItemServer(Item item) {
        if(!IsServer) return;
        Items.Add(item.ItemID, item);
        AddItemToInfoList(item);
    }

    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItemServer(string itemID) {
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

    void AddItemToInfoList(Item item) {
        ItemInfo itemInfo = new ItemInfo {
            ItemID = new ForceNetworkSerializeByMemcpy<FixedString64Bytes>(item.ItemID),
            Name = new ForceNetworkSerializeByMemcpy<FixedString64Bytes>(item.ItemName),
            Description = new ForceNetworkSerializeByMemcpy<FixedString512Bytes>(item.GetDescription())
        };

        itemInfoList.Add(itemInfo);
    }

   
}
