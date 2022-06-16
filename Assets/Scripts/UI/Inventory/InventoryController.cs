using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class InventoryController : MonoBehaviour {
    [SerializeField] GameObject parent;
    [SerializeField] Slot slotPrefab;

    Inventory inventory;

    void Awake() {
        inventory = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<Inventory>();
    }

    void Display() {
        //Destroys any left over 
        Slot[] slots = parent.GetComponentsInChildren<Slot>();
        foreach (Slot slot in slots)
        {
            Destroy(slot.gameObject);
        }

        foreach(ItemInfo item in inventory.GetItemInfoList()) {
            Slot slot = Instantiate(slotPrefab);
            slot.transform.SetParent(parent.transform);
            Sprite itemSprite = ResourceManager.Instance.GetItemData(item.Name.ToString()).Sprite;
            slot.SetItemSprite(itemSprite);
        }
    }
}
