using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nostal;

public class ChaseObject2 : MonoBehaviour
{
    //public GameObject obj;
    private Vector3 startPosition;
    private Quaternion startRotation;
    //public Animator animator;
    public WindowBreak windowBreak;
    public bool isTriggered = false;

    public void Start() {
        
        // startPosition = obj.transform.position;
        // startRotation = obj.transform.rotation;
        // if(animator == null)
        //     animator = obj.GetComponent<Animator>();

        // GameplayEventManager.ChaseMapReset += Reset;
    }

    public void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player") && !isTriggered) {
            // animator.SetFloat("Speed", 1.0f);
            // animator.SetTrigger("Trigger");

            windowBreak.Action();

            isTriggered = true;
        } 
    }


    public void Reset() {
        Debug.Log("Resetting Chase Object : " + gameObject);
        // animator.SetFloat("Speed", -1.0f);
        // animator.SetTrigger("Trigger");
        windowBreak.Reset();

        isTriggered = false;
    }

    private void OnDestroy() {
        GameplayEventManager.ChaseMapReset -= Reset;
    }
}
