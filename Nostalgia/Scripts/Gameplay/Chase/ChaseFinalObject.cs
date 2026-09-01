using _Scripts.Interfaces;
using Nostal.Interfaces;
using UnityEngine;
using Fusion;

public class ChaseFinalObject : NetworkBehaviour, IInteractable, IResettable
{
    private bool isFatherInteract = false;
    private bool isDaughterInteract = false;
    public Collider m_collider;
    [SerializeField] private InteractPromptData[] m_interactPromptData;

    public override void Spawned() {
        m_collider = GetComponent<Collider>();
    }

    public void OnInteract(NetworkObject netObject) {
        if(netObject == GameManager.Instance.FatherNetworkObject) {
            InteractRpc(true);
        }
        else if(netObject == GameManager.Instance.DaughterNetworkObject) {
            InteractRpc(false);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void InteractRpc(bool isFather)
    {
        if (isFather)
        {
            isFatherInteract = true;
            Debug.Log("Father Interact");
        }
        else
        {
            isDaughterInteract = true;
            Debug.Log("Daughter Interact");
        }

        // 둘다 레버를 당겼을 때
        if (isFatherInteract && isDaughterInteract)
        {
            ChaseMapManager.Instance.Clear();
        }
    }  

    public void Reset() 
    {
        // Debug.Log("Reset", this);
        isFatherInteract = false;
        isDaughterInteract = false;
    }

    public InteractPromptData GetInteractPromptData()
    {
        return m_interactPromptData[0];
    }
}
