using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using System;

public class Inventory : NetworkBehaviour {
    public static int InventorySize = 35;

    public event Action<ItemInfo> OnItemAdded;
    public event Action<WeaponItem> OnWeaponChange;

    public NetworkList<ItemInfo> itemInfoList {get; private set;}

    Dictionary<string, Item> itemLookup {get; set;} = new Dictionary<string, Item>();

    List<Item> items;

    public WeaponItem CurrentWeapon {get; private set;}
    
    public NetworkVariable<ItemInfo> WeaponInfo {get; private set;}
    
    public bool IsFull => itemLookup.Count == InventorySize;


    void Awake() {
        itemInfoList = new NetworkList<ItemInfo>();
        WeaponInfo = new NetworkVariable<ItemInfo>();
        items = new List<Item>(new Item[InventorySize]);
    }

   

    public NetworkList<ItemInfo> GetItemInfoList() {
        itemInfoList.Clear();
        Debug.Log(items.Count);
        for(int i = 0; i < InventorySize; i++) {
            Item item = items[i];
            if(item != null) {

                itemInfoList.Add(item.GetItemInfo());
            }
            else {
                itemInfoList.Add(new ItemInfo());
            }
        }
        return itemInfoList;
    }




    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public Item GetItemServer(string itemID) {
        if(!IsServer) return null;
        return itemLookup[itemID];
    }


    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <returns></returns>
    public List<Item> GetItemListServer() {
        
        if(!IsServer) return null;
        return itemLookup.Values.ToList();
    }


    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <param name="items"></param>

    public void SetItemListServer(List<Item> items) {
        if(!IsServer) return;
        foreach(Item item in items) {
            if(item == null) {
                this.items[items.IndexOf(item)] = null;
                continue;
            }
            AddItemServer(item);

        }
    }


    // [ServerRpc]
    // public void AddGearToInventory(string itemID) {
    //     if(Weapon.ItemID == itemID) {
    //         AddItemServer(Weapon);
    //     }
    // }

    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <param name="item"></param>
    public void AddItemServer(Item item) {
        if(!IsServer) return;

        if(item == null) return;

        if(IsFull) {
            Debug.Log("There is no space in the inventory");
            return;
        }


        
        itemLookup.Add(item.ItemID, item);

        int i;
        for(i = 0; i < items.Count; i++) {
            if(items[i] == null) {
                items[i] = item;
                break;
            }
        }

    

        OnItemAdded?.Invoke(item.GetItemInfo());
    }


    [ServerRpc]
    public void RemoveItemServerServerRpc(string itemID) {
        RemoveItemServer(itemID);
    }

    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItemServer(string itemID) {
        if(!IsServer) return;

        Item itemToRemove = itemLookup[itemID];
        itemLookup.Remove(itemID);
        items[items.IndexOf(itemToRemove)] = null;

        Debug.Log("Removed Item in inventory");

    }

    [ServerRpc]
    public void SetWeaponServerRpc(string itemID) {

        Item item = itemLookup[itemID];
      
        if(item == null) {
            Debug.LogWarning("Trying to set weapon but item was not found in inventory");
        }

        WeaponItem newWeapon;
        if(item is WeaponItem) {
            newWeapon = (WeaponItem)item;  
        }
        else {
            Debug.LogWarning("Item is not a weapon");
            return;
        }

        if(CurrentWeapon != null) {
            AddItemServer(CurrentWeapon);
        }

        RemoveItemServer(itemID);

        SetWeaponServer(newWeapon);
    }

    void SetWeaponServer(WeaponItem weapon) {
        this.CurrentWeapon = weapon;
        WeaponInfo.Value = weapon.GetItemInfo();
        OnWeaponChange?.Invoke(weapon);
    }

    [ServerRpc]
    public void SetItemOrderServerRpc(ItemInfo[] itemInfoList) {
        for(int i = 0; i < InventorySize; i++) {
            if(i >= itemInfoList.Count()) {
                items[i] = null;
                
            }
            else if(itemInfoList[i].ItemID.Value.IsEmpty) {
                items[i] = null;
            }
            else {
                items[i] = itemLookup[itemInfoList[i].ItemID.Value.ToString()];
            }
        }
    }

}
