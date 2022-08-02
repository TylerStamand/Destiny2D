using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening;

public class RangeWeapon : Weapon
{

    [Header("Projectile")]
    [SerializeField] Projectile projectilePrefab;
    
    NetworkVariable<float> projectileSpeed = new NetworkVariable<float>(1);


    SpriteRenderer spriteRenderer;


    void Awake() {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.enabled = false;
        lastUseTime.Value = 0;
    }


    public override void OnNetworkDespawn() {
        if (IsClient && transform.parent != null) {
            transform.parent.DOKill();
        }
    }

    public override void InitializeWeaponServer(WeaponStats weaponStats, bool playerWeapon) {
        base.InitializeWeaponServer(weaponStats, playerWeapon);
        projectileSpeed.Value = weaponStats.ProjectileSpeed;
    }


    [ClientRpc]
    protected override void AttackClientRpc(Direction direction) {
        spriteRenderer.enabled = true;

        if (transform.parent != null) {
            transform.parent.eulerAngles = Utilities.GetAngleFromDirection(direction);
            transform.parent.DOLocalMove(transform.parent.localPosition + transform.parent.transform.up * 1f, .1f).SetLoops(2, LoopType.Yoyo).onComplete +=
                () => {
                    spriteRenderer.enabled = false;
                };
        }
        else {
            Debug.LogWarning("Weapon parent is not assigned, will not animate");
        }

        Projectile projectile = Instantiate(projectilePrefab, transform.parent.position + transform.parent.transform.up, transform.parent.transform.rotation);
        
        projectile.Initialize(projectileSpeed.Value, Damage.Value, ParentNetID, IsOwner);
    }
}
