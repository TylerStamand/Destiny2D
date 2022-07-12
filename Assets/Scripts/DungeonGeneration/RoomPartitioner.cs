using System;
using System.Collections.Generic;
using Plankton;
using UnityEngine;
using System.Linq;

using Random = System.Random;
public class RoomPartitioner {

    public static List<Room> PartitionRooms(Room room, int seed,  int numberOfSteps, out PlanktonMesh mesh) {
        RoomPartitioner partitioner = new RoomPartitioner(seed, room, true);
        
        partitioner.Partition(ref partitioner.head, partitioner.splitHorizontal, numberOfSteps, 0);
        partitioner.FindAdjacency(ref partitioner.head);
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
    bool initialSplitHorizontal;
    int minArea;

    List<Room> rooms;

    RoomPartitioner(int seed, Room room, bool initialSplitHorizontal) {
       
        random = new Random(seed);
        lowerRatio = .20f;
        upperRatio = .80f;
        ratioDifference = upperRatio - lowerRatio; 
        head = new RoomNode(room);
        this.initialSplitHorizontal = initialSplitHorizontal;
        splitHorizontal = initialSplitHorizontal;
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


        }



        numberOfSteps--;

        RoomNode leftNode = new RoomNode(leftRoom);
        leftNode.Head = head;
        head.Left = leftNode;

        RoomNode rightNode = new RoomNode(rightRoom);
        rightNode.Head = head;
        head.Right = rightNode;

        Partition(ref head.Left, !splitHorizontal, numberOfSteps, leftFaceIndex);
        Partition(ref head.Right, !splitHorizontal, numberOfSteps, rightFaceIndex);


        return;



    }

    void FindAdjacency(ref RoomNode head) {
        
        FindFrontiers(ref head, initialSplitHorizontal);
        MatchFrontiers(ref head, initialSplitHorizontal);
    }

    void FindFrontiers(ref RoomNode head, bool splitHorizontal) {
        if (head.Left != null) {
            FindFrontiers(ref head.Left, !splitHorizontal);
        }
        if (head.Right != null) {
            FindFrontiers(ref head.Right, !splitHorizontal);
        }
        if (!head.IsLeaf) {
            if(splitHorizontal) {
                if(head.Left != null) {
                    head.NorthFrontier.AddRange(head.Left.NorthFrontier);
                    head.EastFrontier.AddRange(head.Left.EastFrontier);
                    head.WestFrontier.AddRange(head.Left.WestFrontier);

                }
                if(head.Right != null) {
                    head.SouthFrontier.AddRange(head.Right.SouthFrontier);
                    head.EastFrontier.AddRange(head.Right.EastFrontier);
                    head.WestFrontier.AddRange(head.Right.WestFrontier);

                }
            
            }
            else {
                if(head.Left != null) {
                    head.WestFrontier.AddRange(head.Left.WestFrontier);
                    head.NorthFrontier.AddRange(head.Left.NorthFrontier);
                    head.SouthFrontier.AddRange(head.Left.SouthFrontier);

                }
                if(head.Right != null) {
                    head.EastFrontier.AddRange(head.Right.EastFrontier);
                    head.NorthFrontier.AddRange(head.Right.NorthFrontier);
                    head.SouthFrontier.AddRange(head.Right.SouthFrontier);
                } 
            }
        }
        else {
            head.NorthFrontier.Add(head);
            head.SouthFrontier.Add(head);
            head.EastFrontier.Add(head);
            head.WestFrontier.Add(head);
        }
    }

    void MatchFrontiers(ref RoomNode head, bool splitHorizontal) {
        if(head.IsLeaf) return;

        else {
            MatchFrontiers(ref head.Left, !splitHorizontal);
            MatchFrontiers(ref head.Right, !splitHorizontal);
           
            if(splitHorizontal) {
                RoomNode currentTop;
                RoomNode currentBottom;

                int topIndex = 0;
                int bottomIndex = 0;

                while(topIndex < head.Left.SouthFrontier.Count && bottomIndex < head.Right.NorthFrontier.Count) {
                    currentTop = head.Left.SouthFrontier[topIndex];
                    currentBottom = head.Right.NorthFrontier[bottomIndex];

                    currentTop.Room.South.Add(currentBottom.Room);
                    currentBottom.Room.North.Add(currentTop.Room);

                    if(currentTop.Room.XPosition + currentTop.Room.Width > currentBottom.Room.XPosition + currentBottom.Room.Width) {
                        bottomIndex++;       
                    }
                    else if (currentTop.Room.XPosition + currentTop.Room.Width < currentBottom.Room.XPosition + currentBottom.Room.Width) {
                        topIndex++;
                    }
                    else {
                        bottomIndex++;
                        topIndex++;
                    }
                }
            }
            else {
                RoomNode currentLeft;
                RoomNode currentRight;

                int leftIndex = 0;
                int rightIndex = 0;

                while (leftIndex < head.Left.EastFrontier.Count && rightIndex < head.Right.WestFrontier.Count) {
                    currentLeft = head.Left.EastFrontier[leftIndex];
                    currentRight = head.Right.WestFrontier[rightIndex];

                    currentLeft.Room.East.Add(currentRight.Room);
                    currentRight.Room.West.Add(currentLeft.Room);

                    if (currentLeft.Room.YPosition + currentLeft.Room.Height > currentRight.Room.YPosition + currentRight.Room.Height) {
                        rightIndex++;
                    }
                    else if (currentLeft.Room.YPosition + currentLeft.Room.Height < currentRight.Room.YPosition + currentRight.Room.Height) {
                        leftIndex++;
                    }
                    else {
                        rightIndex++;
                        leftIndex++;
                    }
                }

                
            }
        }
    }
}