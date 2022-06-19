using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening;

[DefaultExecutionOrder(1)]
public class DropClient : NetworkBehaviour {

    static float idleDeltaY = 1.3f;
    static float speed = .7f;

    SpriteRenderer spriteRenderer;
    Transform spriteTransform;
    public override void OnNetworkSpawn() {
        if (!IsClient) {
            enabled = false;
            return;
        }

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteTransform = spriteRenderer.gameObject.transform;

        DropServer server = GetComponent<DropServer>();
 
        ItemData itemData = ResourceManager.Instance.GetItemData(server.ItemName.Value.Value.ToString());

        if(itemData != null) {
            spriteRenderer.sprite = itemData.Sprite;
        }
        else {
            Debug.Log("Item data null");
        }
        

        if(server.IsAnimating.Value) {
            StartIdleAnimation();
        }
    }


    [ClientRpc]
    public void StartIdleAnimationClientRpc() {
        StartIdleAnimation();
      
    }

    //Change this to the new method in netspawn
    [ClientRpc]
    public void SetItemClientRpc(string itemName) {
        Debug.Log("Setting ItemDrop on Client");
        ItemData itemData = ResourceManager.Instance.GetItemData(itemName);
        GetComponentInChildren<SpriteRenderer>().sprite = itemData.Sprite;
    }

    public override void OnNetworkDespawn() {
        base.OnNetworkDespawn();
        spriteTransform.DOKill();
    }

    private void StartIdleAnimation() {
        spriteTransform.DOLocalMoveY(spriteTransform.position.y + idleDeltaY, speed).SetLoops(-1, LoopType.Yoyo);
    }

}
