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
    public delegate void TriggerEvent(Collider2D collider2D, GameObject srcGameObject);
    public TriggerEvent OnTriggerEnter;
    public TriggerEvent OnTriggerExit;
    
    protected virtual void OnTriggerEnter2D(Collider2D collider2D) {
        OnTriggerEnter?.Invoke(collider2D, gameObject);
    }

    protected virtual void OnTriggerExit2D(Collider2D collider2D) {
        OnTriggerExit?.Invoke(collider2D, gameObject);
    }
    
}
