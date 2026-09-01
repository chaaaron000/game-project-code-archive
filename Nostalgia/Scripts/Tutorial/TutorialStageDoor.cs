using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TutorialStageDoor : Door
{
    private TutorialManager m_tutorialManager;

    public override void Spawned()
    {
        base.Spawned();

        m_tutorialManager = FindObjectOfType<TutorialManager>();
    }

    public override void OnInteract(NetworkObject playerObject) 
    {
        base.OnInteract(playerObject);

        m_tutorialManager.StartStageRpc();
    }

    public void ResetDoor() {
        if (isOpen) 
        {
            PlayAnimationBackwardRpc();
        }
    }
}
