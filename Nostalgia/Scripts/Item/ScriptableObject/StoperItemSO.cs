using UnityEngine;

namespace Item
{
    [CreateAssetMenu(fileName = "SO_StoperItem", menuName = "Scriptable Object/Items/Stoper", order = 4)]
    public class StoperItemSO : ConsumableItemSO
    {
        [Header("Stoper Item Config")] 
        [SerializeField] private float m_pauseDuration = 30f;
        
        public override void Use(Player usingPlayer)
        {
            // Debug.LogWarning("스토퍼 아이템 사용");
            GameManager.Instance.PauseDeathTimerRpc(m_pauseDuration);
        }
    }
}