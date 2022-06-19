using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class InventoryController : MonoBehaviour {
    [SerializeField] GameObject parent;
    [SerializeField] Slot slotPrefab;
    [SerializeField] HeldUIItem heldUIItemPrefab;
    [SerializeField] GameObject BackPanel;

    PlayerControllerServer playerControllerServer;
    Inventory inventory;

    List<ItemInfo> items;
    List<Slot> slots;



    ItemInfo currentHeldItemInfo;
    HeldUIItem currentHeldUIItem;

    void Awake() {
        items = new List<ItemInfo>();
        slots = GetComponentsInChildren<Slot>().ToList();
        playerControllerServer = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerControllerServer>(); 
        inventory = playerControllerServer.GetComponent<Inventory>();

      
        
        
        if(inventory != null) {
            PopulateItemList();
            Display();
        }
        else {
            Debug.Log("Player does not have an inventory, can not display Inventory");
        }
    }

    void Update() {

    }


    void PopulateItemList() {
        foreach (ItemInfo item in inventory.GetItemInfoList()) {
            items.Add(item);
        }
    }

    void Display() {
        //Destroys any left over 
        
        foreach (Slot slot in slots)
        {
            Destroy(slot.gameObject);
        }

        slots.Clear();

        foreach(ItemInfo item in items) {
            Slot slot = Instantiate(slotPrefab);
            slot.transform.SetParent(parent.transform);
        
            
            slot.SetItem(item);
            slot.OnClick += HandleSlotClick;

            slots.Add(slot);
        }
    }


    void HandleSlotClick(Slot slot) {
        //TODO: Spawn heldItem in the inventory scene

        items.Remove(slot.Item);
        Display();

        ItemInfo itemInfo = slot.Item;
        
        currentHeldItemInfo = itemInfo;

        currentHeldUIItem = Instantiate(heldUIItemPrefab);
        SceneManager.MoveGameObjectToScene(currentHeldUIItem.gameObject, SceneManager.GetSceneByName("Inventory"));
        currentHeldUIItem.GetComponent<SpriteRenderer>().sprite = ResourceManager.Instance.GetItemData(itemInfo.Name.Value.ToString()).Sprite;
        BackPanel.GetComponent<Button>().onClick.AddListener(HandleItemDrop);


    }

    void HandleItemDrop() {
        playerControllerServer.DropItemServerRpc(currentHeldItemInfo);
        Destroy(currentHeldUIItem.gameObject);
        BackPanel.GetComponent<Button>().onClick.RemoveListener(HandleItemDrop);
    }
}
