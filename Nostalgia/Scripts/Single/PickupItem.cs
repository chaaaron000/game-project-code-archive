using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nostal.Single
{
    public class PickupItem : Interactable
    {
        [Header("Pickup Item")]
        [SerializeField]
        private Collider m_collider;

        public override void Interact(SinglePlayer player)
        {
            // TODO: 아이템 인벤토리에 넣기

            m_collider.enabled = false;
            Destroy(gameObject);
        }
    }
}

