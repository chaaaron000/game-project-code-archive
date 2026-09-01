using UnityEngine;
using UnityEngine.Localization;

namespace Nostal.Interfaces
{
    [System.Serializable]
    public struct InteractPromptData
    {
        public LocalizedString PromptText;
        public Sprite KeySprite;
    }
    
    public interface IInteractable
    {
        void OnInteract(Fusion.NetworkObject playerObject);
        
        InteractPromptData GetInteractPromptData();
    }
}
