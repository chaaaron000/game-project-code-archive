using System.Collections;
using System.Collections.Generic;
using _Scripts.Interfaces;
using UnityEngine;
using Item;
using Fusion;
using Nostal;
using Nostal.Interfaces;

[RequireComponent(typeof(NetworkObject), typeof(NetworkTransform), typeof(BoxCollider))]
public class ChaseItem : NetworkBehaviour, IInteractable, IResettable
{
    [Header("Item Scriptable Object")]
    [SerializeField] private ConsumableItemSO m_itemSO;

    [Header("Interactable Prompt Data")] 
    [SerializeField] private InteractPromptData m_interactPromptData;

    [SerializeField] private GameObject drink;
    [SerializeField] private Collider m_collider;

    public InteractPromptData GetInteractPromptData()
    {
        return m_interactPromptData;
    }

    public void OnInteract(NetworkObject playerObject)
    {
        Player player = playerObject.GetComponent<Player>();

        // Debug.LogWarning("Consumable Item OnInteract");

        PlayerInventory playerInventory = player.GetComponent<PlayerInventory>();
        if (playerInventory != null && playerInventory.CanAddItem(m_itemSO))
        {
            playerInventory.AddItem(m_itemSO);
            DespawnRpc();
        }
    }
    
    public void Reset() 
    {
        // Debug.Log("Reset", this);
        SpawnRpc();
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void SpawnRpc() 
    {
        drink.SetActive(true);
        m_collider.enabled = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void DespawnRpc() 
    {
        drink.SetActive(false);
        m_collider.enabled = false;
    }
}
