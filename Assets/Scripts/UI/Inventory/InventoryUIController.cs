using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


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
            inventory.OnInventoryChange += PopulateItemList;
            inventory.OnInventoryChange += Display;

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
        inventory.OnInventoryChange -= PopulateItemList;
        inventory.OnInventoryChange -= Display;
    }


    void PopulateItemList() {
        items.Clear();


        foreach (ItemInfo item in inventory.GetItemInfoList()) {
            items.Add(item);
            Debug.Log(item.ItemID.Value.ToString());
        }

        if(currentHeldItemInfo.ItemID.Value.ToString() != "") {
            Debug.Log("Removing held item from list");
            Debug.Log(currentHeldItemInfo.ItemID.Value.ToString());
            Debug.Log(items.Remove(currentHeldItemInfo));
        }

        weapon = inventory.weaponInfo.Value;
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
            slots.Add(slot);
        }

        for(int i = 0; i < items.Count; i++) {
            ItemInfo item = items[i];

            if(!item.ItemID.Value.IsEmpty) {
                slots[i].SetItem(item);
                slots[i].OnClick += HandleSlotClick;
            }

        }


        if(weapon.ItemID.Value.ToString() != "") {
            weaponSlot.SetItem(weapon);
        }
    }


    void HandleSlotClick(Slot slot) {
       
        items[items.IndexOf(slot.Item)] = new ItemInfo();
        Display();

        ItemInfo itemInfo = slot.Item;

        if(currentHeldUIItem != null)  {
            Destroy(currentHeldUIItem.gameObject);
            items.Add(currentHeldItemInfo);
        }
        
        currentHeldItemInfo = itemInfo;

        currentHeldUIItem = CreateHeldUIItem(currentHeldItemInfo);

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
        //inventory.RemoveItemServerServerRpc(newWeaponItemInfo.ItemID.Value.ToString());
        

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

    void HandleAddItemToInventory() {
        Debug.Log("Inventory Clicked");
       // inventory.AddGearToInventory(currentHeldItemInfo.ItemID.Value.ToString());
    }
}
