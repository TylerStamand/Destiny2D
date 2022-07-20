using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraFollower : MonoBehaviour
{
    GameObject playerObject;


    void Update() {
        if(playerObject != null) {
            transform.position = playerObject.transform.position;
        }
    }

    public void SetPlayer(GameObject playerObject) {
        this.playerObject = playerObject;
    }
}
