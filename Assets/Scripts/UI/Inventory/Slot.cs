using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image itemImageSlot;
    [SerializeField] DescriptionBox DescriptionBoxPrefab;
 
    public bool ItemSet;
    public ItemInfo Item {get; private set;}

    public event Action<Slot> OnClick;

    Canvas canvas;

    DescriptionBox currentDescriptionObject;


    void Awake() {
        if (TryGetComponent<Button>(out Button button)) {
            button.onClick.AddListener(HandleButtonClick);
        }
        ItemSet = false;

        canvas = GetComponentInParent<Canvas>();
    }

    void OnDestroy() {
        if (currentDescriptionObject != null) {
            Destroy(currentDescriptionObject.gameObject);
        }
    }

    public void SetItem(ItemInfo item) {
        if(item.ItemID.IsEmpty) return;
        
        this.Item = item;
        Sprite itemSprite = ResourceManager.Instance.GetItemData(item.Name.Value.ToString()).Sprite;
        itemImageSlot.sprite = itemSprite;
        itemImageSlot.color = Color.white;
        
        ItemSet = true; 
    }

    void HandleButtonClick() {
        OnClick?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (Item.ItemID.IsEmpty) return;
        if (currentDescriptionObject != null) {
            Destroy(currentDescriptionObject.gameObject);
        }
        currentDescriptionObject = Instantiate(DescriptionBoxPrefab);
        currentDescriptionObject.transform.SetParent(canvas.transform);
        currentDescriptionObject.transform.SetAsLastSibling();
        currentDescriptionObject.Title.text = Item.Name.Value.ToString();
        currentDescriptionObject.Description.text = Item.Description.Value.ToString();
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (currentDescriptionObject != null) {
            Destroy(currentDescriptionObject.gameObject);
        }
    }
}
