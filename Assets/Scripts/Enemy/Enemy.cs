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

public abstract class Enemy : NetworkBehaviour, IDamageable {
   
    [field: SerializeField] public NetworkVariable<float> Health { get; private set; } = new NetworkVariable<float>();
    
    [SerializeField] float unitCollisionDistance;    
    [SerializeField] LayerMask layersToStopFrom;
    [SerializeField] List<DropData> Drops;
    [SerializeField] protected BehaviourTree behaviourTree;

    [Header("Animation")]
    [SerializeField] float damageAnimationSpeed = .1f;

    public float MoveSpeed = 1;
    public float AlertRadius = 1;
    public float StopDistance = 2;
    public float MaxAttackRange = 4;
    public ContactFilter2D ContactFilter;
    public Collider2D Collider {get; protected set;}
    public Rigidbody2D Rigidbody {get; protected set;}
    public WeaponHolder WeaponHolder {get; protected set;}

    public Action<Enemy> OnDie;

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
   
    protected virtual void Awake() {
        Collider = GetComponent<Collider2D>();
        Rigidbody = GetComponent<Rigidbody2D>();
        WeaponHolder = GetComponent<WeaponHolder>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        behaviourTree = behaviourTree.Clone(this);
    }

    protected virtual void Update() {
        if(IsServer) {
            behaviourTree.Update();
        }

        if (IsClient) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, AlertRadius, LayerMask.GetMask(new string[] { "Player" }));
            if (colliders.Length > 0) {
                GameObject target = colliders[0].gameObject;
                if ((target.transform.position.x - transform.position.x) >= 0) {
                    spriteRenderer.flipX = false;
                }
                else {
                    spriteRenderer.flipX = true;
                }
            }
        }
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Unit") || collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            Vector2 dir = collision.contacts[0].point - (Vector2)transform.position;
            dir = -dir.normalized;

            Rigidbody.AddForce(dir);
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
