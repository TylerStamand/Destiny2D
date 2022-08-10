using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPlayer : ActionNode
{

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(Agent.transform.position, Agent.AlertRadius, LayerMask.GetMask(new string[] { "Player" }));
        if (colliders.Length > 0) {
            // Debug.Log("Setting found Player");
            DataContext.SetValue("Player", colliders[0].gameObject);
            // Debug.Log(colliders[0].name);
            // Debug.Log(DataContext.GetValue("Player"));
            return State.Success;
        }
        else {
            DataContext.SetValue("Player", null);
            return State.Failure;
        }
    }

}
