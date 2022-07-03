using System;

public class RoomPartitioner {

    // public static List<Room> PartitionRooms(Room room, int numberOfSteps) {

    // } 

    RoomNode head;

    Random random;

    float lowerRatio;
    float upperRatio;
    float ratioDifference;

    bool splitHorizontal;

    int minArea;

    RoomPartitioner(int seed, Room room) {
        random = new Random(seed);
        lowerRatio = 30;
        upperRatio = 70;
        ratioDifference = upperRatio - lowerRatio; 
        head = new RoomNode(room);
        splitHorizontal = true;
    }

    RoomNode SimulationStep(RoomNode head, bool splitHorizontal, int numberOfSteps) {
        float roomRatio = (float)(random.NextDouble() * ratioDifference) + lowerRatio;
        Room roomToSplit = head.Room;

        Room leftRoom;
        Room rightRoom;

        if(numberOfSteps <= 0) {
            return null;
        }

        if(splitHorizontal) {

            //Top Room
            int leftRoomHeight = (int) (roomToSplit.Height * roomRatio);
            leftRoom = new Room(roomToSplit.XPosition, roomToSplit.YPosition, roomToSplit.Width, leftRoomHeight ); 
            
            //Bottom Room
            int rightRoomHeight = roomToSplit.Height - leftRoomHeight; 
            rightRoom =  new Room(roomToSplit.XPosition, roomToSplit.YPosition - leftRoomHeight, roomToSplit.Width, rightRoomHeight);

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

        head.Left = new RoomNode(leftRoom, head);
        head.Right = new RoomNode(rightRoom, head);

        SimulationStep(head.Left, !splitHorizontal, numberOfSteps);
        SimulationStep(head.Right, !splitHorizontal, numberOfSteps);


        return head;
    
    
    
    }
    
}