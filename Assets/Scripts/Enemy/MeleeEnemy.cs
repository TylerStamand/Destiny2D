using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeleeEnemy : Enemy {

    [SerializeField] float moveSpeed = 1;
    [SerializeField] float alertRadius = 1;
    [SerializeField] float stopDistance = 2;

    PlayerControllerServer target;

    void Update() {
        if(IsServer) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, alertRadius, LayerMask.GetMask(new string[] { "Player" }));
            if (colliders.Count() > 0)
            {
                target = colliders[0].GetComponent<PlayerControllerServer>();
                if (Vector2.Distance(target.transform.position, transform.position) >= stopDistance)
                {
                    transform.position = Vector2.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime );
                }

            }
            else {
                target = null;
            }
        }

        if(IsClient) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, alertRadius, LayerMask.GetMask(new string[] { "Player" }));
            if (colliders.Count() > 0)
            {
                target = colliders[0].GetComponent<PlayerControllerServer>();
                if ((target.transform.position.x - transform.position.x) >= 0)
                {
                    spriteRenderer.flipX = false;
                }
                else {
                    spriteRenderer.flipX = true;
                }

            }
    
        }
        
         
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, alertRadius);

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
