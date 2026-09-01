using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseWrongWayTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChaseMapManager.Instance.SetMobWrongWayTrigger(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChaseMapManager.Instance.SetMobWrongWayTrigger(false);
        }
    }
}
