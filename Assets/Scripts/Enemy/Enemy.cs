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
    
    [SerializeField] List<DropData> Drops;

    [Header("Animation")]
    [SerializeField] float damageAnimationSpeed = .1f;

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
   

    protected virtual void Awake() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

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
    }


    void DropItems() {
       
        
        foreach(DropData drop in Drops) {
            float dropRoll = Random.Range(0,1);
            if(dropRoll <= drop.ChanceToDrop) {
                int numberToDrop = Random.Range(drop.AmountPossible.MinValue, drop.AmountPossible.MaxValue);
                Debug.Log($"Max {drop.AmountPossible.MaxValue} Min: {drop.AmountPossible.MinValue} Dropped: {numberToDrop}");
                for (int i = 0; i <= numberToDrop; i++) {
                    DropServer dropPrefab = ResourceManager.DropPrefab; 
                    DropServer dropInstance = Instantiate(dropPrefab, transform.position, Quaternion.identity);
                    
                    Item itemToDrop = drop.Item.CreateItem();
                    dropInstance.SetItem(itemToDrop);
                    dropInstance.NetworkObject.Spawn();
                    
                    
                }
            }

        }
        
       

    }


}
