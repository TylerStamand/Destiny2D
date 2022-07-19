using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D))]
public class TriggerEventExposer : MonoBehaviour
{
    // public Action<Collider2D> OnTriggerExit;
    // public Action<Collider2D> OnTrigger;
    
    public Action<Collider2D> OnTriggerEnter;
    
    void OnTriggerEnter2D(Collider2D collider2D) {
        OnTriggerEnter?.Invoke(collider2D);
    }
    
}
