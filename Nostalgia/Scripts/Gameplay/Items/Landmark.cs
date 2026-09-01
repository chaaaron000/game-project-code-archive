using Fusion;
using UnityEngine;

namespace Item
{
    public class Landmark : NetworkBehaviour
    {
        [SerializeField] private ParticleSystem m_particleSystem;

        public override void Spawned()
        {
            base.Spawned();
            
            m_particleSystem.Play();
            
            // TODO: 이거 Father한테만 보이게 하나?
        }
    }
}