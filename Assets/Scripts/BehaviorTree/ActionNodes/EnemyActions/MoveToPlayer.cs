using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPlayer : ActionNode
{
    GameObject target;
    protected override void OnStart() {
        target = DataContext.GetValue("Player") as GameObject;
        // Debug.Log("Keys");
        // foreach(string key in DataContext.data.Keys) {
        //     Debug.Log(key + " " + DataContext.data[key]);
        // }
        if(target != null) {
            Debug.Log("Moving to " + target.name);
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if(target == null) {
            return State.Failure;
        }
        
        if (Vector2.Distance(target.transform.position, Agent.transform.position) >= Agent.StopDistance) {
            Vector2 positionToMoveTowards = Vector2.MoveTowards(Agent.transform.position, target.transform.position, Agent.MoveSpeed * Time.deltaTime);
            Vector2 differenceInPosition = new Vector2(positionToMoveTowards.x - Agent.transform.position.x, positionToMoveTowards.y - Agent.transform.position.y);


            RaycastHit2D[] results = new RaycastHit2D[1];


            int numOfHits = Agent.Collider.Cast(differenceInPosition.normalized, Agent.ContactFilter, results, Agent.MoveSpeed * Time.deltaTime);

            if (numOfHits == 0) {
                Agent.Rigidbody.MovePosition(Agent.transform.position + (Vector3)differenceInPosition);
                return State.Running;
            }
            
            return State.Failure;

        }
        else {
            return State.Success;
        }
    }

   
}
