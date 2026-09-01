using System.Collections;
using System.Collections.Generic;
using _Scripts.Interfaces;
using UnityEngine;
using Nostal;

public class ChaseObject : MonoBehaviour, IResettable
{
    public GameObject obj;
    private Vector3 startPosition;
    private Quaternion startRotation;
    public Animator animator;
    public bool isTriggered = false;

    public void Start() {
        startPosition = obj.transform.position;
        startRotation = obj.transform.rotation;
        if(animator == null)
            animator = obj.GetComponent<Animator>();

        GameplayEventManager.ChaseMapReset += Reset;
    }

    public void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player") && !isTriggered) {
            animator.SetFloat("Speed", 1.0f);
            animator.SetTrigger("Trigger");
            isTriggered = true;
        } 
    }

    public void Reset() 
    {
        animator.SetFloat("Speed", -1.0f);
        animator.SetTrigger("Trigger");
        isTriggered = false;
    }

    private void OnDestroy() {
        GameplayEventManager.ChaseMapReset -= Reset;
    }
}
