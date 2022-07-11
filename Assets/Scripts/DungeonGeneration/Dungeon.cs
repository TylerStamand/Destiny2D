using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Plankton;
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
    PlanktonMesh planktonMesh;

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
        rooms = RoomPartitioner.PartitionRooms(startRoom, seed, numberOfSteps, out planktonMesh);

        Debug.Log("Faces: " + planktonMesh.Faces.Count);
        Debug.Log("Rooms: " + rooms.Count);

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


        //Draw Rooms
        foreach (Room room in rooms) {

            wallTile.color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), 1);
            for (int i = room.XPosition; i < room.XPosition + room.Width; i++) {
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

    void OnDrawGizmos() {


        if(rooms != null) {
            foreach(Room room in rooms) {
                List<Room> adjacentRooms = room.GetAdjacentRooms();
                foreach(Room adjacentRoom in adjacentRooms) {

                    Gizmos.DrawLine(room.Center, adjacentRoom.Center);
                }
            }
        }




        // if (planktonMesh == null) return;

        // Gizmos.color = Color.white;
        // for (int i = 0; i < planktonMesh.Faces.Count; i++) {

        //     Vector3 oldPoint = Vector3.zero;
        //     Vector3 newPoint = Vector3.zero;
        //     int[] vertexIndexs = planktonMesh.Faces.GetFaceVertices(i);
        //     //Debug.Log("I: " + i);
        //     for (int j = 0; j < vertexIndexs.Length + 1; j++) {
        //        // Debug.Log($"X: {planktonMesh.Vertices[j].X} + Y: {planktonMesh.Vertices[j].Y}");
                
        //         oldPoint = newPoint;

        //         PlanktonVertex vertex = planktonMesh.Vertices[vertexIndexs[j % vertexIndexs.Length]];

        //         newPoint = new Vector3(vertex.X, vertex.Y);
        //         if (j != 0) {
        //         //    Debug.Log($"From: {oldPoint} To: {newPoint}");
        //             Gizmos.DrawLine(oldPoint, newPoint);

        //         }
        //     }

        // }

        // Gizmos.color = Color.red;

        // Vector3 currentFace;
        // Vector3 adjacentFace;
        // for(int i = 0; i < planktonMesh.Faces.Count; i++) {
        //     PlanktonXYZ xyz =  planktonMesh.Faces.GetFaceCenter(i);
        //     currentFace = new Vector3(xyz.X, xyz.Y, 0);

        //     int[] halfEdgeIndexs = planktonMesh.Faces.GetHalfedges(i);
        //    // Debug.Log(halfEdgeIndexs.Length);
        //     for(int j = 0; j < halfEdgeIndexs.Length; j++) {
        //        // Debug.Log(halfEdgeIndexs[j]);
        //         int pairHalfEdge = planktonMesh.Halfedges.GetPairHalfedge(halfEdgeIndexs[j]);
        //        // Debug.Log(pairHalfEdge);
        //         int adjacentFaceIndex = planktonMesh.Halfedges[pairHalfEdge].AdjacentFace;
        //       //  Debug.Log(adjacentFaceIndex);
        //         if(adjacentFaceIndex != -1) {
        //             xyz = planktonMesh.Faces.GetFaceCenter(adjacentFaceIndex);
        //             adjacentFace = new Vector3(xyz.X, xyz.Y, 0);
        //             Gizmos.DrawLine(currentFace, adjacentFace);
        //         }
        //     }

       
        // }



    }


}


