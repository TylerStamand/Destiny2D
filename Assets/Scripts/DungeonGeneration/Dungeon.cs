using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using System.Linq;
using System;


public class Dungeon : MonoBehaviour {
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tile floorTile;
    [SerializeField] Tile wallTile;
    [SerializeField] int seed;
    [SerializeField] int width;
    [SerializeField] int height;

    [SerializeField] int numberOfRoomSplits;

    [Header("Automaton Generation Options")]
    [Header("Use Values Between 0 and 1")]
    [SerializeField] MinMaxFloat ShrinkPercentage;
    [SerializeField] int numberOfSteps;
    [SerializeField] int deathLimit;
    [SerializeField] int birthLimit;
    [Range(0,1)]
    [SerializeField] float initialChance;
    [SerializeField] int distanceFromRoomBoundaries;
    [SerializeField] int distanceFromPathBoundaries;

    System.Random random;
    List<Room> rooms;
    List<Edge> edges;

    [ContextMenu("GenerateMap")]
    void GenerateMap() {

        random = new System.Random(seed);

        if (ShrinkPercentage.MaxValue > 1 || ShrinkPercentage.MinValue > 1 || ShrinkPercentage.MinValue < 0 || ShrinkPercentage.MaxValue < 0) {
            Debug.LogError("Shrink percentage out of bounds");
            return;
        }

        Debug.Log("Generating Map");
        tilemap.ClearAllTiles();

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
                if (generatedMap[i, j]) {
                    tilemap.SetTile(new Vector3Int(i, j), floorTile);
                }
                else {
                    tilemap.SetTile(new Vector3Int(i, j), wallTile);
                }
            }
        }

        // //Draw Rooms
        // foreach (Room room in rooms) {

        //     wallTile.color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), 1);
        //     for (int i = room.XPosition; i < room.XPosition + room.Width; i++) {
        //         for (int j = room.YPosition; j < room.YPosition + room.Height; j++) {
        //             tilemap.SetTile(new Vector3Int(i, j), wallTile);
        //         }
        //     }
        // }

       




    }

    void OnDrawGizmos() {


        if(rooms != null) {
            foreach(Room room in rooms) {
                List<Room> adjacentRooms = room.GetAdjacentRooms();
                foreach(Room adjacentRoom in adjacentRooms) {

                    Gizmos.color = Color.white;  
                    Gizmos.DrawLine(room.Center, adjacentRoom.Center);
                    
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
                Gizmos.DrawLine(edge.From.Room.Center, edge.To.Room.Center);
            }
        }





    }


}


