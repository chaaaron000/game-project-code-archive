using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Smile : Mob
{
    public AttackEvent _attackEvent;
    public SoundEvent _soundEvent;
    public NetworkObject netObj;
    public override void Init() {      
        attackCoolTime = 5.0f;
        stateSpeed[(int)MobState.Idle] = 0.6f;
        stateSpeed[(int)MobState.Alert] = 1.0f;
        stateSpeed[(int)MobState.Chase] = 2.5f;

        viewRadius = 11.0f;
        viewAngle = 60f;
        base.Init();
        //서버만 우는 소리 실행해서 동기화
        if(!HasStateAuthority) return;
        netObj = GetComponent<NetworkObject>();
        StartCoroutine(Cry());
    }

    public IEnumerator Cry() {
        float minTime = 7f; // 최소 시간 간격
        float maxTime = 14f; // 최대 시간 간격
        while(true) {
            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);
            float cryNum = Random.Range(0, 2);
            if(cryNum == 0){
                if(GameManager.Instance.GetLocalPlayer() == GameManager.Instance.DaughterNetworkObject) {
                    //딸이면 웃는 소리 증폭
                    SoundManager.Instance.SFX_Play_rpc("smileCry1", netObj, 5);
                    SoundManager.Instance.Set_SFX_LocalDistance("smileCry1", gameObject, 25);
                }else if(GameManager.Instance.GetLocalPlayer() == GameManager.Instance.FatherNetworkObject){
                    //아빠면 웃는 소리 줄이기
                    SoundManager.Instance.SFX_Play_rpc("smileCry1", netObj, 5);
                    SoundManager.Instance.Set_SFX_LocalDistance("smileCry1", gameObject, 25);
                }
            } 
            else if(cryNum == 1){
                if(GameManager.Instance.GetLocalPlayer() == GameManager.Instance.DaughterNetworkObject) {
                    //딸이면 웃는 소리 증폭
                    SoundManager.Instance.SFX_Play_rpc("smileCry2", netObj, 25);
                    SoundManager.Instance.Set_SFX_LocalDistance("smileCry2", gameObject, 5);
                } else if(GameManager.Instance.GetLocalPlayer() == GameManager.Instance.FatherNetworkObject){
                    //아빠면 웃는 소리 줄이기
                    SoundManager.Instance.SFX_Play_rpc("smileCry2", netObj, 25);
                    SoundManager.Instance.Set_SFX_LocalDistance("smileCry2", gameObject, 5);
                }
            }
        }
    }
    public override IEnumerator IdleFunc() {
        //공격 이후 잠깐 대기
        if(_attackEvent.attackFlag) {
            yield return StartCoroutine(AttackFunc());
            yield break;
        }

        //사운드 처리
        if(_soundEvent.soundFlag) {
            SetDestination(_soundEvent.position);
            SetState(MobState.Alert);
            _soundEvent.soundFlag = false;
            yield break;
        }

        //거리 처리
        yield return StartCoroutine(base.IdleFunc());  

        //시야각에 적 발견
        if(Observe() == true) {
            SetState(MobState.Chase);
        }
    }

    public override IEnumerator AlertFunc() {
        for(int i=0; i<50; i++) {
            //공격 처리
            if(_attackEvent.attackFlag) {
                yield return StartCoroutine(AttackFunc());
                yield break;
            }

            //사운드 처리
            if(_soundEvent.soundFlag) {
                SetDestination(_soundEvent.position);
                _soundEvent.soundFlag = false;
            }

            //시야각에 적 발견
            if(Observe() == true) {
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
            yield return new WaitForSeconds(0.2f);
        }
        SetState(MobState.Idle);
    }

    public override IEnumerator ChaseFunc() {
        /*
            StartBGM();
        */

        for(int i=0; i<20; i++) {
            //공격 처리
            if(_attackEvent.attackFlag) {
                Debug.Log(gameObject + "공격처리");
                yield return StartCoroutine(AttackFunc());
                yield break;
            }
            
            //추격 처리
            if(hitPlayer != null) {
                if(hitPlayer._deathFlag) {
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

            //시야각에 적 발견시 계속 추격(쿨타임 초기화)
            if(Observe() == true) {
                i = 0;
            }
            SetAnimatorRpc("SprintStop");
            yield return new WaitForSeconds(0.5f);
            SetAnimatorRpc("SprintGo");
        }
        SetState(MobState.Idle);
    }

    public IEnumerator AttackFunc() {
        SetAnimatorRpc("Attack");
        //smileAttack
        Attack(_attackEvent.damagedPlayer, 100, (int)mobID.smile);
        ai.isStopped = true;
        SetNextTarget();
        yield return new WaitForSeconds(attackCoolTime);
        _attackEvent.attackFlag = false;
        SetState(MobState.Idle);
        ai.isStopped = false;
        _soundEvent.soundFlag = false; 
    }

    private void OnTriggerStay(Collider other) {
        //Sound 오브젝트는 클라에서만 생기므로 각 클라에서 감지해서 StateAuthority를 가진 클라에게 RPC를 보낸다.
        if(other.gameObject.CompareTag("Sound")) {
            Debug.Log("Sound!");
            DetectSoundRpc(other.transform.position);
        }

        //그 외 충돌 판정은 StateAuthority를 가진 클라만 실행
        if(!HasStateAuthority) return;
        if(other.gameObject.CompareTag("Player") && _attackEvent.attackFlag == false) {
            if(hitPlayer != null) {
                if(hitPlayer.isHidden == false) {
                    Debug.Log(other.gameObject + "Catch!");
                    _attackEvent.attackFlag = true;
                    _attackEvent.damagedPlayer = other.gameObject.GetComponentInParent<Player>();
                }
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DetectSoundRpc(Vector3 pos) {
        _soundEvent.position = pos;
        _soundEvent.soundFlag = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public override void ResetAnimationTriggerRpc() {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Alert");
        animator.ResetTrigger("Chase");
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("SprintGo");
        animator.ResetTrigger("SprintStop");
    }
}
