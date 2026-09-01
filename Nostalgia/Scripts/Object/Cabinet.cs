using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Nostal.Interfaces;

public class Cabinet : NetworkBehaviour, IInteractable
{
    //각각 캐비넷 입장, 퇴장 시 플레이어를 이동시킬 위치와 회전값
    public Transform cabinetInPosition;
    public Transform cabinetOutPosition;

    [SerializeField][Networked] private bool isEmpty { get; set; } = true;
    [Networked] private bool isLocked { get; set; } = false;
    private const float interactDelay = 0.5f;
    
    [Header("Interactable Prompt Data")] 
    [SerializeField] private InteractPromptData m_interactPromptData;

    public void OnInteract(NetworkObject playerObject)
    {
        Player player = playerObject.GetComponent<Player>();
        if (player == null)
        {
            return;
        }
        
        HandleInteractRpc(player);
    }

    public InteractPromptData GetInteractPromptData()
    {
        return m_interactPromptData;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void HandleInteractRpc(Player player)
    {
        if (isLocked)
        {
            return;
        }
        
        isLocked = true;
        
        bool playerHidden = player.isHidden;
        bool tryingToEnter = isEmpty && !playerHidden;
        bool tryingToExit = !isEmpty && playerHidden;
        
        if (tryingToEnter)  // 입장 가능한 상태 and 플레이어의 입장 시도
        {
            player.EnterCabinetRpc(cabinetInPosition.position, cabinetInPosition.rotation, gameObject.GetComponent<NetworkObject>());
            //캐비넷 소리
            SoundManager.Instance.SFX_Play_rpc("openCabinet",gameObject.GetComponent<NetworkObject>());

            isEmpty = false;

        }
        else if (tryingToExit)  // 입장 불가능한 상태 and 플레이어의 퇴장 시도 -> 이미 입장한 상태라면(퇴장)
        {
            player.ExitCabinetRpc(cabinetOutPosition.position, cabinetOutPosition.rotation);
            isEmpty = true;
        }

        
        StartCoroutine(ReleaseLockDelayCoroutine());
    }
    
    private IEnumerator ReleaseLockDelayCoroutine()
    {
        yield return new WaitForSeconds(interactDelay);
        isLocked = false;
    }
}
