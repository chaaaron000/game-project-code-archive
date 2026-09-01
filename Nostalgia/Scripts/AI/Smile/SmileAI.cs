using System.Collections;
using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;

[RequireComponent(typeof(StateMachineController))]
public class SmileAI : BaseMob, IStateMachineOwner
{
    private const float MIN_CRYING_INTERVAL = 4f;  // 최소 시간 간격
    private const float MAX_CRYING_INTERVAL = 8f; // 최대 시간 간격
    
    private StateMachine<StateBehaviour> m_smileAI;

    [Header("State")] 
    [SerializeField] private SmileIdleBehaviour   m_idleState;
    [SerializeField] private SmileAlertBehaviour  m_alertState;
    [SerializeField] private SmileChaseBehaviour  m_chaseState;
    [SerializeField] private SmileAttackBehaviour m_attackState;
    
    [Header("Events")]
    [SerializeField] private AttackEvent m_attackEvent;
    [SerializeField] private SoundEvent  m_soundEvent;
    
    public bool bIsPlayerHiddenWhileChasing = false;
    private bool m_bIsPlayerFind = false;

    public override void Spawned()
    {
        base.Spawned();
        
        StartCoroutine(Cry());
    }


    void IStateMachineOwner.CollectStateMachines(List<IStateMachine> stateMachines)
    {
        m_smileAI = new StateMachine<StateBehaviour>(
            "Expression AI", 
            m_idleState, m_alertState, m_chaseState, m_attackState
        );
        
        m_smileAI.SetDefaultState(m_idleState.StateId);
        stateMachines.Add(m_smileAI);
    }

    public override void FixedUpdateNetwork()
    {
        // 다른 상태 조건이 없다면 기본적으로 Idle로 돌아감
        m_bIsPlayerFind = Observe();

        if (m_attackEvent.attackFlag)
        {
            m_smileAI.ForceActivateState(m_attackState);
            return;
        }

        if (m_bIsPlayerFind)
        {
            m_chaseState.ResetInvisibilityDuration();
            m_smileAI.ForceActivateState(m_chaseState);
            return;
        }

        if(bIsPlayerHiddenWhileChasing)
        {
            bIsPlayerHiddenWhileChasing = false;
            m_soundEvent.soundFlag = false;
            m_smileAI.TryActivateState(m_idleState);
            return;
        }

        if (m_soundEvent.soundFlag)
        {
            m_smileAI.TryActivateState(m_alertState);
        }
    }

    private IEnumerator Cry()
    {
        //서버만 실행
        if (!HasStateAuthority) yield break;

        while (true)
        {
            float waitTime = Random.Range(MIN_CRYING_INTERVAL, MAX_CRYING_INTERVAL);
            yield return new WaitForSeconds(waitTime);

            float cryNum = Random.Range(0, 2);
            string sfxClipName = cryNum switch
            {
                0 => "smileCry1",
                1 => "smileCry2",
                _ => "smileCry1"
            };

            //float sfxDistance = Runner.LocalPlayer == GameManager.Instance.DaughterPlayerRef ? 25 : 5;

            SoundManager.Instance.SFX_Play_dautherOnly_rpc(sfxClipName, mobNetworkObject);
        }
    }
}
