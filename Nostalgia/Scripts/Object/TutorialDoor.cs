using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Nostal.Interfaces;

public class TutorialDoor : NetworkBehaviour, IInteractable
{
    [SerializeField] private NetworkObject insidePos;
    public TutorialManager tutorialManager;

    [Header("Interactable Prompt Data")] 
    [SerializeField] private InteractPromptData m_interactPromptData;
    
    public void OnInteract(NetworkObject playerObject)
    {
        StartCoroutine(EnterHospital());
    }

    public IEnumerator EnterHospital()
    {
        UIManager.Instance.FadeView.FadeOut(2);
        yield return new WaitForSeconds(2);
        PlayerMovement playerMovement = GameManager.Instance.GetLocalPlayer().GetComponent<PlayerMovement>();

        //발자국 소리 변경
        playerMovement.SetDirt(false);
        playerMovement.TeleportRpc(insidePos);
        UIManager.Instance.FadeView.FadeIn(2);

        // 튜토리얼 텍스트 띄우기
        tutorialManager.ShowTutorialGoalTextRpc(1);
    }

    public InteractPromptData GetInteractPromptData()
    {
        return m_interactPromptData;
    }
}
