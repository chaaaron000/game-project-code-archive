using Fusion.Addons.FSM;
using UnityEngine;

public class ExpressionlessAlertBehaviour : MobStateBehaviour
{
    private const float TARGET_CHANGE_WAIT_TIME = 5f;
    private const float MAX_ALERT_DURATION = 10f;

    [Header("Events")]
    [SerializeField] private SoundEvent m_soundEvent;

    private Vector3 m_previousSoundPosition = default;
    
    private float m_targetChangeTimer = 0;
    
    protected override void OnEnterState()
    {
        m_mobAI.CurrentState = MobState.Alert;
        m_targetChangeTimer = TARGET_CHANGE_WAIT_TIME;
        SetAnimatorIntRpc("CurrentState", (int)MobState.Alert);
    }
    
    protected override void OnFixedUpdate()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        
        if (Machine.StateTime > MAX_ALERT_DURATION)
        {
            Machine.TryDeactivateState(StateId);
            return;
        }
        
        m_targetChangeTimer += Runner.DeltaTime;
        
        if (m_previousSoundPosition != m_soundEvent.position)
        {
            m_mobAI.SetNavMeshDestination(m_soundEvent.position);
            return;
        }

        m_previousSoundPosition = m_soundEvent.position;
        
        if (m_targetChangeTimer < TARGET_CHANGE_WAIT_TIME || m_mobAI.NavMeshRemainingDistance > 1.0f)
        {
            return;
        }
        
        m_targetChangeTimer = 0;
        m_mobAI.SetNextPatrolPoint();
        SetAnimatorTriggerRpc("Stop");
    }

    protected override void OnExitState()
    {
        m_soundEvent.soundFlag = false;
    }
}