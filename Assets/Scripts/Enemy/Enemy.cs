using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening;
using Random = System.Random;


public class Enemy : NetworkBehaviour, IDamageable {

    [field: SerializeField] public NetworkVariable<float> Health { get; private set; } = new NetworkVariable<float>();
    
    [Header("Drops")]

    [SerializeField] MinMaxInt numberOfDrops;
    [SerializeField] List<DropServer> Drops;

    [Header("Animation")]
    [SerializeField] float damageAnimationSpeed = .1f;

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;


    protected virtual void Awake() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage) {


        Health.Value -= damage;
        Debug.Log(gameObject.name + " health " + Health.Value);

        if (Health.Value <= 0) {
            Die();
            return;
        }

        TakeDamageAnimationClientRpc();
    }


    [ClientRpc]
    void TakeDamageAnimationClientRpc() {
        Sequence damageSequence = DOTween.Sequence();
        damageSequence.Append(spriteRenderer.DOFade(.1f, damageAnimationSpeed))
           .Append(spriteRenderer.DOFade(1, damageAnimationSpeed)).SetLoops(2);
    }

    void Die() {

        //Drops
        Random random = new Random();
        int dropNumber = random.Next(numberOfDrops.MinValue, numberOfDrops.MaxValue + 1);
        for (int i = 0; i < dropNumber; i++) {
            int dropListIndex = random.Next(0, Drops.Count);
            DropServer dropPrefab = Drops[dropListIndex];
            DropServer drop = Instantiate(dropPrefab, transform.position, Quaternion.identity);
            drop.NetworkObject.Spawn();
        }


        NetworkObject.Despawn();


    }


}
