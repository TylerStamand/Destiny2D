using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[DefaultExecutionOrder(0)]
public class PlayerControllerServer : NetworkBehaviour, IDamageable {


    public NetworkVariable<Vector2> AnimatorMovement { get; private set; } = new NetworkVariable<Vector2>();

    PlayerControllerClient playerControllerClient;
    MeleeWeapon weapon;
    GameObject weaponSlot;
    NetworkVariable<float> health = new NetworkVariable<float>();

    
    void Awake() {
        playerControllerClient = GetComponent<PlayerControllerClient>();
        if(TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody)) {
            rigidbody.gravityScale = 0;
        }
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        if (!IsServer) {
            enabled = false;
            return;
        }

        health.Value = 5;

        GameObject spawnPoint = SpawnManager.Instance.GetSpawnLocation();
        playerControllerClient.SetSpawnClientRpc(spawnPoint.transform.position, new ClientRpcParams() { Send = new ClientRpcSendParams() { TargetClientIds = new[] { OwnerClientId } } });
    }


    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage) {

        health.Value -= damage;
    }

    [ServerRpc]
    public void UpdateAnimatorMovementServerRpc(Vector2 movement) {
        AnimatorMovement.Value = movement;
    }


}
