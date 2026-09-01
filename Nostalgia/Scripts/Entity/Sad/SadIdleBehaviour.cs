using UnityEngine;
using UnityEngine.AI;

public class SadIdleBehaviour : MobStateBehaviour
{
    private const float TARGET_CHANGE_WAIT_TIME = 5f;
    
    [Header("Nav Mesh Agent")] 
    [SerializeField] private NavMeshAgent m_navMeshAgent;

    private float m_targetChangeTimer = 0;
    
    protected override void OnEnterState()
    {
        m_mobAI.CurrentState = MobState.Idle;
        m_targetChangeTimer = TARGET_CHANGE_WAIT_TIME;
    }
    
    protected override void OnFixedUpdate()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        
        Patrolling();
    }
    
    private void Patrolling()
    {
        m_targetChangeTimer += Runner.DeltaTime;
        
        if (m_targetChangeTimer < TARGET_CHANGE_WAIT_TIME || m_mobAI.NavMeshRemainingDistance > 1.0f)
        {
            return;
        }
        
        m_targetChangeTimer = 0;
        m_mobAI.SetNextPatrolPoint();
    }
}
