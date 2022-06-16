using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] Image itemImageSlot;

    public void SetItemSprite(Sprite sprite) {
        itemImageSlot.sprite = sprite;
    }
}
