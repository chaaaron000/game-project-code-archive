using Fusion.Addons.FSM;
using UnityEngine;
using UnityEngine.Serialization;

public class ExpressionlessChaseBehaviour : StateBehaviour
{
    private const float CHASE_MAX_DURATION = 10f;

    public float ChasingTime;
    
    protected override bool CanExitState(StateBehaviour nextState)
    {
        return ChasingTime > CHASE_MAX_DURATION;
    }
    
    protected override void OnFixedUpdate()
    {
        if (ChasingTime > CHASE_MAX_DURATION)
        {
            Machine.TryDeactivateState(StateId);
        }
        
        ChasingTime += Runner.DeltaTime;
    }
    
    protected override void OnEnterStateRender()
    {
        Debug.Log("Chasing ... ");
    }
}