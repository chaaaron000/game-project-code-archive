using System.Collections;
using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;

public class ExpressionlessAttackBehaviour : StateBehaviour
{
    [Header("Events")]
    [SerializeField] private AttackEvent mAttackEvent;
    
    private const float ATTACK_COOL_TIME = 5f;
    
    protected override void OnFixedUpdate()
    {
        if (Machine.StateTime > ATTACK_COOL_TIME)
        {
            Machine.TryDeactivateState(StateId);
        }
    }

    protected override void OnEnterStateRender()
    {
        Debug.Log("Attacking ... ");
    }

    protected override void OnExitState()
    {
        mAttackEvent.attackFlag = false;
    }
}
