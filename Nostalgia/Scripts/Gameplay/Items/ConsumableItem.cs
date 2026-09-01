using Fusion;
using Nostal.Interfaces;
using UnityEngine;

namespace Item
{
    [RequireComponent(typeof(NetworkObject), typeof(NetworkTransform), typeof(BoxCollider))]
    public class ConsumableItem : NetworkBehaviour, IInteractable
    {
        [Header("Item Scriptable Object")]
        [SerializeField] private ConsumableItemSO m_itemSO;

        [Header("Interactable Prompt Data")] 
        [SerializeField] private InteractPromptData m_interactPromptData;
        private bool interactable = true;

        public void OnInteract(NetworkObject playerObject)
        {
            Player player = playerObject.GetComponent<Player>();

            PlayerInventory playerInventory = player.GetComponent<PlayerInventory>();
            if(playerInventory != null && interactable) {
                if(playerInventory.CanAddItem(m_itemSO)) {
                    playerInventory.AddItem(m_itemSO);
                    DespawnRpc();

                    //아이템 중복 먹어지지 않게 하기
                    SetInteractableRpc();

                }
                else {
                    Debug.Log("아이템을 추가할 수 없습니다.");
                    return;
                }
            }
 
        }

        public InteractPromptData GetInteractPromptData()
        {
            return m_interactPromptData;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void DespawnRpc()
        {
            Runner.Despawn(GetComponent<NetworkObject>());
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void SetInteractableRpc()
        {
            interactable = false;
        }
    }
}