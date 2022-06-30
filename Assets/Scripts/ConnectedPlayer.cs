using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ConnectedPlayer : NetworkBehaviour
{
    public NetworkVariable<ulong> ClientID = new NetworkVariable<ulong>();
    public NetworkVariable<ulong> PlayerUnitObjectNetID = new NetworkVariable<ulong>();


    public string GetPlayerID() {

        return PlayerPrefs.GetString("PlayerID");
    }
}
