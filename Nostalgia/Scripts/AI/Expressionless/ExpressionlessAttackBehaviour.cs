using System.Collections;
using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;
using UnityEngine.Serialization;

public class ExpressionlessAttackBehaviour : MobStateBehaviour
{
    [Header("Events")]
    [SerializeField] private AttackEvent m_attackEvent;
    
    protected override void OnEnterState()
    {
        SetAnimatorTriggerRpc("Attack");
        m_mobAI.Attack(m_attackEvent.damagedPlayer, m_mobAI.AttackDamage, (int)Entity.mobID.expressionless);
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
