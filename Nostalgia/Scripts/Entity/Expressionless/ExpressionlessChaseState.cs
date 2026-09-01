using Nostal.Interfaces.State;
using UnityEngine;

public class ExpressionlessChaseState : IState
{
    private Expressionless expressionless;

    private readonly float _maxChaseTime = 10f;
    private float _timer;

    public ExpressionlessChaseState(Expressionless expressionless)
    {
        this.expressionless = expressionless;
    }
    
    public void OnStateEnter()
    {
        Debug.Log("Chase OnStateEnter");
        expressionless.SetAnimatorRpc("Chase");
        expressionless.SetState(MobState.Chase);
        expressionless.SetMobSpeedByNowState();
        
        _timer = _maxChaseTime;
    }

    public void OnStateUpdate(float deltaTime)
    {
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

        Player player = expressionless.GetHitPlayer();
        if (player != null)
        {
            // 캐비넷에 숨은 경우
            if (player.isHidden)
            {
                expressionless.TransitionTo(expressionless.AlertState);
                return;
            }
            
            // 죽은 경우
            if (player._deathFlag)
            {
                expressionless._soundEvent.soundFlag = false;
                expressionless.TransitionTo(expressionless.IdleState);
                return;
            }
        
            expressionless.SetDestination(player.gameObject.transform.position);
        }

        if (expressionless.IsPlayerFind)
        {
            _timer = _maxChaseTime;
        }
        
        _timer -= deltaTime;
    }

    public void OnStateExit()
    {
        Debug.Log("Chase OnStateExit");
    }
}