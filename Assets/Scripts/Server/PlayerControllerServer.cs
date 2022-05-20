using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[DefaultExecutionOrder(0)]
public class PlayerControllerServer : NetworkBehaviour, IDamageable {


    [SerializeField] NetworkObject networkParentPrefab;
    [SerializeField] MeleeWeapon weaponPrefab;

    public NetworkVariable<Vector2> AnimatorMovement {get; private set;} = new NetworkVariable<Vector2>();

    PlayerControllerClient playerControllerClient;
    MeleeWeapon weapon;
    GameObject weaponSlot;
    NetworkVariable<float> health = new NetworkVariable<float>();
    void Awake() {
        playerControllerClient = GetComponent<PlayerControllerClient>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
      
        if(!IsServer) {
            enabled = false;
            return;
        }

        health.Value = 5;
       
        GameObject spawnPoint = SpawnManager.Instance.GetSpawnLocation();
        playerControllerClient.SetSpawnClientRpc(spawnPoint.transform.position, new ClientRpcParams() { Send = new ClientRpcSendParams() { TargetClientIds = new[] { OwnerClientId } } });
    }


    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
     
        health.Value -= damage;
        Debug.Log("Object " + NetworkObjectId);
        Debug.Log("Health: " + health.Value);
    }
    
    [ServerRpc]
    public void UpdateAnimatorMovementServerRpc(Vector2 movement) {
        AnimatorMovement.Value = movement;
    }

    // [ServerRpc]
    // public void AttackServerRpc(Vector2 Direction)
    // {
    //     weapon.AttackClientRpc(Direction);
    // }


    [ServerRpc]
    public void InitializePlayerServerRpc(ulong clientID)
    {
        weaponSlot = Instantiate(networkParentPrefab.gameObject);
        NetworkObject networkObject = weaponSlot.GetComponent<NetworkObject>();
        networkObject.SpawnAsPlayerObject(clientID);

        weaponSlot.transform.SetParent(transform);


        playerControllerClient.InitializePlayerClientRpc(networkObject.NetworkObjectId);
    }

    [ServerRpc]
    public void EquipWeaponServerRpc(ulong parentNetID, ulong clientID)
    {

        weapon = Instantiate(weaponPrefab);
        weapon.NetworkObject.SpawnAsPlayerObject(clientID);

        weapon.SetParentClientRpc(parentNetID);
        weapon.transform.SetParent(weaponSlot.transform);


        ulong weaponID = weapon.NetworkObjectId;

        playerControllerClient.EquipWeaponClientRpc(weaponID);
    }
}
