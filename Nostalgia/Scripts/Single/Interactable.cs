using Nostal.Interfaces;
using UnityEngine;

namespace Nostal.Single
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class Interactable : MonoBehaviour
    {
        [Header("Interactable")]
        [SerializeField]
        protected InteractPromptData m_interactPromptData;

        public abstract void Interact(SinglePlayer player);

		public virtual InteractPromptData GetInteractHint()
		{
			return m_interactPromptData;
		}
    }
}
