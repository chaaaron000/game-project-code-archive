using UnityEngine;
using Fusion;
using Nostal.Interfaces;

public class Door : NetworkBehaviour, IInteractable
{
    public Animator animator;
    public Collider doorCollider;
    [Networked] public bool isOpen { get; set; } = false;
    
    [Header("Interactable Prompt Data")] 
    [SerializeField] private InteractPromptData[] m_interactPromptData;
    
    public override void Spawned() {
        animator = transform.GetComponent<Animator>();
        doorCollider = GetComponent<Collider>();
    }
    
    public virtual void OnInteract(NetworkObject playerObject)
    {
        if (isOpen) 
        {
            PlayAnimationBackwardRpc();
            SoundManager.Instance.SFX_Play_rpc("closeDoor",gameObject.GetComponent<NetworkObject>());
        }
        else 
        {
            PlayAnimationForwardRpc();
            SoundManager.Instance.SFX_Play_rpc("openDoor",gameObject.GetComponent<NetworkObject>());
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void PlayAnimationForwardRpc()
    {
        isOpen = true;
        animator.SetFloat("Speed", 1.0f);
        animator.Play("DoorAnimation", 0, 0f);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void PlayAnimationBackwardRpc()
    {
        isOpen = false;
        animator.SetFloat("Speed", -1.0f);
        animator.Play("DoorAnimation", 0, 1f);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(HasStateAuthority == false) return;
        if(other.tag == "Mob" && isOpen == false) {
            PlayAnimationForwardRpc();
            SoundManager.Instance.SFX_Play_rpc("openDoor",gameObject.GetComponent<NetworkObject>());
        }
    }
    
    public void OnTriggerExit(Collider other)
    {
        if(HasStateAuthority == false) return;
        if(other.tag == "Mob" && isOpen == true) {
            PlayAnimationBackwardRpc();
            SoundManager.Instance.SFX_Play_rpc("closeDoor",gameObject.GetComponent<NetworkObject>());
        }
    }

    public InteractPromptData GetInteractPromptData()
    {
        return (isOpen ? m_interactPromptData[0] : m_interactPromptData[1]);
    }
}
