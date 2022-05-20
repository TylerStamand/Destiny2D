using UnityEngine;
using Unity.Netcode;

public class Enemy : NetworkBehaviour, IDamageable {

    [field: SerializeField] public NetworkVariable<float> Health {get; private set;} = new NetworkVariable<float>();

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage) {
        Health.Value -= damage;
        Debug.Log(gameObject.name + " health " + Health.Value);

        if(Health.Value <= 0) {
            NetworkObject.Despawn();
        }
    }
}
