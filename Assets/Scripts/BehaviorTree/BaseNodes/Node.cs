using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    public enum State {
        Running,
        Failure,
        Success
    }

    [HideInInspector] public State CurrentState = State.Running;
    [HideInInspector] public Enemy Agent;
    [HideInInspector] public DataContext DataContext = new DataContext();
    [HideInInspector] public bool Started = false;
    [HideInInspector] public string GUID;
    [HideInInspector] public Vector2 Position;

    public State Update() {
        if(!Started) {
            OnStart();
            Started = true;
        }

        CurrentState = OnUpdate();

        if(CurrentState == State.Failure || CurrentState == State.Success) {
            OnStop();
            Started = false;
        }
        
        return CurrentState;
    }

    public virtual Node Clone() {
        return Instantiate(this);
    }

    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract State OnUpdate();
}
