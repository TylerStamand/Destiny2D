using System;
using System.Collections.Generic;
using Plankton;
using UnityEngine;

using Random = System.Random;
public class RoomPartitioner {

    public static List<Room> PartitionRooms(Room room, int seed,  int numberOfSteps, out PlanktonMesh mesh) {
        RoomPartitioner partitioner = new RoomPartitioner(seed, room);
        
        partitioner.Partition(ref partitioner.head, partitioner.splitHorizontal, numberOfSteps, 0);

        mesh = partitioner.mesh;
        mesh.Compact();
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

        PlanktonXYZ[] vertices;
        int[] vertIndexes;

        vertices = new PlanktonXYZ[] {new PlanktonXYZ(room.XPosition, room.YPosition ,0), new PlanktonXYZ(room.XPosition + room.Width, room.YPosition, 0),
            new PlanktonXYZ(room.XPosition + room.Width, room.YPosition + room.Height, 0), new PlanktonXYZ(room.XPosition, room.YPosition + room.Height, 0)};

        vertIndexes = mesh.Vertices.AddVertices(vertices);


        int faceIndex = mesh.Faces.AddFace(vertIndexes);
       
    }


    void Partition(ref RoomNode head, bool splitHorizontal, int numberOfSteps, int faceIndex) {

        Room leftRoom;
        Room rightRoom;

        int leftFaceIndex;
        int rightFaceIndex;

        int[] vertices = mesh.Faces.GetFaceVertices(faceIndex);

        if (numberOfSteps <= 0) {

            rooms.Add(head.Room);

            return;
        }


        float roomRatio = (float)(random.NextDouble() * ratioDifference) + lowerRatio;
        Room roomToSplit = head.Room;

        // foreach (int vertex in vertices) {
        //     Debug.Log($"X: {mesh.Vertices[vertex].X} Y: {mesh.Vertices[vertex].Y}");
        // }


        if (splitHorizontal) {

            //Bottom Room
            int leftRoomHeight = (int)(roomToSplit.Height * roomRatio);
            leftRoom = new Room(roomToSplit.XPosition, roomToSplit.YPosition, roomToSplit.Width, leftRoomHeight);


            //Top Room
            int rightRoomHeight = roomToSplit.Height - leftRoomHeight;
            rightRoom = new Room(roomToSplit.XPosition, roomToSplit.YPosition + leftRoomHeight, roomToSplit.Width, rightRoomHeight);



            PlanktonXYZ rightVertex = new PlanktonXYZ(roomToSplit.XPosition + roomToSplit.Width, roomToSplit.YPosition + leftRoomHeight, 0);
            PlanktonXYZ leftVertex = new PlanktonXYZ(roomToSplit.XPosition, roomToSplit.YPosition + leftRoomHeight, 0);


            int rightVertexIndex = mesh.Vertices.Add(rightVertex);
            int leftVertexIndex = mesh.Vertices.Add(leftVertex);



            mesh.Faces.RemoveFace(faceIndex);
            //Bottom Face
            leftFaceIndex = mesh.Faces.AddFace(vertices[0], vertices[1], rightVertexIndex, leftVertexIndex);
            //Top Face
            rightFaceIndex = mesh.Faces.AddFace(leftVertexIndex, rightVertexIndex, vertices[2], vertices[3]);



            // Debug.Log($"Right Vertex: {rightVertex.X},{rightVertex.Y}");
            // Debug.Log($"Left Vertex: {leftVertex.X},{leftVertex.Y}");


        }
        else {
            //Left Room
            int leftRoomWidth = (int)(roomToSplit.Width * roomRatio);
            leftRoom = new Room(roomToSplit.XPosition, roomToSplit.YPosition, leftRoomWidth, roomToSplit.Height);

            //Right Room
            int rightRoomWidth = roomToSplit.Width - leftRoomWidth;
            rightRoom = new Room(roomToSplit.XPosition + leftRoomWidth, roomToSplit.YPosition, rightRoomWidth, roomToSplit.Height);



            PlanktonXYZ topVertex = new PlanktonXYZ(roomToSplit.XPosition + leftRoomWidth, roomToSplit.YPosition + roomToSplit.Height, 0);
            PlanktonXYZ bottomVertex = new PlanktonXYZ(roomToSplit.XPosition + leftRoomWidth, roomToSplit.YPosition, 0);


            int topVertexIndex = mesh.Vertices.Add(topVertex);
            int bottomVertexIndex = mesh.Vertices.Add(bottomVertex);



            mesh.Faces.RemoveFace(faceIndex);
            //Left Face
            leftFaceIndex = mesh.Faces.AddFace(vertices[0], bottomVertexIndex, topVertexIndex, vertices[3]);
            //Right Face
            rightFaceIndex = mesh.Faces.AddFace(bottomVertexIndex, vertices[1], vertices[2], topVertexIndex);


            // Debug.Log($"Right Vertex: {topVertex.X},{topVertex.Y}");
            // Debug.Log($"Left Vertex: {bottomVertex.X},{bottomVertex.Y}");
        }



        numberOfSteps--;

        RoomNode leftNode = new RoomNode(leftRoom);
        leftNode.Head = head;
        head.Left = leftNode;

        RoomNode rightNode = new RoomNode(rightRoom);
        rightNode.Head = head;
        head.Right = rightNode;

        PartitionLeft(ref head.Left, !splitHorizontal, numberOfSteps, leftFaceIndex);
        PartitionRight(ref head.Right, !splitHorizontal, numberOfSteps, rightFaceIndex);


        return;



    }


