using Fusion;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TouchDetection : NetworkBehaviour
{
    [SerializeField] private BaseMob m_mobAI;
    [SerializeField] private AttackEvent m_attackEvent;
    
    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Player") || m_attackEvent.attackFlag ||
            m_mobAI.CurrentState != MobState.Chase)
        {
            return;
        }
        
        Player hitPlayer = other.GetComponent<Player>();
        if (!hitPlayer.isHidden)
        {
            UpdateAttackEventRpc(hitPlayer);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void UpdateAttackEventRpc(Player hitPlayer)
    {
        m_attackEvent.attackFlag = true;
        m_attackEvent.damagedPlayer = hitPlayer;
    }
}