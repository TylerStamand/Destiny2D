using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class GameManager : NetworkBehaviour {

    [SerializeField] Enemy defaultEnemyPrefab;

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


    void OnGUI() {
        if(!IsSpawned) return;
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (GUILayout.Button("Spawn Enemy")) {
            Vector3 spawn = SpawnManager.Instance.GetSpawnLocation().transform.position;
            Enemy enemy = Instantiate(defaultEnemyPrefab, spawn, Quaternion.identity);
            enemy.NetworkObject.Spawn();
        }
        GUILayout.EndArea();
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        if(!IsServer) {
            enabled = false;
            return;
        }

        if(IsServer && IsClient) {
            PlayerConnected(NetworkObject.OwnerClientId);
        }

        NetworkManager.OnClientConnectedCallback += PlayerConnected;
    }

    void PlayerConnected(ulong clientID) {
        Debug.Log("Player Connected");
        NetworkObject playerNetObject = NetworkManager.SpawnManager.GetPlayerNetworkObject(clientID);
        PlayerControllerServer player = playerNetObject.GetComponent<PlayerControllerServer>();
        Guid playerID = player.GetPlayerGUID();
        sessionManager.SetupConnectingPlayerSessionData(clientID, playerID);
        PlayerData playerData = sessionManager.GetPlayerData(playerID);
        Debug.Log(playerData.PlayerID);
        player.SetPlayerData(playerData);
    }



}
