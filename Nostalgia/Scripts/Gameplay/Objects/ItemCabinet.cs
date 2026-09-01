using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Nostal.Interfaces;
using Unity.VisualScripting;

public class ItemCabinet : NetworkBehaviour, IInteractable
{
    public Animator animator;
    public BoxCollider itemCollider;
    
    [Header("Interactable Prompt Data")] 
    [SerializeField] private InteractPromptData[] m_interactPromptData;
    
    public override void Spawned() {
        animator = transform.GetComponent<Animator>();    
        itemCollider = GetComponent<BoxCollider>();    
    }
    
    public virtual void OnInteract(NetworkObject playerObject)
    {
        PlayAnimationRpc();
        SoundManager.Instance.SFX_Play_rpc("openDoor",gameObject.GetComponent<NetworkObject>());
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void PlayAnimationRpc()
    {
        animator.Play("ItemCabinetAnimation", 0, 0f);
        itemCollider.enabled = false;
    }


    public InteractPromptData GetInteractPromptData()
    {
        return m_interactPromptData[0];
    }
}
