using System.Linq;
using UnityEngine;

public class AttackPlayer : ActionNode {


    PlayerControllerServer playerToAttack;

    protected override void OnStart() {
        playerToAttack = FindObjectsOfType<PlayerControllerServer>().OrderBy(
            (player) => Vector2.Distance(Agent.transform.position, player.transform.position)).FirstOrDefault();
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if(Vector2.Distance(Agent.transform.position, playerToAttack.transform.position) > Agent.MaxAttackRange) {
            return State.Failure;
        }



        Agent.WeaponHolder.UseWeapon(Utilities.DirectionFromVector2(playerToAttack.transform.position - Agent.transform.position));

        return State.Success;
    }
}
