using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Nostal;

public class WindowBreak : MonoBehaviour
{
    public GameObject window1;

    public GameObject window2;
    public GameObject window1_break;
    public GameObject window2_break;

    public Animator animator;

    public AudioSource audioSource;

    public void Start() {
        GameplayEventManager.ChaseMapReset += Reset;
    }

    public void OnDestroy() {
        GameplayEventManager.ChaseMapReset -= Reset;
    }

    public void WindowBreaking()
    {
        RPC_setActive(window1, false);

        RPC_setActive(window1_break, true);
    }
    

    public void WindowBreaking2()
    {
        RPC_setActive(window2, false);

        RPC_setActive(window2_break, true);
    }

    public void resetWindow()
    {
        RPC_setActive(window1, true);
        RPC_setActive(window2, true);

        RPC_setActive(window1_break, false);
        RPC_setActive(window2_break, false);
    }

    public void Action()
    {
        RPC_animation_setbool("breaking",true);
    }

    public void Reset()
    {
        RPC_animation_setbool("breaking", false);
        resetWindow();
    }

    public void SoundPlay()
    {
        audioSource.Play();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_animation_setbool(string name, bool value)
    {
        animator.SetBool(name, value);
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_setActive(GameObject gameObject, bool value) 
    {
        gameObject.SetActive(value);
    }
}
