using Nostal.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nostal.Single
{
    public class Door : Interactable
    {
        [Header("Door")]
        [Header("Animator")]
        [SerializeField]
        private Animator m_animator;

        [Header("SFX")]
        [SerializeField]
        private AudioSource m_openSFX;

        [SerializeField]
        private AudioSource m_closeSFX;

        [Header("Interact Hint")]
        [SerializeField]
        private InteractPromptData m_closePrompt;

        [SerializeField]
        private InteractPromptData m_openPrompt;

        private readonly static string PARAM_NAME = "Speed";
        private readonly static string STATE_NAME = "DoorAnimation";
        private bool m_bIsOpen;

        private void OnEnable()
        {
            m_bIsOpen = false;
        }

        public override void Interact(SinglePlayer player)
        {
            float param = m_bIsOpen ? -1.0f : 1.0f;
            float normalizedTime = m_bIsOpen ? 1f : 0f;

            m_animator.SetFloat(PARAM_NAME, param);
            m_animator.Play(STATE_NAME, 0, normalizedTime);

            if (m_bIsOpen)
            {
                m_closeSFX.Play();
            }
            else
            {
                m_openSFX.Play();
            }

            m_bIsOpen = !m_bIsOpen;
        }
    }
}
