using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;    
using UnityEditor;

public enum Direction {
    Up, Down, Left, Right
}


public static class Utilities {
    public static Direction DirectionFromVector2(Vector2 vector) {
        Vector3 eulerAngles = Vector3.zero;
        if (vector.x != 0) {
            if (vector.x > 0) {
                return Direction.Right;
            }
            else {
                return Direction.Left;
            }
        }
        if (vector.y != 0) {
            if (vector.y > 0) {
                return Direction.Up;
            }
            else {
                return Direction.Down;
            }
        }

        //Default
        return Direction.Right;
    }


    public static Vector3 GetAngleFromDirection(Direction direction) {
        Vector3 eulerAngles = Vector3.zero;

        switch (direction) {
            case Direction.Right:
                eulerAngles.z = -90;
                return eulerAngles;
            case Direction.Left:
                eulerAngles.z = 90;
                return eulerAngles;
            case Direction.Up:
                eulerAngles.z = 0;
                return eulerAngles;
            case Direction.Down:
                eulerAngles.z = 180;
                return eulerAngles;
            default:
                //Probably should error here instead of returning 0
                return eulerAngles;
        }
    }


    public static Vector3 GetAngleFromVector2(Vector2 vector) {
        return GetAngleFromDirection(DirectionFromVector2(vector));
    }

    public static Vector2 GetDirectionVectorFromDirection(Direction direction) {
        Vector2 directionVector = Vector2.zero;

        switch (direction) {
            case Direction.Right:
                directionVector.x = 1;
                return directionVector;
            case Direction.Left:
                directionVector.x = -1;
                return directionVector;
            case Direction.Up:
                directionVector.y = 1;
                return directionVector;
            case Direction.Down:
                directionVector.y = -1;
                return directionVector;
            default:
                //Probably should error here instead of returning 0
                return directionVector;
        }
    }




}
