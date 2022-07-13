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

    [Header("Use Values Between 0 and 1")]
    [SerializeField] MinMaxFloat ShrinkPercentage;

    //[SerializeField] int deathLimit;
    //[SerializeField] int birthLimit;
    //[Range(0,1)]
    //[SerializeField] float initialChance;
    [SerializeField] int numberOfSteps;

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

        Room startRoom = new Room(-width / 2, -height / 2, width, height);

        rooms = RoomPartitioner.PartitionRooms(startRoom, seed, numberOfSteps);

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

        //Draw Rooms
        foreach (Room room in rooms) {

            wallTile.color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), 1);
            for (int i = room.XPosition; i < room.XPosition + room.Width; i++) {
                for (int j = room.YPosition; j < room.YPosition + room.Height; j++) {
                    tilemap.SetTile(new Vector3Int(i, j), wallTile);
                }
            }
        }

        foreach(Edge edge in edges) {
            wallTile.color = Color.red;


            int currentX = (int)edge.From.Room.Center.x;
            int currentY = (int)edge.From.Room.Center.y;
            int endX = (int) edge.To.Room.Center.x;
            int endY = (int) edge.To.Room.Center.y;

            int xIncrement = currentX < endX ? 1 : -1;
            int yIncrement = currentY < endY ? 1 : -1;
            


            if(random.NextDouble() > .5) {
                
                while(currentX != endX) {
                    currentX += xIncrement;
                    tilemap.SetTile(new Vector3Int(currentX, currentY), wallTile);    
                }
                while(currentY != endY) {
                    currentY += yIncrement;
                    tilemap.SetTile(new Vector3Int(currentX, currentY), wallTile);
                }
                

            }
            else {
                while (currentY != endY) {
                    currentY += yIncrement;
                    tilemap.SetTile(new Vector3Int(currentX, currentY), wallTile);
                }
                while (currentX != endX) {
                    currentX += xIncrement;
                    tilemap.SetTile(new Vector3Int(currentX, currentY), wallTile);
                }

            }         
        }





        // GenerationOptions options = new GenerationOptions {
        //     Seed = seed,
        //     Width = width,
        //     Height = height,
        //     DeathLimit = deathLimit,
        //     BirthLimit = birthLimit,
        //     NumberOfSteps = NumberOfSteps,
        //     InitialChance01 = initialChance
        // };

        // bool[,] generatedMap = Generator.GenerateMap(options);


        // for (int i = 0; i < generatedMap.GetLength(0); i++) {
        //     for (int j = 0; j < generatedMap.GetLength(1); j++) {
        //         if (generatedMap[i, j]) {
        //             tilemap.SetTile(new Vector3Int(i, j), floorTile);
        //         }
        //         else {
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

                Gizmos.color = Color.clear;
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


