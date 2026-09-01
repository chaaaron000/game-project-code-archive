using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TutorialRoomEnterTrigger : NetworkBehaviour
{
    public TutorialChaseDoor tutorialChaseDoor;
    public GameObject invisibleWall;

    private void OnTriggerEnter(Collider other)
    {
        if(GameManager.Instance.GetLocalPlayer() == other.GetComponent<NetworkObject>())
        {
            Debug.Log("Enable tutorial InvisibleWall");
            invisibleWall.SetActive(true);
        }

        if(!HasStateAuthority) return;

        if (other.CompareTag("Player"))
        {
            tutorialChaseDoor.PlayerEnter(other.GetComponent<NetworkObject>());
        }
    }
}
