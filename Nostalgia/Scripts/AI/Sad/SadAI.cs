using System.Collections;
using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;

[RequireComponent(typeof(StateMachineController))]
public class SadAI : BaseMob, IStateMachineOwner
{
    private const float MIN_CRYING_INTERVAL = 17f;
    private const float MAX_CRYING_INTERVAL = 19f;
    
    private StateMachine<StateBehaviour> m_sadAI;

    [Header("State")] 
    [SerializeField] private SadIdleBehaviour m_idleState;
    [SerializeField] private SadChaseBehaviour m_chaseState;
    [SerializeField] private SadAttackBehaviour m_attackState;

    [Header("Events")] 
    [SerializeField] private AttackEvent m_attackEvent;

    [Header("Effect")] 
    [SerializeField] private GameObject m_leftFoot;
    [SerializeField] private GameObject m_rightFoot;
    [SerializeField] private GameObject particleObject;
    
    
    private bool m_bIsPlayerFind = false;

    public override void Spawned()
    {
        base.Spawned();

        StartCoroutine(Cry());
        particleObject.SetActive(false);
        
        // 아빠 플레이어에서만 플레이
        if (Runner.LocalPlayer == GameManager.Instance.FatherPlayerRef)
        {
            StartCoroutine(FootParticlePlayCoroutine());
            particleObject.SetActive(true);
        }
    }

    void IStateMachineOwner.CollectStateMachines(List<IStateMachine> stateMachines)
    {
        m_sadAI = new StateMachine<StateBehaviour>(
            "Sad AI", 
            m_idleState, m_chaseState, m_attackState
        );
        
        m_sadAI.SetDefaultState(m_idleState.StateId);
        stateMachines.Add(m_sadAI);
    }
    
    public override void FixedUpdateNetwork()
    {
        // 다른 상태 조건이 없다면 기본적으로 Idle로 돌아감
        m_bIsPlayerFind = Observe();

        if (m_attackEvent.attackFlag)
        {
            m_sadAI.ForceActivateState(m_attackState);
            return;
        }

        if (m_bIsPlayerFind)
        {
            m_chaseState.ResetInvisibilityDuration();
            m_sadAI.ForceActivateState(m_chaseState);
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
            //SoundManager.Instance.SFX_Play_rpc("sadCry1", mobNetworkObject);
            SoundManager.Instance.SFX_Play_dautherOnly_rpc("sadCry1", mobNetworkObject);
        }
    }

    private IEnumerator FootParticlePlayCoroutine()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y - 1.25f, transform.position.z);
        Vector3 leftPosition = pos  - transform.right * 0.8f;
        Vector3 rightPosition = pos + transform.right * 0.8f;

        GameObject  leftFootObject = Instantiate( m_leftFoot,  leftPosition, transform.rotation);
        GameObject rightFootObject = Instantiate(m_rightFoot, rightPosition, transform.rotation);

        float coolTime = 2.5f;
        while (true)
        {
            coolTime = CurrentState switch
            {
                MobState.Idle  => 2.5f,
                MobState.Chase => 1.2f,
                _              => coolTime
            };

            Destroy(leftFootObject);
            
            pos = new Vector3(transform.position.x, transform.position.y - 1.1f, transform.position.z);
            leftPosition = pos - transform.right * 0.8f;
            leftFootObject = Instantiate(m_leftFoot, leftPosition, transform.rotation);
            leftFootObject.SetActive(true);
            
            yield return new WaitForSeconds(coolTime);

            Destroy(rightFootObject);
            
            pos = new Vector3(transform.position.x, transform.position.y - 1.1f, transform.position.z);
            rightPosition = pos + transform.right * 0.8f;
            rightFootObject = Instantiate(m_rightFoot, rightPosition, transform.rotation);
            rightFootObject.SetActive(true);
            
            yield return new WaitForSeconds(coolTime);
        }
    }
}
