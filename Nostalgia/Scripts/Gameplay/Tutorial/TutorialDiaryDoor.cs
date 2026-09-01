using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TutorialDiaryDoor : Door
{
    private bool isInteractable = false;

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void EnableInteractRpc() {
        isInteractable = true;
    }

    public override void OnInteract(NetworkObject playerObject) {
        if(!isInteractable) return; //일기장 획득 전 상호작용 불가

        base.OnInteract(playerObject);
    }
}
