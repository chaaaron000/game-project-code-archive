using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TutorialTextTrigger : NetworkBehaviour
{
    [SerializeField] private int tutorialTextIndex;
    [SerializeField] private bool isGoalText;

    private TutorialManager m_tutorialManager;
    private bool isTutorialTriggered = false;

    public override void Spawned()
    {
        base.Spawned();

        m_tutorialManager = FindObjectOfType<TutorialManager>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority)
        {
            return;
        }
        
        if (other.CompareTag("Player") && !isTutorialTriggered)
        {
            // Debug.Log("TutorialTrigger: " + tutorialTextIndex);
            if (!isGoalText)
            {
                m_tutorialManager.ShowTutorialTextRpc(tutorialTextIndex);
            }
            else
            {
                m_tutorialManager.ShowTutorialGoalTextRpc(tutorialTextIndex);
            }
            
            isTutorialTriggered = true;
        }
    }
}