    void PartitionLeft(ref RoomNode head, bool splitHorizontal, int numberOfSteps, int faceIndex) {
        Room leftRoom;
        Room rightRoom;

        int leftFaceIndex;
        int rightFaceIndex;
        int rightPartitionVertex;

        int[] vertices = mesh.Faces.GetFaceVertices(faceIndex);

        if (numberOfSteps <= 0) {

            rooms.Add(head.Room);

            return;
        }


        float roomRatio = (float)(random.NextDouble() * ratioDifference) + lowerRatio;
        Room roomToSplit = head.Room;

        // foreach (int vertex in vertices) {
        //     Debug.Log($"X: {mesh.Vertices[vertex].X} Y: {mesh.Vertices[vertex].Y}");
        // }


        if (splitHorizontal) {

            //Bottom Room
            int leftRoomHeight = (int)(roomToSplit.Height * roomRatio);
            leftRoom = new Room(roomToSplit.XPosition, roomToSplit.YPosition, roomToSplit.Width, leftRoomHeight);


            //Top Room
            int rightRoomHeight = roomToSplit.Height - leftRoomHeight;
            rightRoom = new Room(roomToSplit.XPosition, roomToSplit.YPosition + leftRoomHeight, roomToSplit.Width, rightRoomHeight);



            PlanktonXYZ rightVertex = new PlanktonXYZ(roomToSplit.XPosition + roomToSplit.Width, roomToSplit.YPosition + leftRoomHeight, 0);
            PlanktonXYZ leftVertex = new PlanktonXYZ(roomToSplit.XPosition, roomToSplit.YPosition + leftRoomHeight, 0);


            int rightVertexIndex = mesh.Vertices.Add(rightVertex);
            int leftVertexIndex = mesh.Vertices.Add(leftVertex);

            rightPartitionVertex = rightVertexIndex;

            mesh.Faces.RemoveFace(faceIndex);
            //Bottom Face
            leftFaceIndex = mesh.Faces.AddFace(vertices[0], vertices[1], rightVertexIndex, leftVertexIndex);
            //Top Face
            rightFaceIndex = mesh.Faces.AddFace(leftVertexIndex, rightVertexIndex, vertices[2], vertices[3]);



            // Debug.Log($"Right Vertex: {rightVertex.X},{rightVertex.Y}");
            // Debug.Log($"Left Vertex: {leftVertex.X},{leftVertex.Y}");


        }
        else {
            //Left Room
            int leftRoomWidth = (int)(roomToSplit.Width * roomRatio);
            leftRoom = new Room(roomToSplit.XPosition, roomToSplit.YPosition, leftRoomWidth, roomToSplit.Height);

            //Right Room
            int rightRoomWidth = roomToSplit.Width - leftRoomWidth;
            rightRoom = new Room(roomToSplit.XPosition + leftRoomWidth, roomToSplit.YPosition, rightRoomWidth, roomToSplit.Height);



            PlanktonXYZ topVertex = new PlanktonXYZ(roomToSplit.XPosition + leftRoomWidth, roomToSplit.YPosition + roomToSplit.Height, 0);
            PlanktonXYZ bottomVertex = new PlanktonXYZ(roomToSplit.XPosition + leftRoomWidth, roomToSplit.YPosition, 0);


            int topVertexIndex = mesh.Vertices.Add(topVertex);
            int bottomVertexIndex = mesh.Vertices.Add(bottomVertex);

            rightPartitionVertex = bottomVertexIndex;

            mesh.Faces.RemoveFace(faceIndex);
            //Left Face
            leftFaceIndex = mesh.Faces.AddFace(vertices[0], bottomVertexIndex, topVertexIndex, vertices[3]);
            //Right Face
            rightFaceIndex = mesh.Faces.AddFace(bottomVertexIndex, vertices[1], vertices[2], topVertexIndex);


            // Debug.Log($"Right Vertex: {topVertex.X},{topVertex.Y}");
            // Debug.Log($"Left Vertex: {bottomVertex.X},{bottomVertex.Y}");
        }



        numberOfSteps--;

        RoomNode leftNode = new RoomNode(leftRoom);
        leftNode.Head = head;
        head.Left = leftNode;

        RoomNode rightNode = new RoomNode(rightRoom);
        rightNode.Head = head;
        head.Right = rightNode;

        PartitionLeft(ref head.Left, !splitHorizontal, numberOfSteps, leftFaceIndex);
        PartitionRight(ref head.Right, !splitHorizontal, numberOfSteps, rightFaceIndex, rightPartitionVertex);


        return;

    }

