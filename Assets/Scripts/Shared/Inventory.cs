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
        
        for (int i = 0; i < InventorySize; i++) {
            itemInfoList.Add(new ItemInfo());
        }
    }

   

    public NetworkList<ItemInfo> GetItemInfoList() {
       
        Debug.Log("Getting Item Info List");
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
        return items;
    }


    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <param name="items"></param>

    public void SetInventoryServer(List<Item> items, WeaponItem weaponItem = null) {
        Debug.Log("Items when setting " + items.Count);
        if(!IsServer) return;
        for(int i = 0; i < InventorySize; i++) {
            Item item = items[i];
            if(item == null) {
                this.items[i] = null;
                itemInfoList[i] = new ItemInfo();
                continue;
            }

            this.items[i] = item;
            itemInfoList[i] = item.GetItemInfo();
            itemLookup.Add(item.ItemID, item);
        }

        if(weaponItem != null) {
            SetWeaponServer(weaponItem);
        }
    }


    /// <summary>
    /// Server Only Function, adds an item at the first empty slot
    /// </summary>
    /// <param name="item"></param>
    public void AddItemServer(Item item) {
        if(!IsServer) return;

        if(item == null) {
            Debug.LogWarning("Item to add to inventory is null");
            return;
        }

        if(IsFull) {
            Debug.Log("There is no space in the inventory");
            return;
        }


        
        itemLookup.TryAdd(item.ItemID, item);

        
        for(int i = 0; i < InventorySize; i++) {
            if(items[i] == null) {
                items[i] = item;
                itemInfoList[i] = item.GetItemInfo();
                Debug.Log("Item Added Successfully");
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


        Debug.Log("Removing Item Server");
        Item itemToRemove = itemLookup[itemID];

        int indexOfItemToRemove = items.IndexOf(itemToRemove); 

        itemLookup.Remove(itemID);
        items[indexOfItemToRemove] = null;
        itemInfoList[indexOfItemToRemove] = new ItemInfo();

        Debug.Log("Removed Item in inventory");

    }

    [ServerRpc]
    public void SetWeaponServerRpc(string itemID) {
        Debug.Log("Setting Weapon");
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

        //Removes item from item list but does not drop it from the lookup
        Item itemToRemove = itemLookup[itemID];
        if(items.IndexOf(itemToRemove) != -1) {
            items[items.IndexOf(itemToRemove)] = null;
            itemInfoList[items.IndexOf(itemToRemove)] = new ItemInfo();
        }

        SetWeaponServer(newWeapon);
    }

    void SetWeaponServer(WeaponItem weapon) {
        this.CurrentWeapon = weapon;
        WeaponInfo.Value = weapon.GetItemInfo();
        OnWeaponChange?.Invoke(weapon);
    }

    [ServerRpc]
    public void SetItemOrderServerRpc(ForceNetworkSerializeByMemcpy<FixedString64Bytes>[] itemInfoOrderIDs) {
        
        Debug.Log("Setting Item Order");
        for(int i = 0; i < InventorySize; i++) {
            if(i >= itemInfoOrderIDs.Count()) {
                items[i] = null;
                itemInfoList[i] = new ItemInfo();
            }
            else if(String.IsNullOrEmpty(itemInfoOrderIDs[i].Value.ToString())) {
                items[i] = null;
                itemInfoList[i] = new ItemInfo();
            }
            else {
                items[i] = itemLookup[itemInfoOrderIDs[i].Value.ToString()];
                itemInfoList[i] = items[i].GetItemInfo();
            }
        }
    }

    [ServerRpc]
    public void SetItemOrderServerRpc(int itemIndexFrom, int itemIndexTo) {
        itemInfoList[itemIndexFrom] = items[itemIndexTo].GetItemInfo();
        itemInfoList[itemIndexTo] = items[itemIndexFrom].GetItemInfo();

        Item itemTemp = items[itemIndexTo];
        items[itemIndexTo] = items[itemIndexFrom];
        items[itemIndexFrom] = itemTemp;

    }

}
