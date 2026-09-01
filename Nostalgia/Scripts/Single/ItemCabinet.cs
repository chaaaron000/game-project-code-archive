using UnityEngine;

namespace Nostal.Single
{
    public class ItemCabinet : Interactable
    {
        [Header("Item Cabinet")]
        [SerializeField]
        private Collider m_collider;

        [SerializeField]
        private AudioSource m_openSFX;

        [SerializeField]
        private Animator m_animator;

        private readonly static string OPEN_ANIMATION_NAME = "ItemCabinetAnimation";

        private void OnEnable()
        {
            m_collider.enabled = true;
        }

        public override void Interact(SinglePlayer player)
        {
            m_collider.enabled = false;

            m_animator.Play(OPEN_ANIMATION_NAME, 0, 0f);
            m_openSFX.Play();
        }
    }
}
