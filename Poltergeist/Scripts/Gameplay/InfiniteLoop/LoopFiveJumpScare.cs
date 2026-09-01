using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopFiveJumpScare : MonoBehaviour
{

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<ItemDatabase>().hasItem("Key"))
        {
            // Debug.Log("Player Entered");
            anim.Play("Loop5_JumpScare");
        }
    }
}
