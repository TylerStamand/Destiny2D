using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D))]
public class TriggerEventExposer : MonoBehaviour
{
    // public Action<Collider2D> OnTriggerExit;
    // public Action<Collider2D> OnTrigger;
    
    //Make this custom
    public Action<Collider2D, GameObject> OnTriggerEnter;
    
    void OnTriggerEnter2D(Collider2D collider2D) {
        OnTriggerEnter?.Invoke(collider2D, gameObject);
    }
    
}
