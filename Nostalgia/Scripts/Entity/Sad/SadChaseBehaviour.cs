using Fusion.Addons.FSM;

public class SadChaseBehaviour : MobStateBehaviour
{
    private const float MAX_CHASE_DURATION = 10f;

    private Player m_currentTargetPlayer;
    private float mInvisibilityDuration = 0f;
    
    public void ResetInvisibilityDuration()
    {
        mInvisibilityDuration = 0f;
    }
    
    protected override bool CanExitState(StateBehaviour nextState)
    {
        return mInvisibilityDuration > MAX_CHASE_DURATION;
    }
    
    protected override void OnEnterState()
    {
        m_mobAI.CurrentState = MobState.Chase;
        m_currentTargetPlayer = m_mobAI.TargetPlayer;
        m_currentTargetPlayer.ChasedRpc();
    }
    
    protected override void OnFixedUpdate()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        if (m_mobAI.TargetPlayer != m_currentTargetPlayer)
        {
            m_currentTargetPlayer.StopChasedRpc();
            m_currentTargetPlayer = m_mobAI.TargetPlayer;
            m_currentTargetPlayer.ChasedRpc();
        }

        if (mInvisibilityDuration > MAX_CHASE_DURATION || 
            m_mobAI.TargetPlayer.isHidden              ||
            m_mobAI.TargetPlayer._deathFlag)
        {
            Machine.ForceDeactivateState(StateId);
        }

        if (m_mobAI.TargetPlayer != null)
        {
            m_mobAI.SetNavMeshDestination(m_mobAI.TargetPlayer.transform.position);
        }
        
        mInvisibilityDuration += Runner.DeltaTime;
    }

    protected override void OnExitState()
    {
        base.OnExitState();
        
        m_currentTargetPlayer.StopChasedRpc();
    }
}
