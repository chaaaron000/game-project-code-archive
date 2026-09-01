using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;

public class TutorialChaseDoor : Door
{
    [Networked] private bool canOpen {get; set;} = false;
    private bool isFatherEnter = false;
    private bool isDaughterEnter = false;
    public TutorialManager tutorialManager;

    public override void OnInteract(NetworkObject playerObject)
    {
        if (!canOpen) {
            tutorialManager.ShowTutorialTextRpc(10);
            return;
        }
        base.OnInteract(playerObject);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void CanOpenDoorRpc(bool canOpen) {
        Debug.Log("TutorialChaseDoor: canOpenDoorRpc");
        this.canOpen = canOpen;
    }

    public void PlayerEnter(NetworkObject playerObj) {
        if(HasStateAuthority == false) return;
        if(playerObj == GameManager.Instance.FatherNetworkObject) {
            Debug.Log("TutorialChaseDoor: Father Enter");
            isFatherEnter = true;
        }
        else if(playerObj == GameManager.Instance.DaughterNetworkObject) {
            Debug.Log("TutorialChaseDoor: Daughter Enter");
            isDaughterEnter = true;
        }

        if(isFatherEnter && isDaughterEnter) {
            CanOpenDoorRpc(true);
        }
    }
}

