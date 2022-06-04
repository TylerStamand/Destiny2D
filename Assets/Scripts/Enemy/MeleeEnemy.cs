using System.Collections;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using UnityEditor;

[RequireComponent(typeof(WeaponHolder))]
public class MeleeEnemy : Enemy {

    [SerializeField] float moveSpeed = 1;
    [SerializeField] float alertRadius = 1;
    [SerializeField] float stopDistance = 2;    

    [SerializeField] WeaponData weaponData;

    WeaponHolder weaponHolder;

    PlayerControllerServer target;

    protected override void Awake() {
        base.Awake();
        weaponHolder = GetComponent<WeaponHolder>();
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
        weaponHolder.EquipWeaponServerRpc(NetworkObjectId, NetworkManager.Singleton.LocalClientId, weaponData.Name);
    }



    void Update() {
        if (IsServer) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, alertRadius, LayerMask.GetMask(new string[] { "Player" }));
            if (colliders.Count() > 0) {
                target = colliders[0].GetComponent<PlayerControllerServer>();

                if (Vector2.Distance(target.transform.position, transform.position) >= stopDistance) {
                    transform.position = Vector2.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
                }
                else {
                    weaponHolder.UseWeapon(Utilities.DirectionFromVector2(target.transform.position - transform.position));
                }
            }
            else {
                target = null;
            }
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

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alertRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
