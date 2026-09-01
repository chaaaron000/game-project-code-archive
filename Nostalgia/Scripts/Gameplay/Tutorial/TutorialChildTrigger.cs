using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TutorialChildTrigger : NetworkBehaviour
{
    [SerializeField] private int index;
    private bool isTriggered = false;
    [SerializeField] public TutorialChild tutorialChild;
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("TutorialChildTrigger: " + index);
        if(isTriggered) return;
        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            tutorialChild.Triggered(index);
        }
    }
}
