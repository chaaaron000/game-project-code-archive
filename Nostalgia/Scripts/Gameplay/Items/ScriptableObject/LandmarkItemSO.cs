using Fusion;
using UnityEngine;

namespace Item
{
    [CreateAssetMenu(fileName = "SO_LandmarkItem", menuName = "Scriptable Object/Items/Landmark", order = 3)]
    public class LandmarkItemSO : ConsumableItemSO
    {
        [Header("Landmark Item Config")]
        [SerializeField] private NetworkPrefabRef m_networkPrefabRef;

        public override void Use(Player usingPlayer)
        {
            // Debug.LogWarning("랜드마크 아이템 사용");

            Vector3    position = usingPlayer.gameObject.transform.position;
            Quaternion rotation = usingPlayer.gameObject.transform.rotation;

            SoundManager.Instance.SFX_Play_rpc(
                "usingLandmark",
                usingPlayer.GetComponent<NetworkObject>()
                );
            
            GameManager.Instance.SpawnNetworkObjectRpc(m_networkPrefabRef, position, rotation);
        }
    }
}