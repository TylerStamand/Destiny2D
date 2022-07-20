using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using System.Linq;
using System;


//Place Treasure rooms with monsters

public class DungeonGenerator : MonoBehaviour {
    [SerializeField] Tilemap floorTileMap;
    [SerializeField] Tilemap wallTileMap;
    [SerializeField] Tilemap nextFloorTileMap;
    [SerializeField] Tile floorTile;
    [SerializeField] Tile wallTile;
    [SerializeField] Tile SpawnTile;
    [SerializeField] Tile LeaveTile;

    [SerializeField] int seed;
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] int numberOfRoomSplits;
    [Range(0,1)]
    [SerializeField] float finishingRoomPercentile;

    [Header("Use Values Between 0 and 1")]
    [Header("Automaton Generation Options")]
    [SerializeField] MinMaxFloat ShrinkPercentage;
    [SerializeField] int numberOfSteps;
    [SerializeField] int deathLimit;
    [SerializeField] int birthLimit;
    [Range(0,1)]
    [SerializeField] float initialChance;
    [SerializeField] int distanceFromRoomBoundaries;
    [SerializeField] int distanceFromPathBoundaries;

    public Action OnStairsEntered;


    System.Random random;
    List<Room> rooms;
    List<Edge> edges;

    Room spawnRoom;
   




    void Awake() {
        nextFloorTileMap.GetComponent<TriggerEventExposer>().OnTriggerEnter += (Collider2D collider) => OnStairsEntered?.Invoke();
    }

    [ContextMenu("GenerateMap")]
    public void GenerateMap(int seed) {

        random = new System.Random(seed);

        if (ShrinkPercentage.MaxValue > 1 || ShrinkPercentage.MinValue > 1 || ShrinkPercentage.MinValue < 0 || ShrinkPercentage.MaxValue < 0) {
            Debug.LogError("Shrink percentage out of bounds");
            return;
        }

        Debug.Log("Generating Map");
        wallTileMap.ClearAllTiles();
        floorTileMap.ClearAllTiles();
        nextFloorTileMap.ClearAllTiles();
        Room startRoom = new Room(0, 0, width, height);

        rooms = RoomPartitioner.PartitionRooms(startRoom, seed, numberOfRoomSplits);

        //Shrinks down rooms
        foreach (Room room in rooms) {
            //float widthShrinkPercent = (float)random.NextDouble() * (ShrinkPercentage.MaxValue - ShrinkPercentage.MinValue) + ShrinkPercentage.MinValue;
            float leftShrinkPercent = (float)random.NextDouble() * (ShrinkPercentage.MaxValue - ShrinkPercentage.MinValue) + ShrinkPercentage.MinValue;
            float rightShrinkPercent = (float)random.NextDouble() * (ShrinkPercentage.MaxValue - ShrinkPercentage.MinValue) + ShrinkPercentage.MinValue;

            int leftShrink = (int)(room.Width * leftShrinkPercent);
            int rightShrink = (int)(room.Width * rightShrinkPercent);

           // float heightShrinkPercent = (float)random.NextDouble() * (ShrinkPercentage.MaxValue - ShrinkPercentage.MinValue) + ShrinkPercentage.MinValue;
            float topShrinkPercent = (float)random.NextDouble() * (ShrinkPercentage.MaxValue - ShrinkPercentage.MinValue) + ShrinkPercentage.MinValue;
            float bottomShrinkPercent = (float)random.NextDouble() * (ShrinkPercentage.MaxValue - ShrinkPercentage.MinValue) + ShrinkPercentage.MinValue;

            int topShrink = (int)(room.Height * topShrinkPercent);
            int bottomShrink = (int)(room.Height * bottomShrinkPercent);

            room.XPosition += leftShrink;
            room.YPosition += bottomShrink;
            room.Width -= rightShrink + leftShrink;
            room.Height -= topShrink + bottomShrink;
        }

        edges = EdgeSelector.GetEdges(rooms);




        GenerationOptions options = new GenerationOptions {
            Seed = seed,
            Width = width,
            Height = height,
            DeathLimit = deathLimit,
            BirthLimit = birthLimit,
            NumberOfSteps = numberOfSteps,
            InitialChance01 = initialChance,
            DistanceFromRoomBoundaries = distanceFromRoomBoundaries,
            DistanceFromPathBoundaries = distanceFromPathBoundaries
        };

        bool[,] generatedMap = Automaton.GenerateMap(options, rooms, edges);


        for (int i = 0; i < generatedMap.GetLength(0); i++) {
            for (int j = 0; j < generatedMap.GetLength(1); j++) {
                if (!generatedMap[i, j]) {
                    wallTileMap.SetTile(new Vector3Int(i, j), wallTile);
                }
                floorTileMap.SetTile(new Vector3Int(i, j), floorTile);
                
            }
        }



        SetStartEndRooms();
        



    }

    public List<Vector2> GetSpawnPositions() {
        if(spawnRoom == null) {
            return null;
        }

        List<Vector2> spawnPositions = new List<Vector2>();
        Vector2 spawnRoomCenter = spawnRoom.Center;
        spawnPositions.Add(new Vector2(spawnRoomCenter.x + 1, spawnRoomCenter.y));
        spawnPositions.Add(new Vector2(spawnRoomCenter.x - 1, spawnRoomCenter.y));
        spawnPositions.Add(new Vector2(spawnRoomCenter.x, spawnRoomCenter.y + 1));
        spawnPositions.Add(new Vector2(spawnRoomCenter.x, spawnRoomCenter.y - 1));
        return spawnPositions;
    }

    void SetStartEndRooms() {
        int startingRoomIndex = random.Next(rooms.Count - 1);
        spawnRoom = rooms[startingRoomIndex];

        List<Room> prospectEndRooms = rooms;
        prospectEndRooms.RemoveAt(startingRoomIndex);

        prospectEndRooms.OrderBy(room => spawnRoom.Center.sqrMagnitude - room.Center.sqrMagnitude);

        int finishingRoomIndex = random.Next((int)(rooms.Count * finishingRoomPercentile), rooms.Count - 1);
        Room finishingRoom = rooms[finishingRoomIndex];

        //For Debugging purposes
        floorTileMap.SetTile((Vector3Int)spawnRoom.Center, SpawnTile);
        
        nextFloorTileMap.SetTile((Vector3Int)finishingRoom.Center, LeaveTile);

    }

    void OnDrawGizmos() {


        if(rooms != null) {
            foreach(Room room in rooms) {
                List<Room> adjacentRooms = room.GetAdjacentRooms();
                foreach(Room adjacentRoom in adjacentRooms) {

                    Gizmos.color = Color.white;  
                    Gizmos.DrawLine(((Vector3Int)room.Center), (Vector3Int)adjacentRoom.Center);
                    
                }

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(new Vector2(room.OriginalXPosition, room.OriginalYPosition), new Vector2(room.OriginalXPosition + room.OriginalWidth, room.OriginalYPosition));
                Gizmos.DrawLine(new Vector2(room.OriginalXPosition, room.OriginalYPosition), new Vector2(room.OriginalXPosition, room.OriginalYPosition + room.OriginalHeight));
                Gizmos.DrawLine(new Vector2(room.OriginalXPosition + room.OriginalWidth, room.OriginalYPosition + room.OriginalHeight), new Vector2(room.OriginalXPosition, room.OriginalYPosition + room.OriginalHeight));
                Gizmos.DrawLine(new Vector2(room.OriginalXPosition + room.OriginalWidth, room.OriginalYPosition + room.OriginalHeight), new Vector2(room.OriginalXPosition + room.OriginalWidth, room.OriginalYPosition));

            }

           
        }

        if(edges != null) {
            Gizmos.color = Color.red;
            foreach(Edge edge in edges) {
                Gizmos.DrawLine((Vector3Int)edge.From.Room.Center, (Vector3Int)edge.To.Room.Center);
            }
        }





    }


}


