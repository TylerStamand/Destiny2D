using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening;
public class Enemy : NetworkBehaviour, IDamageable {

    [field: SerializeField] public NetworkVariable<float> Health {get; private set;} = new NetworkVariable<float>();
    
    [Header("Animation")]
    [SerializeField] float damageAnimationSpeed = .1f;

    Animator animator;
    SpriteRenderer spriteRenderer;
    

    void Awake() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

       
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage) {
        
       
        Health.Value -= damage;
        Debug.Log(gameObject.name + " health " + Health.Value);

        if(Health.Value <= 0) {
            NetworkObject.Despawn();
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

  
}
