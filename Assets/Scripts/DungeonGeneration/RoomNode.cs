

public class RoomNode {

    public RoomNode(Room room, RoomNode head = null, RoomNode left = null, RoomNode right = null) {
        Room = room;
        Head = head;
        Left = left;
        Right = right;
    }

    public RoomNode Head;
    public RoomNode Left;
    public RoomNode Right;

    public Room Room;

}