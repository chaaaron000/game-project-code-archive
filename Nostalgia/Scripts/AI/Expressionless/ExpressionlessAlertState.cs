using Nostal.Interfaces.State;
using UnityEngine;

public class ExpressionlessAlertState : IState
{
    private Expressionless expressionless;

    private readonly float arrivalThreshold = 1f;
    private readonly float _maxAlertTime = 10f;
    private readonly float _alertAnimationWaitTime = 3f;
    private float _timer;
    private float _alertAnimationTimer;
    private bool _isArrived;

    public ExpressionlessAlertState(Expressionless expressionless)
    {
        this.expressionless = expressionless;
    }
    
    public void OnStateEnter()
    {
        Debug.Log("Alert OnStateEnter");
        SoundManager.Instance.SFX_Play_rpc("expressionlessAlert", expressionless.netObj);
        expressionless.SetAnimatorRpc("Alert");
        expressionless.SetState(MobState.Alert);
        expressionless.SetMobSpeedByNowState();
        expressionless.ClearHitPlayer();
        
        _timer = _maxAlertTime;
        _isArrived = false;
    }

    public void OnStateUpdate(float deltaTime)
    {
        // 10초 이상 지났을 때
        if (_timer < 0f)
        {
            expressionless.TransitionTo(expressionless.IdleState);
            return;
        }
        
        if (expressionless._attackEvent.attackFlag)
        {
            expressionless.TransitionTo(expressionless.AttackState);
            return;
        }
        
        if (expressionless.IsPlayerFind)
        {
            expressionless.TransitionTo(expressionless.ChaseState);
            return;
        }
        
        if (expressionless._soundEvent.soundFlag) 
        {
            expressionless.SetDestination(expressionless._soundEvent.position);
            expressionless._soundEvent.soundFlag = false;
        }
        
        // 이미 애니메이션 대기 중일 경우
        if (_isArrived)
        {
            _alertAnimationTimer -= deltaTime;
            
            if (_alertAnimationTimer > 0f)
            {
                return;
            }
            
            expressionless.SetAnimatorRpc("AlertIsStopped", false);
            expressionless.TransitionTo(expressionless.IdleState);
            return;
        }

        if (expressionless.NavMeshRemainingDistance <= arrivalThreshold)
        {
            expressionless.SetAnimatorRpc("AlertIsStopped", true);
            _isArrived = true;
            _alertAnimationTimer = _alertAnimationWaitTime;
            return;
        }
        
        _timer -= deltaTime;
        // Debug.Log(_timer);
    }

    public void OnStateExit()
    {
        Debug.Log("Alert OnStateExit");
    }
}