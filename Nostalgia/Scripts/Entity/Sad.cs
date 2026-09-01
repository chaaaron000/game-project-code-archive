using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.AI;

public class Sad : Mob
{
    private PlayerRef _fatherPlayerRef;
    public AttackEvent _attackEvent;

    public GameObject leftFoot;
    public GameObject rightFoot;
    public NetworkObject netObj;
    public override void Init() {
        ParticlePlay();

        attackCoolTime = 4.0f;
        stateSpeed[(int)MobState.Idle] = 0.7f;
        //사람 속도보다 살짝 느리게 바꾸기
        stateSpeed[(int)MobState.Chase] = 1.3f;
        viewRadius = 5.0f;
        viewAngle = 70f;
        base.Init();

        //서버만 우는 소리 실행해서 동기화
        if(!HasStateAuthority) return;
        netObj = GetComponent<NetworkObject>();
        StartCoroutine(Cry());
    }

    public IEnumerator Cry() {
        float minTime = 17f; // 최소 시간 간격
        float maxTime = 19f; // 최대 시간 간격
        while(true) {
            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);
            SoundManager.Instance.SFX_Play_rpc("sadCry1", netObj);
        }
    }

    public void ParticlePlay() {
        PlayerRef localPlayerRef = Runner.LocalPlayer;
        _fatherPlayerRef = GameManager.Instance.FatherPlayerRef;

        if (localPlayerRef == _fatherPlayerRef) {
            StartCoroutine(FootParticlePlay());
        } 
    }

    public IEnumerator FootParticlePlay() {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y - 1.0f, transform.position.z);
        Vector3 leftPosition = pos - transform.right * 0.8f;
        Vector3 rightPosition = pos + transform.right * 0.8f;
        

        GameObject leftFootObject = Instantiate(leftFoot, leftPosition, transform.rotation);
        GameObject rightFootObject = Instantiate(rightFoot, rightPosition, transform.rotation);

        float coolTime = 2.5f;
        while(true) {
            if(nowState == MobState.Idle) {
                coolTime = 2.5f;
            }
            else if(nowState == MobState.Chase) {
                coolTime = 1.2f;
            }

            Destroy(leftFootObject);
            pos = new Vector3(transform.position.x, transform.position.y - 0.93f, transform.position.z);
            leftPosition = pos - transform.right * 0.8f;
            leftFootObject = Instantiate(leftFoot, leftPosition, transform.rotation);
            leftFootObject.SetActive(true);
            yield return new WaitForSeconds(coolTime);

            Destroy(rightFootObject);
            pos = new Vector3(transform.position.x, transform.position.y - 0.93f, transform.position.z);
            rightPosition = pos + transform.right * 0.8f;
            rightFootObject = Instantiate(rightFoot, rightPosition, transform.rotation);
            rightFootObject.SetActive(true);
            yield return new WaitForSeconds(coolTime);
        }
    }

    public override IEnumerator IdleFunc() {
        //공격 이후 잠깐 대기
        if(_attackEvent.attackFlag) {
            yield return StartCoroutine(AttackFunc());
            yield break;
        }

        //Navmesh로 다음 목표로 가는 것에 대한 처리
        yield return StartCoroutine(base.IdleFunc());  

        //시야각에 적 발견 시 Chase 상태로 전환
        if(Observe() == true) {
            SetState(MobState.Chase);
        }
    }

    public override IEnumerator ChaseFunc() {
        /*
            StartBGM();
        */
        for(int i=0; i<75; i++) {
            //공격 처리
            if(_attackEvent.attackFlag) {
                Debug.Log(gameObject + "공격처리");
                yield return StartCoroutine(AttackFunc());
                yield break;
            }

            //추격 처리
            targetPos = hitPlayer.transform.position;
            SetDestination(targetPos);

            //시야각에 적 발견시 추격 쿨타임 초기화
            if(Observe() == true) {
                i = 0;
            }
            yield return new WaitForSeconds(0.2f);
        }
        SetState(MobState.Idle);
        Debug.Log("Idle Mode On");
    }

    public IEnumerator AttackFunc() {
        SetAnimatorRpc("Attack");
        Attack(_attackEvent.damagedPlayer, 100, (int)mobID.sad);

        //sadAttack
        ai.isStopped = true;
        SetNextTarget();
        yield return new WaitForSeconds(attackCoolTime);

        _attackEvent.attackFlag = false;
        SetState(MobState.Idle);
        ai.isStopped = false;
    }

    private void OnTriggerStay(Collider other) {
        if(!HasStateAuthority) return;
        if(other.gameObject.CompareTag("Player") && _attackEvent.attackFlag == false) {
            if(hitPlayer != null) {
                if(hitPlayer.isHidden == false) {
                    _attackEvent.damagedPlayer = other.gameObject.GetComponentInParent<Player>();
                    _attackEvent.attackFlag = true;
                }
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public override void ResetAnimationTriggerRpc() {

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public new void SetAnimatorRpc(string name) {

    }
}
