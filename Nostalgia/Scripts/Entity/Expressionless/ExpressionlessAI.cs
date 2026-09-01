using System.Collections;
using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;

[RequireComponent(typeof(StateMachineController))]
public class ExpressionlessAI : BaseMob, IStateMachineOwner
{
    private const float MIN_CRYING_INTERVAL = 6f;   // 최소 시간 간격
    private const float MAX_CRYING_INTERVAL = 11f;  // 최대 시간 간격
    
    private StateMachine<StateBehaviour> _expressionAI;
    
    [Header("State")]
    [SerializeField] private ExpressionlessIdleBehaviour _idleState;
    [SerializeField] private ExpressionlessAlertBehaviour _alertState;
    [SerializeField] private ExpressionlessChaseBehaviour _chaseState;
    [SerializeField] private ExpressionlessAttackBehaviour _attackState;
    
    [Header("Events")]
    [SerializeField] private AttackEvent _attackEvent;
    [SerializeField] private SoundEvent _soundEvent;

    private bool m_bIsPlayerFind = false;
    public bool bIsPlayerHiddenWhileChasing = false;

    public override void Spawned()
    {
        base.Spawned();

        StartCoroutine(Cry());
    }

    void IStateMachineOwner.CollectStateMachines(List<IStateMachine> stateMachines)
    {
        _expressionAI = new StateMachine<StateBehaviour>(
            "Expression AI", 
            _idleState, _alertState, _chaseState, _attackState
        );
        
        _expressionAI.SetDefaultState(_idleState.StateId);
        stateMachines.Add(_expressionAI);
    }
    
    public override void FixedUpdateNetwork()
    {
        // 다른 상태 조건이 없다면 기본적으로 Idle로 돌아감
        m_bIsPlayerFind = Observe();

        if (_attackEvent.attackFlag)
        {
            _expressionAI.ForceActivateState(_attackState);
            return;
        }

        if (m_bIsPlayerFind)
        {
            _chaseState.ResetInvisibilityDuration();
            _expressionAI.ForceActivateState(_chaseState);
            return;
        }

        if (_soundEvent.soundFlag || bIsPlayerHiddenWhileChasing)
        {
            bIsPlayerHiddenWhileChasing = false;
            _expressionAI.TryActivateState(_alertState);
        }
    }
    
    private IEnumerator Cry()
    {
        while (true) 
        {
            float waitTime = Random.Range(MIN_CRYING_INTERVAL, MAX_CRYING_INTERVAL);
            yield return new WaitForSeconds(waitTime);
            
            int cryNum = Random.Range(0, 2);
            SoundManager.Instance.SFX_Play_rpc(cryNum == 0 ? "expressionlessCry1" : "expressionlessCry2", mobNetworkObject);
        }
    }
}
