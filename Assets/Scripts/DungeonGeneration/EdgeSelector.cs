using System;
using System.Collections.Generic;

public class Edge : IEquatable<Edge> {
    public Edge(Room room1, Room room2, int distance) {
        Room1 = room1;
        Room2 = room2;
        Distance = distance;
    }

    public Room Room1;
    public Room Room2;
    public int Distance;


    public bool Equals(Edge otherEdge) {
        return (Room1 == otherEdge.Room1 && Room2 == otherEdge.Room2) || (Room1 == otherEdge.Room2 && Room2 == otherEdge.Room1);
    }
}

public class EdgeSelector {
    
    public static List<Edge> GetEdges(List<Room> rooms ) {
        return null;
    }

    List<Room> rooms;
    List<Edge> sortedEdges;

    EdgeSelector(List<Room> rooms) {
        this.rooms = rooms;
        SetSortedEdges();
    }

    void SetSortedEdges() {
        foreach (Room room in rooms) {
            foreach (Room adjacentRoom in room.GetAdjacentRooms()) {
                Edge newEdge = new Edge(room, adjacentRoom, CalculateEdgeDistance(room, adjacentRoom));
                if (!sortedEdges.Contains(newEdge)) {
                    for (int i = 0; i < sortedEdges.Count; i++) {
                        if (newEdge.Distance < sortedEdges[i].Distance) {
                            sortedEdges.Insert(i, newEdge);
                        }
                    }
                }
            }
        }
    }

    int CalculateEdgeDistance(Room room1, Room room2) {
        int XDistance = (int)room1.Center.x - (int)room2.Center.x;
        XDistance = Math.Abs(XDistance);
        int YDistance = (int)room1.Center.y - (int)room1.Center.y;
        YDistance = Math.Abs(YDistance);
        return XDistance + YDistance;
    }


}