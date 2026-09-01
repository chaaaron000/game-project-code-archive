using System.Collections.Generic;
using Fusion;
using Nostal.Util;
using UnityEngine;

namespace Nostal.Player
{
    public enum PlayerEffectType
    {
        Teleport,
        Adrenaline
    }
    
    public class PlayerEffects : NetworkBehaviour
    {
        [System.Serializable]
        private struct EffectMap
        {
            public PlayerEffectType Type;
            public EffectController Controller;
        }

        [SerializeField] private List<EffectMap> m_effectBinding;
        
        private Dictionary<PlayerEffectType, EffectController> m_effectMap;

        public override void Spawned()
        {
            base.Spawned();

            m_effectMap = new Dictionary<PlayerEffectType, EffectController>();

            foreach (EffectMap effectMap in m_effectBinding)
            {
                if (!m_effectMap.ContainsKey(effectMap.Type))
                {
                    m_effectMap.Add(effectMap.Type, effectMap.Controller);
                }
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void PlayEffectRPC(PlayerEffectType effectType)
        {
            if (m_effectMap.TryGetValue(effectType, out EffectController controller))
            {
                controller.PlayEffect();
            }
        }
    }
}