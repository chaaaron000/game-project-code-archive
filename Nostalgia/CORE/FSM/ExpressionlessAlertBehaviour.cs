using Fusion.Addons.FSM;
using UnityEngine;

public class ExpressionlessAlertBehaviour : StateBehaviour
{
    private const float ALERT_MAX_DURATION = 10f;
    
    protected override void OnFixedUpdate()
    {
        if (Machine.StateTime > ALERT_MAX_DURATION)
        {
            Machine.TryDeactivateState(StateId);
        }
    }
    
    protected override void OnEnterStateRender()
    {
        Debug.Log("Alerting ... ");
    }
}