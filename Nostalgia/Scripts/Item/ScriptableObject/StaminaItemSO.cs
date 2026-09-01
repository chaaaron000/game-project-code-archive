using UnityEngine;

namespace Item
{
    [CreateAssetMenu(fileName = "SO_StaminaItem", menuName = "Scriptable Object/Items/Stamina", order = 0)]
    public class StaminaItemSO : ConsumableItemSO
    {
        public override void Use(Player usePlayer)
        {
            SoundManager.Instance.SFX_Play("usingStamina");
            usePlayer.RefillStamina();
        }
    }
}