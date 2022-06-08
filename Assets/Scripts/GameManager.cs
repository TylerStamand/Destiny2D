using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class GameManager : NetworkBehaviour {

    static private GameManager instance;

    SessionManager sessionManager = SessionManager.Instance;

    void Awake() {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.F)) {
            SessionManager.Instance.SavePlayerData();
        }
    }


    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        if(!IsServer) {
            enabled = false;
            return;
        }

        NetworkManager.OnClientConnectedCallback += PlayerConnected;
    }

    void PlayerConnected(ulong clientID) {
        NetworkObject playerNetObject = NetworkManager.SpawnManager.GetPlayerNetworkObject(clientID);
        PlayerControllerServer player = playerNetObject.GetComponent<PlayerControllerServer>();
        string playerID = player.GetPlayerGUID().ToString();
        PlayerData connectedPlayerData = sessionManager.GetPlayerData(playerID);
    
        //recheck
        PlayerData newPlayerData = new PlayerData();
        connectedPlayerData = newPlayerData;
        sessionManager.SetupConnectingPlayerSessionData(clientID, playerID, newPlayerData);
    
        player.SetPlayerData((PlayerData)connectedPlayerData);
        
    }



}
