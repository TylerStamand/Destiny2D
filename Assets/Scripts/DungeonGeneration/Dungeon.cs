using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Plankton;
using System;


public class Dungeon : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tile floorTile;
    [SerializeField] Tile wallTile;
    [SerializeField] int seed;
    [SerializeField] int width;
    [SerializeField] int height;
   
    [Header ("Use Values Between 0 and 1")]
    [SerializeField] MinMaxFloat ShrinkPercentage;

    //[SerializeField] int deathLimit;
    //[SerializeField] int birthLimit;
    //[Range(0,1)]
    //[SerializeField] float initialChance;
    [SerializeField] int numberOfSteps;

    System.Random random;

    PlanktonMesh planktonMesh;

    [ContextMenu("GenerateMap")]
    void GenerateMap() {

        random = new System.Random(seed);
        
        if(ShrinkPercentage.MaxValue > 1 || ShrinkPercentage.MinValue > 1 || ShrinkPercentage.MinValue < 0 || ShrinkPercentage.MaxValue < 0)
        {
            Debug.LogError("Shrink percentage out of bounds");
            return;
        }

        Debug.Log("Generating Map");
        tilemap.ClearAllTiles();

        Room startRoom = new Room(-width/2, -height/2, width, height);
        List<Room> rooms = RoomPartitioner.PartitionRooms(startRoom, seed, numberOfSteps, out planktonMesh);

        Debug.Log("Faces: " + planktonMesh.Faces.Count);
        Debug.Log("Rooms: " + rooms.Count);
        
        //Shrinks down rooms
       /* foreach (Room room in rooms) {
            float widthShrinkPercent = (float)random.NextDouble() * (ShrinkPercentage.MaxValue - ShrinkPercentage.MinValue) + ShrinkPercentage.MinValue;
            float leftShrinkPercent = (float)random.NextDouble() * widthShrinkPercent;
            float rightShrinkPercent = widthShrinkPercent - leftShrinkPercent;
            
            int leftShrink = (int)(room.Width * leftShrinkPercent);
            int rightShrink = (int)(room.Width * rightShrinkPercent);


            float heightShrinkPercent= (float)random.NextDouble() * (ShrinkPercentage.MaxValue - ShrinkPercentage.MinValue) + ShrinkPercentage.MinValue;
            float topShrinkPercent = (float)random.NextDouble() * heightShrinkPercent;
            float bottomShrinkPercent= heightShrinkPercent - topShrinkPercent;
            
            int topShrink = (int)(room.Height * topShrinkPercent);
            int bottomShrink = (int)(room.Height * bottomShrinkPercent);

            room.XPosition += leftShrink;
            room.YPosition += bottomShrink;
            room.Width -= rightShrink + leftShrink;
            room.Height -= topShrink + bottomShrink;
        }
       */

        //Draw Rooms
        foreach(Room room in rooms) {
            wallTile.color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), 1);
            for (int i = room.XPosition; i < room.XPosition +  room.Width; i++) {
                for (int j = room.YPosition; j < room.YPosition + room.Height; j++) {
                    tilemap.SetTile(new Vector3Int(i, j), wallTile);
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

    void OnDrawGizmos()
    {
        if (planktonMesh == null) return; 
        
        for (int i = 0; i < planktonMesh.Faces.Count; i++) {

            Vector3 oldPoint = Vector3.zero;
            Vector3 newPoint = Vector3.zero;
            int[] vertexIndexs = planktonMesh.Faces.GetFaceVertices(i);
            for (int j = 0; j < vertexIndexs.Length; j++) {
                oldPoint = newPoint;
                
                PlanktonVertex vertex = planktonMesh.Vertices[i];

                newPoint = new Vector3(vertex.X, vertex.Y);
                if (oldPoint != Vector3.zero && newPoint != Vector3.zero)
                    {
                    Gizmos.DrawRay(oldPoint, newPoint);

                }
            }
       
        }

       

    }


}


