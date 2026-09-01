using System.Collections;
using Fusion;
using Nostal.Util;
using UnityEngine;

public class ExitDoor : NetworkBehaviour
{
    [SerializeField] private bool _isOpen;
    [Networked] private bool m_bIsFatherClear { get; set; }
    [Networked] private bool m_bIsDaughterClear { get; set; }

    [Header("문 파츠")]
    [SerializeField] private GameObject m_DoorFrame;
    [SerializeField] private GameObject m_Exit;

    [Header("레이어")] 
    [SerializeField] private LayerMask m_DefaultLayer;
    [SerializeField] private LayerMask m_ExitHighlightLayer;
    
    [Header("다음 씬")]
    [SerializeField] public NostalgiaGameLevel nextScene;

    private int m_DefaultLayerIndex;
    private int m_ExitHighlightLayerIndex;

    public override void Spawned() 
    {
        if (GameManager.Instance != null) 
        {
            GameManager.Instance._exitDoor = this;
        }
        
        m_DefaultLayerIndex = LayerUtility.GetFirstLayerIndex(m_DefaultLayer);
        m_ExitHighlightLayerIndex = LayerUtility.GetFirstLayerIndex(m_ExitHighlightLayer);

        gameObject.layer = m_DefaultLayerIndex;
        LayerUtility.SetLayerAllChildren(transform, m_DefaultLayerIndex);

        // 딸에게 문 안보이게 설정
        m_DoorFrame.SetActive(GameManager.Instance.IsLocalPlayerFather);
        m_Exit.SetActive(false);

        m_bIsDaughterClear = false;
        m_bIsFatherClear = false;
        _isOpen = false;
    }

    public void OpenDoor()
    {
        // 딸에게도 문 보이게 설정
        SetActiveExitDoorRPC(true);

        _isOpen = true;

        // 멀리서도 아빠에게 보이게 하기
        ShowExitDoorXRayRPC(GameManager.Instance.FatherPlayerRef);
    }

    private void Exit(NetworkObject player)
    {
        if (!HasStateAuthority)
        {
            return;
        }

        if (player == GameManager.Instance.FatherNetworkObject)
        {
            m_bIsFatherClear = true;
            player.GetComponent<PlayerMovement>().ClearRpc(m_bIsDaughterClear);
        }
        else if (player == GameManager.Instance.DaughterNetworkObject)
        {
            m_bIsDaughterClear = true;
            player.GetComponent<PlayerMovement>().ClearRpc(m_bIsFatherClear);
        }

        GameManager.Instance.ClearEventRpc();

        // 여기에 게임 클리어 처리 하기
        if (m_bIsFatherClear && m_bIsDaughterClear) 
        {
            GameManager.Instance.Clear(nextScene);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !_isOpen)
        {
            return;
        }
        
        if (GameManager.Instance.GetOtherPlayer().GetComponent<Player>()._deathFlag) 
        {
            //UI 띄워서 탈출 못하게 함
            if (UIManager.Instance.ExitBlockUIController == null) 
            {
                UIManager.Instance.EnableExitBlockUI();
            }
                
            StartCoroutine(BlockExit());
            return;
        }
            
        Exit(other.GetComponent<NetworkObject>());
    }

    private IEnumerator BlockExit() 
    {
        UIManager.Instance.ExitBlockUIController.Show();
        yield return new WaitForSeconds(3f);
        UIManager.Instance.ExitBlockUIController.Hide();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void ShowExitDoorXRayRPC([RpcTarget] PlayerRef fatherRef)
    {
        gameObject.layer = m_ExitHighlightLayerIndex;
        LayerUtility.SetLayerAllChildren(transform, m_ExitHighlightLayerIndex);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void SetActiveExitDoorRPC(bool bActive)
    {
        m_DoorFrame.SetActive(bActive);
        m_Exit.SetActive(bActive);
    }
}
