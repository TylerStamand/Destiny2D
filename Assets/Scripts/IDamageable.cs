using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public interface IDamageable
{

    void TakeDamageServerRpc(float damage);
}
