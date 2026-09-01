using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TutorialDiary : DiaryObject
{
    public TutorialDiaryDoor tutorialDiaryDoor; //일기장 문

    private TutorialManager m_tutorialManager;
    private Collider m_collider;

    public override void Spawned()
    {
        base.Spawned();
        Debug.Log("Tutorial Diary Spawned");

        m_tutorialManager = FindObjectOfType<TutorialManager>();
        m_collider = GetComponent<Collider>();
    }

    public override void OnInteract(NetworkObject playerObject) {
        base.OnInteract(playerObject);

        Debug.Log("Tutorial Diary Interact, m_tutorialManager: " + m_tutorialManager);
        m_tutorialManager.ShowTutorialGoalTextRpc(2);
        m_tutorialManager.ShowTutorialTextRpc(1);
        tutorialDiaryDoor.EnableInteractRpc(); //일기장 문 열기
        DisableColliderRpc();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void DisableColliderRpc()
    {
        m_collider.enabled = false;
    }
}
