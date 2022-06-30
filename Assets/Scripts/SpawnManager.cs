using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnManager : MonoBehaviour {



    [field: SerializeField] public List<GameObject> Spawns { get; private set; }

    public static SpawnManager Instance;

    int spawnNumber = -1;


    void Awake() {
        Debug.Log("SceneManager Awake");
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }


    }



    public GameObject GetSpawnLocation() {

        return Spawns[++spawnNumber % Spawns.Count];
    }



}
