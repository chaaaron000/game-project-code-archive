using UnityEngine;
using Nostal.Interfaces.State;

public class ExpressionlessIdleState : IState
{
    private Expressionless expressionless;

    private readonly float _animationWaitTime = 2.5f;
    
    private bool _startAnimation = false;
    private float _timer;

    public ExpressionlessIdleState(Expressionless expressionless)
    {
        this.expressionless = expressionless;
    }
    
    public void OnStateEnter()
    {
        Debug.Log("Idle OnStateEnter");
        expressionless.SetAnimatorRpc("Idle");
        expressionless.SetState(MobState.Idle);
        expressionless.SetMobSpeedByNowState();
        expressionless.SetNextTarget();
        expressionless.ClearHitPlayer();
        
        _startAnimation = false;
        expressionless._soundEvent.soundFlag = false;
    }

    public void OnStateUpdate(float deltaTime)
    {
        if (expressionless._attackEvent.attackFlag)
        {
            expressionless.TransitionTo(expressionless.AttackState);
            return;
        }
        
        if (expressionless._soundEvent.soundFlag)
        {
            expressionless.TransitionTo(expressionless.AlertState);
            return;
        }
        
        if (expressionless.IsPlayerFind)
        {
            expressionless.TransitionTo(expressionless.ChaseState);
            return;
        }

        Patrolling();

        if (_startAnimation)
        {
            _timer -= deltaTime;
        }
    }

    public void OnStateExit()
    {
        Debug.Log("Idle OnStateExit");

        if (!expressionless._soundEvent.soundFlag)
        {
            return;
        }
        
        expressionless.SetDestination(expressionless._soundEvent.position);
        expressionless._soundEvent.soundFlag = false;
        SoundManager.Instance.SFX_Play_rpc("expressionlessAlert", expressionless.netObj);
    }

    private void Patrolling()
    {
        if (_startAnimation)
        {
            if (_timer > 0f)
            {
                return;
            }
            
            expressionless.SetAnimatorRpc("IdleIsStopped", false);
            _startAnimation = false;
        }

        if (expressionless.NavMeshRemainingDistance > 1.0f)
        {
            return;
        }
        
        expressionless.SetNextTarget();
        expressionless.SetAnimatorRpc("IdleIsStopped", true);
            
        _startAnimation = true;
        _timer = _animationWaitTime;
    }
}