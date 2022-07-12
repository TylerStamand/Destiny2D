using System.Collections.Generic;
using System;
using UnityEngine;
public class Room : IEquatable<Room> {

    public readonly int OriginalXPosition;
    public readonly int OriginalYPosition;
    public readonly int OriginalWidth;
    public readonly int OriginalHeight;
    public int XPosition;
    public int YPosition;
    public int Width;
    public int Height;

    public int Area => Width * Height;
    public Vector2 Center => new Vector2(XPosition + Width/2, YPosition + Height/2 );
    public Room(int xPosition, int yPosition, int width, int height) {
        XPosition = xPosition;
        YPosition = yPosition;
        Width = width;
        Height = height;

        OriginalXPosition = xPosition;
        OriginalYPosition = yPosition;
        OriginalWidth = width;
        OriginalHeight = height;

        North = new List<Room>();
        South = new List<Room>();
        East = new List<Room>();
        West = new List<Room>();
    }


    public List<Room> North;
    public List<Room> South;
    public List<Room> East;
    public List<Room> West;

    public List<Room> GetAdjacentRooms() {
        List<Room> rooms =  new List<Room>();
        rooms.AddRange(North);
        rooms.AddRange(South);
        rooms.AddRange(East);
        rooms.AddRange(West);
        return rooms;
    }

    public bool Equals(Room other) {
        if(XPosition == other.XPosition &&
            YPosition == other.YPosition &&
            Width == other.Width &&
            Height == other.Height) {
                return true;
        }
        return false;
    }

    public override bool Equals(object obj) {
        return Equals(obj as Room);
    }
}