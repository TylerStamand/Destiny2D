using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ParentTest : NetworkBehaviour {
    [SerializeField] GameObject parentPrefab;

   

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        if(IsServer) {
            StartCoroutine(SetParentCo());
        }
    }

    IEnumerator SetParentCo() {
        yield return new WaitForSeconds(0.0001f);
        GameObject newParent = Instantiate(parentPrefab);
        if (newParent.TryGetComponent<NetworkObject>(out NetworkObject netObj)) {
            netObj.Spawn();
            newParent.transform.parent = this.transform;
        }
    }

    
}
