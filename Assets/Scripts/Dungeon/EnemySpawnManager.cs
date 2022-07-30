using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System;

public class EnemySpawnManager : NetworkBehaviour {

    [SerializeField] Enemy defaultEnemyPrefab;

    static float spawnProbability = 0.005f; 
    
    //Theres probably a more intelligent way of setting the limit than a hard value
    static int maxEnemies = 8;

    public static EnemySpawnManager Instantiate(List<Room> roomList) {
        
        
        EnemySpawnManager currentSpawnManager = Instantiate(ResourceManager.Instance.EnemySpawnManagerPrefab).GetComponent<EnemySpawnManager>();
        currentSpawnManager.GetComponent<NetworkObject>().Spawn();
        currentSpawnManager.Initialize(roomList);
        return currentSpawnManager;
    }
    
    Dictionary<Room, EnemyRoom> RoomToEnemyRoomMap;

    List<EnemyRoom> enemyRooms;
    List<EnemyRoom> currentPlayerRooms;

    List<Enemy> spawnedEnemies;

    void Initialize(List<Room> rooms) {
        currentPlayerRooms = new List<EnemyRoom>();
        enemyRooms = new List<EnemyRoom>();
        RoomToEnemyRoomMap = new Dictionary<Room, EnemyRoom>();
        spawnedEnemies = new List<Enemy>();

        foreach(Room room in rooms) {
            EnemyRoom enemyRoom = new EnemyRoom(room);
            RoomToEnemyRoomMap.Add(room, enemyRoom);
            enemyRoom.OnPlayerCountChange += HandlePlayerCountChange;
            enemyRooms.Add(enemyRoom);
            
        }
    }

    public override void OnNetworkSpawn() {
        if(!IsServer) {
            enabled = false;
        }
    }

    //Done in fixed update so that the checks per second are consistent 
    void FixedUpdate() {
        spawnedEnemies.RemoveAll(e => e == null);
        foreach(EnemyRoom enemyRoom in currentPlayerRooms) {
            Room room = enemyRoom.Room;
            foreach(Room adjRoom in room.GetAdjacentRooms()) {
                EnemyRoom adjEnemyRoom = RoomToEnemyRoomMap[adjRoom];

                if(adjEnemyRoom.EnemiesInRoom < adjEnemyRoom.EnemyCap && adjEnemyRoom.PlayersInRoom == 0 && spawnedEnemies.Count < maxEnemies) {
                    if(UnityEngine.Random.value <= spawnProbability)
                        SpawnEnemy(adjRoom);
                }
            }
        }
    }

    void HandlePlayerCountChange(int playerCount, EnemyRoom room) {
        if(playerCount == 0) {
            currentPlayerRooms.Remove(room);
        }
        else {
            if(!currentPlayerRooms.Contains(room)) {
                currentPlayerRooms.Add(room);
            }
        }
    }

    Enemy SpawnEnemy(Room room) {
        int xPosition = UnityEngine.Random.Range(room.XPosition, room.XPosition + room.Width);
        int yPosition = UnityEngine.Random.Range(room.YPosition, room.YPosition + room.Height);
        Vector2 spawnPosition = new Vector2(xPosition, yPosition);
        //Get current dungeon information for enemy spawn list, for now spawn default
        Enemy enemy = Instantiate(defaultEnemyPrefab, spawnPosition, Quaternion.identity);
        enemy.NetworkObject.Spawn();
        enemy.OnDie += (Enemy enemyToRemove) => {spawnedEnemies.Remove(enemyToRemove);};
        Debug.Log($"Spawned an enemy at {xPosition}, {yPosition}");
        spawnedEnemies.Add(enemy);
        return enemy;
    }




    //Despawning when too far


}

