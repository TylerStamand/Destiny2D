using System.Collections.Generic;
using UnityEngine;


//This class needs a lot of work
public class SessionManager
{

    SessionManager() {}

    public static SessionManager Instance => instance ??= new SessionManager();

    static SessionManager instance;

    Dictionary<string, PlayerSessionData> clientData = new Dictionary<string, PlayerSessionData>();
    Dictionary<ulong, string> ClientIDToPlayerId = new Dictionary<ulong, string>();

    public void DisconnectClient(ulong clientId) {
        // Mark client as disconnected, but keep their data so they can reconnect.
        if (ClientIDToPlayerId.TryGetValue(clientId, out var playerId)) {
            if (clientData.TryGetValue(playerId, out PlayerSessionData playerData)) {
                var data = clientData[playerId];
                data.IsConnected = false;
                clientData[playerId] = data;
            }
        }
    }


    public void SetupConnectingPlayerSessionData(ulong clientId, string playerId) {
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

        PlayerSessionData sessionPlayerData;

        // Reconnecting. Give data from old player to new player
        if (isReconnecting) {
            // Update player session data
            sessionPlayerData = clientData[playerId];
        }

        else {
            sessionPlayerData = new PlayerSessionData();

        }

        sessionPlayerData.ClientID = clientId;
        sessionPlayerData.IsConnected = true;
        sessionPlayerData.PlayerID = playerId;
        
        //Populate our dictionaries with the SessionPlayerData
        
        clientData[playerId] = sessionPlayerData;
        ClientIDToPlayerId[clientId] = playerId;


        
    }

    public bool IsDuplicateConnection(string playerId) {
        return clientData.ContainsKey(playerId) && clientData[playerId].IsConnected;
    }

    public PlayerSessionData GetPlayerData(string playerID) {
        clientData.TryGetValue(playerID, out PlayerSessionData playerData);
        return playerData;
    }

    
}
