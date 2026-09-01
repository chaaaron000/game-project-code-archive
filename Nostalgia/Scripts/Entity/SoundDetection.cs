using Fusion;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SoundDetection : NetworkBehaviour
{
    [SerializeField] private SoundEvent m_soundEvent;
    
    private void OnTriggerStay(Collider other) 
    {
        // Sound 오브젝트는 클라에서만 생기므로 각 클라에서 감지해서 StateAuthority를 가진 클라에게 RPC를 보낸다.
        if (other.gameObject.CompareTag("Sound")) 
        {
            SetSoundEventRpc(other.transform.position);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void SetSoundEventRpc(Vector3 detectPosition)
    {
        m_soundEvent.soundFlag = true;
        m_soundEvent.position = detectPosition;
    }
    
}
