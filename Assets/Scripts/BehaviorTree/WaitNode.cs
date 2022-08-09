using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitNode : ActionNode {

    public float Duration = 1;
    float startTime; 

    protected override void OnStart() {
        startTime = Time.time;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if(Time.time - startTime > Duration) {
            return State.Success;
        }
        return State.Running;
    }
}
