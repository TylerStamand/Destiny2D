using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEditor;

public class WeaponDropServer : DropServer
{
    [SerializeField] WeaponStats weaponStats;


    WeaponData weaponData;

    protected override void Awake() {
        base.Awake();
        weaponData = ResourceSystem.Instance.GetWeaponData(weaponStats.WeaponName) ;
        GetComponentInChildren<SpriteRenderer>().sprite = weaponData.Sprite;
    }

    void OnValidate() {
        //Keeps from send message error

        EditorApplication.delayCall += Validate;
    }

    void Validate() {
        //keeps from missing ref exception
        if(this == null) {
            UnityEditor.EditorApplication.delayCall -= Validate;
            return;
        }
        if (weaponData != null && weaponData.Sprite != null) {
            SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null) {
                spriteRenderer.sprite = weaponData.Sprite;
            }
        }

    }



    protected override void PickUpAction(PlayerControllerServer player) {
        player.GetComponent<PlayerControllerServer>().AddWeaponToInventoryServerRpc(weaponStats);

    }

    // [ServerRpc]
    // public void SetWeaponDataServerRpc(string weaponName) {
    //     weaponData = ResourceSystem.Instance.GetWeaponData(weaponName);
        
    // }
}
