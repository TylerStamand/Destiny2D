using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;

[System.Serializable]
public class PlayerSessionData : ISessionPlayerData {

    private string playerID;
    private bool isConnected;
    private ulong clientID;

    public string PlayerID {get => playerID; set => playerID = value;}

    public bool IsConnected { get => isConnected; set => isConnected = value; }
    public ulong ClientID { get => clientID; set => clientID = value; }

    public void Reinitialize() {
        Debug.Log($"Reinitialized on {ClientID}");
    }



  
}
