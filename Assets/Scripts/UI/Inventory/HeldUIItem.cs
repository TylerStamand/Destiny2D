using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldUIItem : MonoBehaviour
{

    Camera mainCamera;
    Vector3 mask = new Vector3(1, 1, 0);

    void Awake() {
        mainCamera = Camera.main;
    }

    void Update() {
        Vector3 newPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        newPosition = new Vector3(newPosition.x, newPosition.y, 0);

        transform.position = newPosition;
        
    }
}
