using UnityEngine;

public class SmileAttackBehaviour : MobStateBehaviour
{
    [Header("Events")]
    [SerializeField] private AttackEvent m_attackEvent;
    
    protected override void OnEnterState()
    {
        SetAnimatorTriggerRpc("Attack");
        m_mobAI.Attack(m_attackEvent.damagedPlayer, m_mobAI.AttackDamage, (int)Entity.mobID.smile);
        m_mobAI.SetNavMeshIsStopped(true);
    }
    
    protected override void OnFixedUpdate()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        
        if (Machine.StateTime > m_mobAI.AttackCoolTime)
        {
            Machine.TryDeactivateState(StateId);
        }
    }

    protected override void OnExitState()
    {
        m_attackEvent.attackFlag = false;
        m_mobAI.SetNavMeshIsStopped(false);
    }
}
