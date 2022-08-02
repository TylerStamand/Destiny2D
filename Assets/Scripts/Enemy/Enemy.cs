using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening;
using Random = UnityEngine.Random;


[System.Serializable]
public struct DropData {
    [Range(0, 1)]
    public float ChanceToDrop;
    public ItemData Item;
    public MinMaxInt AmountPossible;
} 

public class Enemy : NetworkBehaviour, IDamageable {

    [field: SerializeField] public NetworkVariable<float> Health { get; private set; } = new NetworkVariable<float>();
    [SerializeField] float unitCollisionDistance;    
    [SerializeField] LayerMask layersToStopFrom;
    [SerializeField] List<DropData> Drops;

    [Header("Animation")]
    [SerializeField] float damageAnimationSpeed = .1f;

    public Action<Enemy> OnDie;

    protected new Rigidbody2D rigidbody;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
   
    protected virtual void Awake() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();

    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

    }

    void OnCollisionEnter2D(Collision collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Unit") || collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            Vector2 dir = collision.contacts[0].point - transform.position;
            dir = -dir.normalized;

            rigidbody.AddForce(dir);
        }
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
        DropItems();
        NetworkObject.Despawn();
        OnDie?.Invoke(this);
    }


    void DropItems() {
       
        
        foreach(DropData drop in Drops) {
            float dropRoll = Random.Range(0,1);
            if(dropRoll <= drop.ChanceToDrop) {
                int numberToDrop = Random.Range(drop.AmountPossible.MinValue, drop.AmountPossible.MaxValue);
                Debug.Log($"Max {drop.AmountPossible.MaxValue} Min: {drop.AmountPossible.MinValue} Dropped: {numberToDrop}");
                for (int i = 0; i <= numberToDrop; i++) {
                    DropServer dropPrefab = ResourceManager.Instance.DropPrefab; 
                    DropServer dropInstance = Instantiate(dropPrefab, transform.position, Quaternion.identity);
                    
                    Item itemToDrop = drop.Item.CreateItem();
                    dropInstance.SetItem(itemToDrop);
                    dropInstance.NetworkObject.Spawn();
                    
                    
                }
            }

        }
        
       

    }


}
