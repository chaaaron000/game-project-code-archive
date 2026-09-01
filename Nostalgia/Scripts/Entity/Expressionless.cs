using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Nostal.Interfaces.State;

public class Expressionless : Mob
{
    private IState CurrentState { get; set; }

    public ExpressionlessIdleState IdleState;
    public ExpressionlessAlertState AlertState;
    public ExpressionlessChaseState ChaseState;
    public ExpressionlessAttackState AttackState;
    
    public AttackEvent _attackEvent;
    public SoundEvent _soundEvent;
    public NetworkObject netObj;
    
    public bool IsPlayerFind { get; private set; }

    private readonly float observingInterval = 0.2f;

    public override void Init()
    {
        attackCoolTime = 5.0f;
        stateSpeed[(int)MobState.Idle] = 0.6f;
        stateSpeed[(int)MobState.Alert] = 1.0f;
        stateSpeed[(int)MobState.Chase] = 1.2f;

        viewRadius = 7.0f;
        viewAngle = 60f;
        
        base.Init();
        
        // State 패턴으로 관리하기 위해 MobFunc를 멈춤.
        StopCoroutine(mobFuncCoroutine);

        // 서버만 우는 소리 실행해서 동기화
        if (!HasStateAuthority) return;
        
        IdleState = new ExpressionlessIdleState(this);
        AlertState = new ExpressionlessAlertState(this);
        ChaseState = new ExpressionlessChaseState(this);
        AttackState = new ExpressionlessAttackState(this);
        
        CurrentState = IdleState;
        IdleState.OnStateEnter();
        
        netObj = GetComponent<NetworkObject>();
        
        StartCoroutine(Cry());
        // StartObserving();
    }
    
    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        
        IsPlayerFind = Observe();
        CurrentState?.OnStateUpdate(Runner.DeltaTime);
    }
    
    public void TransitionTo(IState nextState)
    {
        CurrentState.OnStateExit();
        CurrentState = nextState;
        nextState.OnStateEnter();
    }

    private IEnumerator Cry() 
    {
        float minTime = 6f; // 최소 시간 간격
        float maxTime = 11f; // 최대 시간 간격
        while (true) 
        {
            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);
            
            int cryNum = Random.Range(0, 2);
            if (cryNum == 0)
                SoundManager.Instance.SFX_Play_rpc("expressionlessCry1", netObj);
            else
                SoundManager.Instance.SFX_Play_rpc("expressionlessCry2", netObj);
        }
    }

    public void StartObserving()
    {
        StartCoroutine(ObservingCoroutine());
    }

    public void StopObserving()
    {
        StopCoroutine(ObservingCoroutine());
    }

    private IEnumerator ObservingCoroutine()
    {
        while (true)
        {
            IsPlayerFind = Observe();
            yield return new WaitForSeconds(observingInterval);
        }
    }

    public override IEnumerator IdleFunc() 
    {
        // 공격 이후 잠깐 대기
        if (_attackEvent.attackFlag) {
            yield return StartCoroutine(AttackFunc());
            yield break;
        }

        // 사운드 처리, 이벤트가 있으면 Alert 상태로 전환
        if (_soundEvent.soundFlag) {
            SetDestination(_soundEvent.position);
            SetState(MobState.Alert);
            _soundEvent.soundFlag = false;
            SoundManager.Instance.SFX_Play_rpc("expressionlessAlert", netObj);
            yield break;
        }

        // Navmesh로 다음 목표로 가는 것에 대한 처리
        yield return StartCoroutine(base.IdleFunc());  

        // 시야각에 적 발견 시 Chase 상태로 전환
        if (Observe() == true) {
            SetState(MobState.Chase);
        }
    }

    public override IEnumerator AlertFunc() 
    {
        for (int i=0; i<50; i++) {
            // 공격 처리
            if (_attackEvent.attackFlag) {
                yield return StartCoroutine(AttackFunc());
                yield break;
            }

            // 사운드 처리
            if (_soundEvent.soundFlag) {
                Debug.Log("SoundEvent! " +  _soundEvent.position);
                SetDestination(_soundEvent.position);
                _soundEvent.soundFlag = false;
            }

            // 시야각에 적 발견
            if (Observe() == true) {
                SetState(MobState.Chase);
                yield break;
            }

            float dist = ai.remainingDistance;
            if(dist <= 0.5f) {
                SetAnimatorRpc("AlertIsStopped", true);
                yield return new WaitForSeconds(3.0f);
                SetNextTarget();
                SetState(MobState.Idle);
                SetAnimatorRpc("AlertIsStopped", false);
                yield break;
            }

            Debug.Log(gameObject + "Alerting...");
            yield return new WaitForSeconds(0.2f);
        }
        SetState(MobState.Idle);
    }

    public override IEnumerator ChaseFunc() 
    {
        for (int i=0; i<50; i++) {
            // 공격 처리
            if (_attackEvent.attackFlag) {
                Debug.Log(gameObject + "공격처리");
                yield return StartCoroutine(AttackFunc());
                yield break;
            }
            
            // 추격 처리
            if (hitPlayer != null) {
                if (hitPlayer.isHidden) {
                    Debug.Log("플레이어가 숨었음");
                    SetState(MobState.Alert);
                    SoundManager.Instance.SFX_Play_rpc("expressionlessAlert", netObj);
                    yield break;
                }
                if (hitPlayer._deathFlag) {
                    Debug.Log(gameObject + "추격처리");
                    SetState(MobState.Idle);
                    _soundEvent.soundFlag = false;
                    yield break;
                }
                targetPos = hitPlayer.gameObject.transform.position;
                SetDestination(targetPos);
            }
            else {
                Debug.Log("HitPlayer가 null인데 Chase임");
            }

            // 시야각에 적 발견시 계속 추격(쿨타임 초기화)
            if (Observe() == true) {
                i = 0;
            }

            Debug.Log(gameObject + "Chasing...");
            yield return new WaitForSeconds(0.2f);
        }
        Debug.Log(gameObject + "Chase 끝 처리");
        SetState(MobState.Idle);
    }

    public IEnumerator AttackFunc() 
    {
        SetAnimatorRpc("Attack");
        Attack(_attackEvent.damagedPlayer, 100, (int)mobID.expressionless);

        // expressionlessAttack
        ai.isStopped = true;
        SetNextTarget();
        yield return new WaitForSeconds(attackCoolTime);

        _attackEvent.attackFlag = false;
        SetState(MobState.Idle);
        ai.isStopped = false;
        _soundEvent.soundFlag = false; 
    }

    private void OnTriggerStay(Collider other) 
    {
        // Sound 오브젝트는 클라에서만 생기므로 각 클라에서 감지해서 StateAuthority를 가진 클라에게 RPC를 보낸다.
        if (other.gameObject.CompareTag("Sound")) 
        {
            DetectSoundRpc(other.transform.position);
        }

        // 그 외 충돌 판정은 StateAuthority를 가진 클라만 실행
        if (!HasStateAuthority) return;
        
        if (other.gameObject.CompareTag("Player") && !_attackEvent.attackFlag && nowState == MobState.Chase)
        {
            if (hitPlayer != null && !hitPlayer.isHidden)
            {
                _attackEvent.attackFlag = true;
                _attackEvent.damagedPlayer = other.gameObject.GetComponentInParent<Player>();
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DetectSoundRpc(Vector3 pos) 
    {
        _soundEvent.position = pos;
        _soundEvent.soundFlag = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public override void ResetAnimationTriggerRpc() 
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Alert");
        animator.ResetTrigger("Chase");
        animator.ResetTrigger("Attack");
    }

    public void ApplyDamageToPlayer()
    {
        Attack(_attackEvent.damagedPlayer, 100, (int)mobID.expressionless);
    }
}
