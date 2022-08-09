using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(WeaponHolder))]
public class MeleeEnemy : Enemy {

    [SerializeField] float moveSpeed = 1;
    [SerializeField] float alertRadius = 1;
    [SerializeField] float stopDistance = 2;   
    [SerializeField] ContactFilter2D contactFilter; 

    [SerializeField] WeaponData weaponData;

    WeaponHolder weaponHolder;
    new Collider2D collider;
    PlayerControllerServer target;

    protected override void Awake() {
        base.Awake();
        weaponHolder = GetComponent<WeaponHolder>();
        collider = GetComponent<Collider2D>();
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        if(IsServer) {
            Debug.Log("Initializing Enemy Weapon");
            StartCoroutine(Initialize());
            
        }
    }


    IEnumerator Initialize() {
        yield return new WaitForSeconds(.0001f);
        Debug.Log("Initializing Enemy");
        while(!weaponHolder.Initialized) {
            yield return null;
        }
        weaponHolder.EquipWeaponServer((WeaponItem)weaponData.CreateItem());
    }



    void Update() {
        if (IsServer) {
            Move();
        }

        if (IsClient) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, alertRadius, LayerMask.GetMask(new string[] { "Player" }));
            if (colliders.Count() > 0) {
                target = colliders[0].GetComponent<PlayerControllerServer>();
                if ((target.transform.position.x - transform.position.x) >= 0) {
                    spriteRenderer.flipX = false;
                }
                else {
                    spriteRenderer.flipX = true;
                }
            }
        }
    }

   

    void Move() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, alertRadius, LayerMask.GetMask(new string[] { "Player" }));
        if (colliders.Count() > 0) {
            target = colliders[0].GetComponent<PlayerControllerServer>();

            if (Vector2.Distance(target.transform.position, transform.position) >= stopDistance) {
                Vector2 positionToMoveTowards = Vector2.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime); 
                Vector2 differenceInPosition = new Vector2(positionToMoveTowards.x - transform.position.x, positionToMoveTowards.y - transform.position.y);


                RaycastHit2D[] results = new RaycastHit2D[1];


                int numOfHits = collider.Cast(differenceInPosition.normalized, contactFilter, results, moveSpeed * Time.deltaTime);

                if(numOfHits == 0) {
                    rigidbody.MovePosition(transform.position + (Vector3)differenceInPosition); 
                }

            }
            else {
                weaponHolder.UseWeapon(Utilities.DirectionFromVector2(target.transform.position - transform.position));
            }
        }
        else {
            target = null;
        }
    }



    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alertRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
