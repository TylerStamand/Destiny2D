using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] Image itemImageSlot;
 
    public ItemInfo Item {get; private set;}

    public event Action<Slot> OnClick;

    void Awake() {
        if (TryGetComponent<Button>(out Button button)) {
            button.onClick.AddListener(HandleButtonClick);
        }
    }

    public void SetItem(ItemInfo item) {
        this.Item = item;
        Sprite itemSprite = ResourceManager.Instance.GetItemData(item.Name.Value.ToString()).Sprite;
        itemImageSlot.sprite = itemSprite;
        itemImageSlot.color = Color.white;
        
        
    }

    void HandleButtonClick() {
        OnClick?.Invoke(this);
    }  





    
}
