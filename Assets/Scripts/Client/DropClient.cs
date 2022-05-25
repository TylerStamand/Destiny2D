using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening;

public class DropClient : NetworkBehaviour {
    
    static float idleDeltaY = 1.3f;
    static float speed = .7f;

    SpriteRenderer spriteRenderer;
    Transform spriteTransform; 
    public override void OnNetworkSpawn() {
        if(!IsClient) {
            enabled = false;
            return;
        }

        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); 
        spriteTransform = spriteRenderer.gameObject.transform;

    }

    [ClientRpc]
    public void StartIdleAnimationClientRpc() {
        
        spriteTransform.DOLocalMoveY(spriteTransform.position.y + idleDeltaY, speed).SetLoops(-1, LoopType.Yoyo);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        spriteTransform.DOKill();
    }

}
