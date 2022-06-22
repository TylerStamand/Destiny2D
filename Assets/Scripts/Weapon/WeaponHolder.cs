using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;
using System;

public class WeaponHolder : NetworkBehaviour
{

    public bool Initialized {get; private set;}

    GameObject weaponSlot;
    Weapon weapon;

    public event Action OnInitializedServer; 

    ulong clientID;

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        if(IsClient) {
            StartCoroutine(Initialize());
        }
    }

    public override void OnNetworkDespawn() {
        if(IsServer) {
            weapon.NetworkObject.Despawn();
            weaponSlot.GetComponent<NetworkObject>().Despawn();
        }
        base.OnNetworkDespawn();
    }

    IEnumerator Initialize() {
        yield return new WaitForSeconds(0.0001f);
        InitializeServerRpc(NetworkManager.Singleton.LocalClientId);
        
    }


    [ClientRpc]
    public void InitializeClientRpc(ulong weaponSlotNetID) {
        NetworkObject netObj = NetworkManager.SpawnManager.SpawnedObjects[weaponSlotNetID];
        weaponSlot = netObj.gameObject;
        weaponSlot.transform.localPosition = Vector2.zero;
        
    }

    [ClientRpc]
    void EquipWeaponClientRpc(ulong itemNetID) {
        

        NetworkObject netObj = NetworkManager.SpawnManager.SpawnedObjects[itemNetID];
        weapon = netObj.gameObject.GetComponent<Weapon>();
        weapon.transform.localPosition = Vector2.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localScale = Vector3.one;

    }

    [ServerRpc]
    void InitializeServerRpc(ulong clientID) {
        
        this.clientID = clientID;
        NetworkObject networkParentPrefab = ResourceManager.Instance.NetworkParentPrefab;
        weaponSlot = Instantiate(networkParentPrefab.gameObject);
        NetworkObject networkObject = weaponSlot.GetComponent<NetworkObject>();
    
        networkObject.SpawnWithOwnership(clientID);
    
        weaponSlot.transform.SetParent(this.transform);


        Initialized = true;
        OnInitializedServer?.Invoke();

        InitializeClientRpc(networkObject.NetworkObjectId);
    }

    /// <summary>
    /// Server Only Function
    /// </summary>
    /// <param name="weaponItem"></param>
    public void EquipWeaponServer(WeaponItem weaponItem) {
        if(!IsServer) return;
        if(!Initialized)  {
            Debug.LogWarning("Tried to equip weapon before initialization was complete");
            return;
        }
        if (weapon != null) {
            weapon.NetworkObject.Despawn();
        }

        WeaponData weaponData = (WeaponData)ResourceManager.Instance.GetItemData(weaponItem.ItemName);
        
        if(weaponData == null) {
            Debug.LogError("Could not get Weapon Data");
            return;
        } 


        weapon = Instantiate(weaponData.WeaponPrefab);
    
        weapon.ParentNetID = NetworkObjectId;

        WeaponStats weaponStats = new WeaponStats {
            WeaponName = weaponItem.ItemName,
            Damage = weaponItem.Damage,
            CoolDown = weaponItem.CoolDown,
            ProjectileSpeed = weaponItem.ProjectileSpeed

        };
        
        weapon.NetworkObject.SpawnWithOwnership(clientID);

        weapon.InitializeWeaponServer(weaponStats);


        if(weaponSlot != null) {
            weapon.transform.SetParent(weaponSlot.transform);
        }else {
            Debug.LogWarning("WeaponSlot is not set");
        }

        ulong weaponID = weapon.NetworkObjectId;
        EquipWeaponClientRpc(weaponID);
    }


    public void UseWeapon(Direction direction) {
        if(IsOwner) {
            if(weapon != null) {
                weapon.AttackServerRpc(direction);
            }
            else {
                Debug.LogWarning("Weapon not initialized");
            }
        }
    }
}
