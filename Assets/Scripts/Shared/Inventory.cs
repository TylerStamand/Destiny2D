using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using System;

public class Inventory : NetworkBehaviour {
    

    public event Action OnInventoryChange;
    public event Action<WeaponItem> OnWeaponChange;

    public NetworkList<ItemInfo> itemInfoList {get; private set;}

    Dictionary<string, Item> Items {get; set;} = new Dictionary<string, Item>();

    public WeaponItem Weapon {get; private set;}
    
    public NetworkVariable<ItemInfo> weaponInfo {get; private set;}
    
    


    void Awake() {
        itemInfoList = new NetworkList<ItemInfo>();
        weaponInfo = new NetworkVariable<ItemInfo>();
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
            itemInfoList.Add(item.GetItemInfo());
        }
        OnInventoryChange?.Invoke();
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
        Items.Add(item.ItemID, item);
        itemInfoList.Add(item.GetItemInfo());

        OnInventoryChange?.Invoke();
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

        Items.Remove(itemID);

        foreach(ItemInfo itemInfo in itemInfoList) {
            if(itemInfo.ItemID.Value.ToString() == itemID) {
                itemInfoList.Remove(itemInfo);
                break;
            }
        }

        Debug.Log("Removed Item in inventory");
        OnInventoryChange?.Invoke();
    }

    [ServerRpc]
    public void SetWeaponServerRpc(string itemID) {

        Item item = Items[itemID];

        WeaponItem newWeapon;
        if(item is WeaponItem) {
            newWeapon = (WeaponItem)item;  
        }
        else {
            Debug.LogWarning("Item is not a weapon");
            return;
        }


        if(newWeapon == null) {
            Debug.LogWarning("Trying to set weapon but item was not found in inventory");
        }

        if(Weapon != null) {
            AddItemServer(Weapon);
        }

        RemoveItemServer(itemID);

        SetWeaponServer(newWeapon);
    }

    void SetWeaponServer(WeaponItem weapon) {
        this.Weapon = weapon;
        weaponInfo.Value = weapon.GetItemInfo();
        OnWeaponChange?.Invoke(weapon);
        OnInventoryChange?.Invoke();
    }

}
