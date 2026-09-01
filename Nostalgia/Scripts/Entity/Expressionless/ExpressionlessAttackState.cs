using Nostal.Interfaces.State;
using UnityEngine;

public class ExpressionlessAttackState : IState
{
    private Expressionless expressionless;
    
    private float _timer;

    public ExpressionlessAttackState(Expressionless expressionless)
    {
        this.expressionless = expressionless;
    }
    
    public void OnStateEnter()
    {
        Debug.Log("Attack OnStateEnter");
        expressionless.SetAnimatorRpc("Attack");
        expressionless.ApplyDamageToPlayer();
        expressionless.SetNavMeshIsStopped(true);
        expressionless.SetNextTarget();

        _timer = expressionless.AttackCoolTime;
    }

    public void OnStateUpdate(float deltaTime)
    {
        if (_timer < 0f)
        {
            expressionless._attackEvent.attackFlag = false;
            expressionless.SetNavMeshIsStopped(false);
            expressionless._soundEvent.soundFlag = false;
            expressionless.TransitionTo(expressionless.IdleState);
            return;
        }
        
        _timer -= deltaTime;
    }

    public void OnStateExit()
    {
        Debug.Log("Attack OnStateExit");
    }
}