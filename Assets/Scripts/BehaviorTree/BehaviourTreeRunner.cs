using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;

    void Awake() {
        tree = tree.Clone();
    }
    
    void Update() {
        tree.Update();
    }
}

