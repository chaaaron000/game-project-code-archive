using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseClearTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            GameManager.Instance.Clear(NostalgiaGameLevel.Ending);
        }
    }
}
