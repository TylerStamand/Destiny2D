using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldUIItem : MonoBehaviour
{

    Camera mainCamera;
    Vector3 mask = new Vector3(1, 1, 0);
    RectTransform rectTransform;

    void Awake() {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        
    }

    void Update() {
        Vector3 newPosition = Input.mousePosition;
        newPosition = new Vector3(newPosition.x, newPosition.y, 0);

        rectTransform.position = newPosition;
        
    }
}
