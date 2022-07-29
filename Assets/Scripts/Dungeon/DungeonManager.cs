using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using UnityEngine.SceneManagement;

public class DungeonManager : NetworkBehaviour {

    DungeonGenerator currentDungeon;
    int currentDungeonSeed;
    int playersLoadedIntoDungeon;

    public override void OnNetworkSpawn() {
        if(!IsServer) return;

        InitializeDungeon(); 
    }

    void InitializeDungeon() {
        if (!IsServer) return;

        Debug.Log("Initializing Dungeon");
        playersLoadedIntoDungeon = 0;
        currentDungeon = FindObjectOfType<DungeonGenerator>();
        currentDungeon.OnStairsEntered += SwitchDungeon;

        currentDungeonSeed = DateTime.Now.Second;
        currentDungeon.GenerateMap(currentDungeonSeed);
        EnemySpawnManager.Instantiate(currentDungeon.Rooms);

    }

    void SwitchDungeon() {
        if(!IsServer) return;

        currentDungeonSeed = DateTime.Now.Second;
        currentDungeon.GenerateMap(currentDungeonSeed);
        List<Vector2> spawnPositions = currentDungeon.GetSpawnPositions();
        List<PlayerControllerServer> players = FindObjectsOfType<PlayerControllerServer>().ToList();
        foreach (PlayerControllerServer player in players) {
            Destroy(player.gameObject);
        }

        playersLoadedIntoDungeon = 0;

        List<ConnectedPlayer> connectedPlayers = GameManager.Instance.ConnectedPlayers; 
        for (int i = 0; i < connectedPlayers.Count; i++) {
            LoadPlayerIntoDungeon(connectedPlayers[i].GetComponent<NetworkObject>().OwnerClientId);

        }

    }

    public void LoadPlayerIntoDungeon(ulong clientID) {
        if(!IsServer) return;
        if (currentDungeon == null) InitializeDungeon();

        LoadDungeonLevelClientRpc(currentDungeonSeed, new ClientRpcParams() { Send = new ClientRpcSendParams() { TargetClientIds = new[] { clientID } } });
        GameManager.Instance.SpawnPlayer(clientID, currentDungeon.GetSpawnPositions()[playersLoadedIntoDungeon]);

        playersLoadedIntoDungeon++;

    }


    [ClientRpc]
    void LoadDungeonLevelClientRpc(int seed, ClientRpcParams sendParams) {
        DungeonGenerator dungeon = FindObjectOfType<DungeonGenerator>();
        dungeon.GenerateMap(seed);
    }



}
