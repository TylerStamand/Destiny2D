using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System;

public class EnemySpawnManager : NetworkBehaviour {

    [SerializeField] RoomTrigger roomTriggerPrefab;

    static EnemySpawnManager currentSpawnManager;
    public static void Instantiate(List<Room> roomList) {
        
        currentSpawnManager = Instantiate(ResourceManager.Instance.EnemySpawnManagerPrefab).GetComponent<EnemySpawnManager>();
        currentSpawnManager.GetComponent<NetworkObject>().Spawn();
        currentSpawnManager.Initialize(roomList);
    }
    

    List<Room> rooms;
    List<GameObject> roomTriggers;

    Room currentPlayerRoom;
    
    void Initialize(List<Room> rooms) {
        this.rooms = rooms;

        int i = 0;
        foreach(Room room in rooms) {
            RoomTrigger roomTrigger = Instantiate(roomTriggerPrefab);
            roomTrigger.name = $"roomTrigger: {i++}";
            //Rooms sometimes off center due to rounding
            roomTrigger.transform.position = ((Vector3Int)room.Center);
            roomTrigger.transform.localScale = new Vector3(room.Width, room.Height, 0);

            roomTrigger.OnTriggerEnter += HandleRoomEntered;
            roomTrigger.room = room;

        }
    }

    void HandleRoomEntered(Collider2D obj, GameObject src) {
        currentPlayerRoom = src.GetComponent<RoomTrigger>().room;
        Debug.Log($"Current Room is now {currentPlayerRoom}");  
    }




    //Relies on DungeonManagerClass
    //Needs Instantiated from Dungeon Manager
    //Needs initialized somehow

    //Spawn Enemies in surrounding rooms, amount of enemies are determined by room area
    //Need way of checking room im in
    //Could use box triggers
    //Should not be spawning in player view
    //Despawning when too far


}

