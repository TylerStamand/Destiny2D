using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WeaponHolder : NetworkBehaviour
{
    [SerializeField] NetworkObject networkParentPrefab;
    [SerializeField] MeleeWeapon weaponPrefab;
    
    GameObject weaponSlot;
    MeleeWeapon weapon;

    

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        if(IsOwner) {
            StartCoroutine(Initialize());
        }
    }

    public override void OnNetworkDespawn() {
        weapon.NetworkObject.Despawn();
        weaponSlot.GetComponent<NetworkObject>().Despawn();
    }

    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject) {
        base.OnNetworkObjectParentChanged(parentNetworkObject);
        Debug.Log("Parent Changed");
    }

    IEnumerator Initialize() {
        yield return new WaitForSeconds(0.0001f);
        InitializeServerRpc(NetworkManager.Singleton.LocalClientId, IsLocalPlayer);
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
        weapon = netObj.gameObject.GetComponent<MeleeWeapon>();
        weapon.transform.localPosition = Vector2.zero;

    }

    [ServerRpc]
    void InitializeServerRpc(ulong clientID, bool isPlayer = false) {
        
        weaponSlot = Instantiate(networkParentPrefab.gameObject);
        NetworkObject networkObject = weaponSlot.GetComponent<NetworkObject>();
    
        networkObject.Spawn();
        
        
        weaponSlot.transform.parent = this.transform;

        InitializeClientRpc(networkObject.NetworkObjectId);
    }

    [ServerRpc]
    public void EquipWeaponServerRpc(ulong parentNetID, ulong clientID) {
     
        weapon = Instantiate(weaponPrefab);
    
        weapon.NetworkObject.Spawn();
        

        weapon.SetParentClientRpc(parentNetID);
        if(weaponSlot != null) {
            weapon.transform.SetParent(weaponSlot.transform);
        }

        ulong weaponID = weapon.NetworkObjectId;
        EquipWeaponClientRpc(weaponID);
    }

    public void UseWeapon(Direction direction) {
        if(IsOwner) {
            if(weapon != null) {
                weapon.AttackServerRpc(direction);
            }
            // else {
            //     Debug.LogWarning("Weapon not initialized");
            // }
        }
    }
}
