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

    public void SetItem(ItemInfo item) {
        this.Item = item;
        Sprite itemSprite = ResourceManager.Instance.GetItemData(item.Name.Value.ToString()).Sprite;
        itemImageSlot.sprite = itemSprite;
        
        if(TryGetComponent<Button>(out Button button)) {
            button.onClick.AddListener(HandleButtonClick);
        }
    }

    void HandleButtonClick() {
        Debug.Log("Slot item clicked");
        OnClick?.Invoke(this);
    }  





    
}
