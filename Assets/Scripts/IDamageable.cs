using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public interface IDamageable
{
    [ServerRpc]
    void TakeDamageServerRpc(float damage);
}
