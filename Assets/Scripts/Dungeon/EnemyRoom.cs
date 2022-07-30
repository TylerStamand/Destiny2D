using UnityEngine;
using System;


public class EnemyRoom {
    static readonly int EnemyCapDivisor = 10;

    public Action<int, EnemyRoom> OnPlayerCountChange;
    public Action<int, EnemyRoom> OnEnemyCountChange;

    public int EnemiesInRoom;

    public int PlayersInRoom;

    public int EnemyCap;

    public Room Room;

    TriggerEventExposer RoomTrigger;

    public EnemyRoom(Room room) {
        EnemiesInRoom = 0;
        EnemyCap = room.Area / EnemyCapDivisor;
        Room = room;
        GenerateRoomTrigger();
    }

    void GenerateRoomTrigger() {
        GameObject roomTriggerGO = new GameObject("Room Trigger", typeof(BoxCollider2D), typeof(TriggerEventExposer) );
        roomTriggerGO.GetComponent<BoxCollider2D>().isTrigger = true;
        //Rooms sometimes off center due to rounding
        roomTriggerGO.transform.position = ((Vector3Int)Room.Center);
        roomTriggerGO.transform.localScale = new Vector3(Room.Width, Room.Height, 0);

        RoomTrigger = roomTriggerGO.GetComponent<TriggerEventExposer>();

        RoomTrigger.OnTriggerEnter += HandleRoomEntered;
        RoomTrigger.OnTriggerExit += HandleRoomExit;

    }

    void HandleRoomEntered(Collider2D collider2D, GameObject src) {
        if(collider2D.GetComponent<Enemy>() != null) {
            EnemiesInRoom++;
            Debug.Log("Enemy Entered Room");
            OnEnemyCountChange?.Invoke(EnemiesInRoom, this);
        }
        else if(collider2D.GetComponent<PlayerControllerServer>() != null) {
            PlayersInRoom++;
            Debug.Log("Player Entered Room");
            OnPlayerCountChange?.Invoke(PlayersInRoom, this);
        }
    }

    void HandleRoomExit(Collider2D collider2D, GameObject src) {
        if(collider2D.GetComponent<Enemy>() != null) {
            EnemiesInRoom--;
            OnEnemyCountChange?.Invoke(EnemiesInRoom, this);
        }
        else if (collider2D.GetComponent<PlayerControllerServer>() != null) {
            PlayersInRoom--;
            OnPlayerCountChange?.Invoke(PlayersInRoom, this);
        }
    }

}