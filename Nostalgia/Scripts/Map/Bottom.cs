using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottom : MonoBehaviour
{
    private void OnTriggerStay(Collider other) {
        if(other.tag == "Player") {
            Debug.Log("Player is in the bottom");
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            player.Fall();
        }
    } 
}
