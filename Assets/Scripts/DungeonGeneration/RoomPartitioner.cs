using System;
using System.Collections.Generic;
using Plankton;
using UnityEngine;

using Random = System.Random;
public class RoomPartitioner {

    public static List<Room> PartitionRooms(Room room, int seed,  int numberOfSteps, out PlanktonMesh mesh) {
        RoomPartitioner partitioner = new RoomPartitioner(seed, room);
        
        partitioner.Partition(ref partitioner.head, partitioner.splitHorizontal, numberOfSteps);

        mesh = partitioner.mesh;

        return partitioner.rooms;
        
    } 

    RoomNode head;
    PlanktonMesh mesh;
    Random random;

    float lowerRatio;
    float upperRatio;
    float ratioDifference;
    bool splitHorizontal;
    int minArea;

    List<Room> rooms;

    RoomPartitioner(int seed, Room room) {
       
        random = new Random(seed);
        lowerRatio = .40f;
        upperRatio = .60f;
        ratioDifference = upperRatio - lowerRatio; 
        head = new RoomNode(room);
        splitHorizontal = true;
        rooms = new List<Room>();

        mesh = new PlanktonMesh();
       
    }


    void Partition(ref RoomNode head, bool splitHorizontal, int numberOfSteps) {

        Room leftRoom;
        Room rightRoom;
        
        if(numberOfSteps <= 0) {

            PlanktonXYZ[] vertices;
            int[] vertIndexes;
            Room room = head.Room;

            vertices = new PlanktonXYZ[] {new PlanktonXYZ(room.XPosition, room.YPosition ,0), new PlanktonXYZ(room.XPosition + room.Width, room.YPosition, 0), 
            new PlanktonXYZ(room.XPosition + room.Width, room.YPosition + room.Height, 0), new PlanktonXYZ(room.XPosition, room.YPosition + room.Height, 0)};

            // foreach(PlanktonXYZ vertex in vertices) {
            //     Debug.Log(vertex.X + " " +  vertex.Y); 
            // }

            vertIndexes = mesh.Vertices.AddVertices(vertices);


            int faceIndex = mesh.Faces.AddFace(vertIndexes);

            int[] vertexIndexs = mesh.Faces.GetFaceVertices(faceIndex);
         
            rooms.Add(room);
       
            return;
        }


        float roomRatio = (float)(random.NextDouble() * ratioDifference) + lowerRatio;
        Room roomToSplit = head.Room;

    

        if(splitHorizontal) {

            //Top Room
            int leftRoomHeight = (int) (roomToSplit.Height * roomRatio);
            leftRoom = new Room(roomToSplit.XPosition, roomToSplit.YPosition, roomToSplit.Width, leftRoomHeight ); 

            //Bottom Room
            int rightRoomHeight = roomToSplit.Height - leftRoomHeight; 
            rightRoom =  new Room(roomToSplit.XPosition, roomToSplit.YPosition + leftRoomHeight, roomToSplit.Width, rightRoomHeight);

        }
        else {
            //Left Room
            int leftRoomWidth = (int)(roomToSplit.Width * roomRatio);
            leftRoom = new Room(roomToSplit.XPosition, roomToSplit.YPosition, leftRoomWidth, roomToSplit.Height);

            //Right Room
            int rightRoomWidth = roomToSplit.Width - leftRoomWidth;    
            rightRoom = new Room(roomToSplit.XPosition + leftRoomWidth, roomToSplit.YPosition, rightRoomWidth, roomToSplit.Height);
        }

        numberOfSteps--;

        RoomNode leftNode = new RoomNode(leftRoom); 
        leftNode.Head = head;
        head.Left = leftNode;
        
        RoomNode rightNode = new RoomNode(rightRoom);
        rightNode.Head = head;
        head.Right = rightNode;

        Partition(ref head.Left, !splitHorizontal, numberOfSteps);
        Partition(ref head.Right, !splitHorizontal, numberOfSteps);


        return;
    
    
    
    }


    
}