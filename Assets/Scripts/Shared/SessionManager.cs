using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager
{

    SessionManager() {

        //Come back to this. Initialization probably shouldn't happen here
        SaveSystem.Init();


        List<PlayerData> playerData = SaveSystem.LoadPlayerData();
        if(playerData != null) {
            foreach(PlayerData player in playerData) {
                clientData[player.PlayerID.ToString()] = player;
            }
        }
    }

    public static SessionManager Instance => instance ??= new SessionManager();

    static SessionManager instance;

    Dictionary<string, PlayerData> clientData = new Dictionary<string, PlayerData>();
    Dictionary<ulong, string> ClientIDToPlayerId = new Dictionary<ulong, string>();

    public void DisconnectClient(ulong clientId) {
        // Mark client as disconnected, but keep their data so they can reconnect.
        if (ClientIDToPlayerId.TryGetValue(clientId, out var playerId)) {
            if (clientData.TryGetValue(playerId, out PlayerData playerData)) {
                var data = clientData[playerId];
                data.IsConnected = false;
                clientData[playerId] = data;
            }
        }
    }


    public void SetupConnectingPlayerSessionData(ulong clientId, string playerId, PlayerData sessionPlayerData) {
        var isReconnecting = false;

        // Test for duplicate connection
        if (IsDuplicateConnection(playerId)) {
            Debug.LogError($"Player ID {playerId} already exists. This is a duplicate connection. Rejecting this session data.");
            return;
        }

        // If another client exists with the same playerId
        if (clientData.ContainsKey(playerId)) {
            if (!clientData[playerId].IsConnected) {
                // If this connecting client has the same player Id as a disconnected client, this is a reconnection.
                isReconnecting = true;
            }

        }

        // Reconnecting. Give data from old player to new player
        if (isReconnecting) {
            // Update player session data
            sessionPlayerData = clientData[playerId];
            sessionPlayerData.ClientID = clientId;
            sessionPlayerData.IsConnected = true;
        }

        //Populate our dictionaries with the SessionPlayerData
        
        clientData[playerId] = sessionPlayerData;
        ClientIDToPlayerId[clientId] = playerId;
    }

    public bool IsDuplicateConnection(string playerId) {
        return clientData.ContainsKey(playerId) && clientData[playerId].IsConnected;
    }

    public PlayerData GetPlayerData(string playerID) {
        return clientData[playerID];
    }

    public void SavePlayerData() {
        List<PlayerData> players = new List<PlayerData>();
        foreach(PlayerData playerData in clientData.Values) {
            players.Add(playerData);
        }
        SaveSystem.SavePlayerData(players);
    }
}
