using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosslineCollider : MonoBehaviour
{

    public GameObject doorToInteract;
    private DoorOpenClose doorScript;

    bool isCrossed = false;

    // Start is called before the first frame update
    void Start()
    {
        if (doorToInteract == null)
        {
            Debug.LogWarning("No doors are set to interact with.");
        }

        doorScript = doorToInteract.GetComponent<DoorOpenClose>();

        if (doorScript == null)
        {
            Debug.LogWarning("No DoorOpenClose set.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isCrossed)
        {
            Debug.Log("Player Entered");
            if (CheckPlayer(other))
            {
                doorScript.objectOpen = true;
                doorScript.ObjectClicked();

                isCrossed = true;
                doorScript.doorLocked = true;
            }
        }
    }

    bool CheckPlayer(Collider other)
    {
        Debug.Log("Check Player");
        if (other.CompareTag("Player"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckIsCrossed()
    {
        return isCrossed;
    }
}
