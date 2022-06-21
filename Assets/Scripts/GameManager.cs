using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Linq;


//Figure out setting the host, then loading data for the host. Then when someone joins, look at save data for their stuff


public class GameManager : NetworkBehaviour {

    [SerializeField] Enemy defaultEnemyPrefab;

    public static GameManager Instance;

//    SessionManager sessionManager;
    
    public bool playerIDSet = false;


    void Awake() {
        if(Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }


        //Come back to this. Initialization probably shouldn't happen here
        SaveSystem.Init();
  //      sessionManager = SessionManager.Instance;
    }

    void Update() {
        if(!IsSpawned) return;
        if(Input.GetKeyDown(KeyCode.F)) {
            SavePlayerData();
        }
        
    }

    string playerID = "0";

    void OnGUI() {

        if(!playerIDSet) {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            playerID = GUILayout.TextField(playerID);
            if (GUILayout.Button("Submit")) {
                if (TryGetComponent<ServerGUI>(out ServerGUI serverGUI)) {
                    serverGUI.enabled = true;
                }
                playerIDSet = true;
                PlayerPrefs.SetString("PlayerID", playerID);
            }
            GUILayout.EndArea();
        }

        if(IsSpawned)  {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (GUILayout.Button("Spawn Enemy")) {
                Vector3 spawn = SpawnManager.Instance.GetSpawnLocation().transform.position;
                Enemy enemy = Instantiate(defaultEnemyPrefab, spawn, Quaternion.identity);
                enemy.NetworkObject.Spawn();
            }
            GUILayout.EndArea();
        }
       
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
        string playerID = player.GetPlayerID();
        //sessionManager.SetupConnectingPlayerSessionData(clientID, playerID);
        PlayerSaveData saveData = SaveSystem.LoadPlayerData(playerID);

        if(saveData != null) {
            player.SetSaveDataServer(saveData);       
        }

    }

    void SavePlayerData() {
        List<PlayerControllerServer> players = GameObject.FindObjectsOfType<PlayerControllerServer>().ToList();
        List<PlayerSaveData> playerSaveDataList = new List<PlayerSaveData>();
        foreach (PlayerControllerServer player in players) {
            playerSaveDataList.Add(player.GetSaveDataServer());
        }
        SaveSystem.SavePlayerData(playerSaveDataList);
    }

}
