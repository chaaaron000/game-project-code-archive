using _Scripts.Interfaces;
using UnityEngine;
using Fusion;
using UnityEngine.Events;

public class ChaseMapDoor : NetworkBehaviour, IResettable
{
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource;

    [Header("Levers")] 
    [SerializeField] private ChaseLever m_FatherLever;
    [SerializeField] private ChaseLever m_DaughterLever;

    [Header("Events")]
    public UnityEvent OnDoorOpened;

    public void TryOpenDoor()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        if (m_FatherLever.IsLeverPulled && m_DaughterLever.IsLeverPulled)
        {
            InvokeDoorOpenedEventRPC();
            m_DaughterLever.OpenDoorRpc();
            m_FatherLever.OpenDoorRpc();

            SetTriggerRpc("StartChase");
        }
    }
    
    public void Reset()
    {
        // Debug.Log("Reset", this);
        if (!HasStateAuthority)
        {
            return;
        }

        SetTriggerRpc("ResetChase");
    }

    public void SoundPlay()
    {
        audioSource.Play();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void SetTriggerRpc(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void InvokeDoorOpenedEventRPC()
    {
        OnDoorOpened?.Invoke();
    }
}
