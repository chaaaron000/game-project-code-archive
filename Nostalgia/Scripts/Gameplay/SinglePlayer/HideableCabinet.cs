using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nostal.Single
{
    public class HideableCabinet : Interactable
    {
        [Header("Hideable Cabinet")]
        [Header("Positions")]
        [SerializeField]
        private Transform m_inPosition;

        [SerializeField]
        private Transform m_outPosition;

        [Header("SFX")]
        [SerializeField]
        private AudioSource m_hideSFX;

        public override void Interact(SinglePlayer player)
        {
            Transform target = player.m_bIsHidding ? m_outPosition : m_inPosition;
            player.Movement.Teleport(target);

            if (player.m_bIsHidding)
            {
                player.Movement.m_bCanMove = true;
                player.Movement.m_bUseGravity = true;
                player.Rotation.m_bCanRotate = true;
            }
            else
            {
                player.Movement.m_bCanMove = false;
                player.Movement.m_bUseGravity= false;
                player.Rotation.m_bCanRotate = false;
                player.Rotation.LookForward();

                m_hideSFX.Play();
            }

            player.m_bIsHidding = !player.m_bIsHidding;
        }
    }
}

