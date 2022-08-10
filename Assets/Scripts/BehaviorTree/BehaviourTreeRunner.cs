using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;

    void Awake() {
        Enemy enemy = GetComponent<Enemy>();
        tree = tree.Clone(enemy);
    }
    
    void Update() {
        tree.Update();
    }
}