    void PartitionRight(ref RoomNode head, bool splitHorizontal, int numberOfSteps, int faceIndex, int rightPartitionVertex = -1) {
        Room leftRoom;
        Room rightRoom;

        int leftFaceIndex;
        int rightFaceIndex;

        int[] vertices = mesh.Faces.GetFaceVertices(faceIndex);

        if (numberOfSteps <= 0) {

            rooms.Add(head.Room);

            return;
        }


        float roomRatio = (float)(random.NextDouble() * ratioDifference) + lowerRatio;
        Room roomToSplit = head.Room;

        // foreach (int vertex in vertices) {
        //     Debug.Log($"X: {mesh.Vertices[vertex].X} Y: {mesh.Vertices[vertex].Y}");
        // }


        if (splitHorizontal) {

            //Bottom Room
            int leftRoomHeight = (int)(roomToSplit.Height * roomRatio);
            leftRoom = new Room(roomToSplit.XPosition, roomToSplit.YPosition, roomToSplit.Width, leftRoomHeight);


            //Top Room
            int rightRoomHeight = roomToSplit.Height - leftRoomHeight;
            rightRoom = new Room(roomToSplit.XPosition, roomToSplit.YPosition + leftRoomHeight, roomToSplit.Width, rightRoomHeight);


            //Deals with the vertex created from the last partition, if there was one
            int leftVertexIndex;
            if(rightPartitionVertex == -1) {
                PlanktonXYZ leftVertex = new PlanktonXYZ(roomToSplit.XPosition, roomToSplit.YPosition + leftRoomHeight, 0);
               leftVertexIndex = mesh.Vertices.Add(leftVertex);
            }
            else {
                leftVertexIndex = rightPartitionVertex;
            }

            PlanktonXYZ rightVertex = new PlanktonXYZ(roomToSplit.XPosition + roomToSplit.Width, roomToSplit.YPosition + leftRoomHeight, 0);
             
            int rightVertexIndex = mesh.Vertices.Add(rightVertex);



            mesh.Faces.RemoveFace(faceIndex);
            //Bottom Face
            leftFaceIndex = mesh.Faces.AddFace(vertices[0], vertices[1], rightVertexIndex, leftVertexIndex);
            //Top Face
            rightFaceIndex = mesh.Faces.AddFace(leftVertexIndex, rightVertexIndex, vertices[2], vertices[3]);



            // Debug.Log($"Right Vertex: {rightVertex.X},{rightVertex.Y}");
            // Debug.Log($"Left Vertex: {leftVertex.X},{leftVertex.Y}");


        }
        else {
            //Left Room
            int leftRoomWidth = (int)(roomToSplit.Width * roomRatio);
            leftRoom = new Room(roomToSplit.XPosition, roomToSplit.YPosition, leftRoomWidth, roomToSplit.Height);

            //Right Room
            int rightRoomWidth = roomToSplit.Width - leftRoomWidth;
            rightRoom = new Room(roomToSplit.XPosition + leftRoomWidth, roomToSplit.YPosition, rightRoomWidth, roomToSplit.Height);





            int topVertexIndex;

            if(rightPartitionVertex == -1) {
                PlanktonXYZ topVertex = new PlanktonXYZ(roomToSplit.XPosition + leftRoomWidth, roomToSplit.YPosition + roomToSplit.Height, 0);
                topVertexIndex = mesh.Vertices.Add(topVertex);
            }
            else {
                topVertexIndex = rightPartitionVertex;
            }

            PlanktonXYZ bottomVertex = new PlanktonXYZ(roomToSplit.XPosition + leftRoomWidth, roomToSplit.YPosition, 0);
            int bottomVertexIndex = mesh.Vertices.Add(bottomVertex);



            mesh.Faces.RemoveFace(faceIndex);
            //Left Face
            leftFaceIndex = mesh.Faces.AddFace(vertices[0], bottomVertexIndex, topVertexIndex, vertices[3]);
            //Right Face
            rightFaceIndex = mesh.Faces.AddFace(bottomVertexIndex, vertices[1], vertices[2], topVertexIndex);


        }



        numberOfSteps--;

        RoomNode leftNode = new RoomNode(leftRoom);
        leftNode.Head = head;
        head.Left = leftNode;

        RoomNode rightNode = new RoomNode(rightRoom);
        rightNode.Head = head;
        head.Right = rightNode;


        Debug.Log(rightFaceIndex);
        PartitionLeft(ref head.Left, !splitHorizontal, numberOfSteps, leftFaceIndex);
        PartitionRight(ref head.Right, !splitHorizontal, numberOfSteps, rightFaceIndex);


        return;

    }


    
}