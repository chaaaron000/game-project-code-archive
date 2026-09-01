using System;
using System.Collections;
using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;

[RequireComponent(typeof(StateMachineController))]
public class ExpressionlessAI : Mob, IStateMachineOwner
{
    private StateMachine<StateBehaviour> mExpressionAI;

    [Header("State")]
    [SerializeField] private ExpressionlessIdleBehaviour mIdleState;
    [SerializeField] private ExpressionlessAlertBehaviour mAlertState;
    [SerializeField] private ExpressionlessChaseBehaviour mChaseState;
    [SerializeField] private ExpressionlessAttackBehaviour mAttackState;

    [Header("Events")]
    [SerializeField] private AttackEvent mAttackEvent;
    [SerializeField] private SoundEvent mSoundEvent;
    
    public bool IsPlayerFind { get; private set; }

    public override void Init()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        
        attackCoolTime = 5.0f;
        stateSpeed[(int)State.Idle] = 0.6f;
        stateSpeed[(int)State.Alert] = 1.0f;
        stateSpeed[(int)State.Chase] = 1.2f;

        viewRadius = 7.0f;
        viewAngle = 60f;
        
        base.Init();
        StopCoroutine(mobFuncCoroutine);
    }

    void Fusion.Addons.FSM.IStateMachineOwner.CollectStateMachines(List<IStateMachine> stateMachines)
    {
        mExpressionAI = new StateMachine<StateBehaviour>(
            "Expression AI", 
            mIdleState, mAlertState, mChaseState, mAttackState
        );
        
        mExpressionAI.SetDefaultState(mIdleState.StateId);
        stateMachines.Add(mExpressionAI);
    }

    public override void FixedUpdateNetwork()
    {
        IsPlayerFind = Observe();

        if (mAttackEvent.attackFlag)
        {
            mExpressionAI.ForceActivateState(mAttackState);
            return;
        }

        if (IsPlayerFind)
        {
            mChaseState.ChasingTime = 0f;
            mExpressionAI.ForceActivateState(mChaseState);
            return;
        }

        if (mSoundEvent.soundFlag)
        {
            mExpressionAI.TryActivateState(mAlertState);
        }
    }
}
