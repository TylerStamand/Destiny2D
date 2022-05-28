using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class test2 : NetworkBehaviour
{
    [ServerRpc]
    public void LogServerRpc() {
        Debug.Log("I have been called");
    }
}
