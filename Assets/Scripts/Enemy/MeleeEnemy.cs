using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(WeaponHolder))]
public class MeleeEnemy : Enemy {

    

    [SerializeField] WeaponData weaponData;

    PlayerControllerServer target;


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
        while(!WeaponHolder.Initialized) {
            yield return null;
        }
        WeaponHolder.EquipWeaponServer((WeaponItem)weaponData.CreateItem());
    }


   



    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AlertRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, StopDistance);
    }

}
