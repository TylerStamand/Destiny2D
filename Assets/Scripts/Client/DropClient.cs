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

    }

    [ClientRpc]
    public void StartIdleAnimationClientRpc() {

        spriteTransform.DOLocalMoveY(spriteTransform.position.y + idleDeltaY, speed).SetLoops(-1, LoopType.Yoyo);
    }

    [ClientRpc]
    public void SetItemClientRpc(Item item) {
        Debug.Log("Setting Item");
        ItemData itemData = ResourceManager.Instance.GetItemData(item.ItemName);
        Debug.Log($"Item: {itemData.Name}");

        GetComponentInChildren<SpriteRenderer>().sprite = itemData.Sprite;
    }

    public override void OnNetworkDespawn() {
        base.OnNetworkDespawn();
        spriteTransform.DOKill();
    }

}
