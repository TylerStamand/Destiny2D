using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;


public class InventoryUIController : MonoBehaviour {
    [SerializeField] GameObject inventorySlotsParent;
    [SerializeField] Slot weaponSlot;
    [SerializeField] Slot slotPrefab;
    [SerializeField] HeldUIItem heldUIItemPrefab;
    [SerializeField] GameObject backPanel;
    [SerializeField] GameObject mainPanel;

    PlayerControllerServer playerControllerServer;
    Inventory inventory;

    List<ItemInfo> items;
    ItemInfo weapon;
    List<Slot> slots;

    ItemInfo currentHeldItemInfo;
    HeldUIItem currentHeldUIItem;

    void Awake() {
        items = new List<ItemInfo>();
        slots = inventorySlotsParent.GetComponentsInChildren<Slot>().ToList();
        playerControllerServer = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerControllerServer>(); 
        inventory = playerControllerServer.GetComponent<Inventory>();
        currentHeldUIItem = null;
        weaponSlot.OnClick += HandleWeaponSlotClick;
      
        
        if(inventory != null) {

            inventory.itemInfoList.OnListChanged += HandleInventoryItemListUpdate;
            inventory.WeaponInfo.OnValueChanged += HandleWeaponItemUpdate;

            PopulateItemList();
            Display();
        }
        else {
            Debug.Log("Player does not have an inventory, can not display Inventory");
        }
    }

    void Update() {

    }

    void OnDestroy() {
        if(inventory != null) {
            inventory.itemInfoList.OnListChanged -= HandleInventoryItemListUpdate;
            inventory.WeaponInfo.OnValueChanged -= HandleWeaponItemUpdate;

            //Deals with exiting inventory with item in hand
            if (currentHeldUIItem != null) {
                for (int i = 0; i < Inventory.InventorySize; i++) {
                    if (items[i].ItemID.Value.IsEmpty) {
                        items[i] = currentHeldItemInfo;
                        break;
                    }
                }
            }

            ItemInfo[] itemInfoArray = new ItemInfo[Inventory.InventorySize];
            for (int i = 0; i < items.Count; i++) {
                itemInfoArray[i] = items[i];

            }
            inventory.SetItemOrderServerRpc(itemInfoArray);
        }
       

    }


    void PopulateItemList() {
        items.Clear();

        Debug.Log("Items in info list " + inventory.GetItemInfoList().Count);
        foreach (ItemInfo item in inventory.GetItemInfoList()) {
            items.Add(item);
        }

        Debug.Log("Items in item list " +  items.Count);

        weapon = inventory.WeaponInfo.Value;
    }

    void Display() {
        
        foreach (Slot slot in slots)
        {
            Destroy(slot.gameObject);
        }

        slots.Clear();



        for (int i = 0; i < Inventory.InventorySize; i++) {
            Slot slot = Instantiate(slotPrefab);
            slot.transform.SetParent(inventorySlotsParent.transform);
            slot.OnClick += HandleSlotClick;
            slots.Add(slot);

            ItemInfo item = items[i];
            if (!item.ItemID.Value.IsEmpty) {
                slot.SetItem(item);
            }
        }


        if(!weapon.ItemID.Value.IsEmpty) {
            weaponSlot.SetItem(weapon);
        }
    }


    void HandleSlotClick(Slot slot) {
        
        
        ItemInfo slotItemInfo = slot.Item;
       
        //Removes the old item info from the slot
        if(currentHeldUIItem == null) {
            items[items.IndexOf(slotItemInfo)] = new ItemInfo();
        }
        //Changes the item slot to the item currently held
        else {
            items[slots.IndexOf(slot)] = currentHeldItemInfo;
            Destroy(currentHeldUIItem.gameObject);
        }
        
        //If the slot held an item, create a held item
        if(!slotItemInfo.ItemID.Value.IsEmpty) {
            currentHeldItemInfo = slotItemInfo;
            currentHeldUIItem = CreateHeldUIItem(currentHeldItemInfo);
        }

        //Otherwise set the held item to null
        else {
            currentHeldUIItem = null;
        }

        Display();

       

    }

    void HandleItemDrop() {
        if(currentHeldUIItem != null) {
            playerControllerServer.DropItemServerRpc(currentHeldItemInfo);
            Destroy(currentHeldUIItem.gameObject);
            currentHeldUIItem = null;
            backPanel.GetComponent<Button>().onClick.RemoveListener(HandleItemDrop);
        }

    }


    void HandleWeaponSlotClick(Slot slot) {
        Debug.Log("Handling Weapon Slot Click");
        if(currentHeldUIItem == null) return;
        
        ItemData itemData = ResourceManager.Instance.GetItemData(currentHeldItemInfo.Name.Value.ToString());
        
        //Item is not a weapon
        if(itemData.ItemType != ItemType.Weapon) return;

        ItemInfo oldWeaponItemInfo = slot.Item;
        
        Destroy(currentHeldUIItem.gameObject);
        currentHeldUIItem = null;



        //Checks if the weapon slot had an item in it
        if(oldWeaponItemInfo.ItemID.Value.ToString() != "") {
            currentHeldUIItem = CreateHeldUIItem(oldWeaponItemInfo);
        }

        ItemInfo newWeaponItemInfo = currentHeldItemInfo;
        currentHeldItemInfo = oldWeaponItemInfo;

        slot.SetItem(newWeaponItemInfo);
        inventory.SetWeaponServerRpc(newWeaponItemInfo.ItemID.Value.ToString());


    }

    HeldUIItem CreateHeldUIItem(ItemInfo itemInfo) {
        HeldUIItem UIItem = Instantiate(heldUIItemPrefab);
        SceneManager.MoveGameObjectToScene(UIItem.gameObject, SceneManager.GetSceneByName("Inventory"));
        UIItem.transform.SetParent(mainPanel.transform);
        UIItem.transform.SetAsLastSibling();
        UIItem.GetComponent<Image>().sprite = ResourceManager.Instance.GetItemData(itemInfo.Name.Value.ToString()).Sprite;
        backPanel.GetComponent<Button>().onClick.AddListener(HandleItemDrop);
       

        return UIItem;
    }

    void AddItemToList(ItemInfo item) {
        
    }

    void HandleInventoryItemListUpdate(NetworkListEvent<ItemInfo> itemChangeEvent) {
        //Means inventory picked something up
        if(currentHeldItemInfo.ItemID.Value.ToString() != itemChangeEvent.Value.ItemID.Value.ToString()) {
            if(itemChangeEvent.Value.ItemID.Value.IsEmpty) {
                items[itemChangeEvent.Index] = new ItemInfo();
            }
            else {
                for(int i = 0; i < items.Count; i++) {
                    if(items[i].ItemID.Value.IsEmpty) {
                        items[i] = itemChangeEvent.Value;
                    }
                }
            }

        }
    }

    void HandleWeaponItemUpdate(ItemInfo prev, ItemInfo newVal) {
        weapon = newVal;
        Display();
    }
}
