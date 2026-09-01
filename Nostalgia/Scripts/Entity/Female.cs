using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.AI;

public class Female : Mob
{
    public AttackEvent _attackEvent;
    public SoundEvent _soundEvent;
    public NetworkObject netObj;
    public Player targetPlayer;
    public Player[] detectedPlayers;
    private bool despawnTrigger = false;
    public int _spawnPositionIndex;
    [SerializeField] private GameObject bloodParticlePrefab;

    public override void Init() {  
        //시야각 관련 초기 설정
        hitLists = new Collider[3];
        playerLayer = 1<<3;
        animator = GetComponent<Animator>();

        //이제부터 몹 알고리즘 관련 내용. StateAuthority만 적용
        if(!HasStateAuthority) return;
        netObj = GetComponent<NetworkObject>();

        StartCoroutine(InitCoroutine());
    }

    public IEnumerator InitCoroutine() {
        //등장하자마자 추격 상태로 들어가도록 (소리 재생) 설정
        detectedPlayers = DetectPlayers();
        foreach(Player player in detectedPlayers) {
            player.ChasedRpc();
        }
        //targetPlayer.ChasedRpc();

        while(true) {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Vent To Crawl"))
            {
                Debug.Log("female Crawling");
                transform.Translate(Vector3.forward * 0.1f);
                yield return new WaitForSeconds(0.1f);

            }
            else {
                break;
            }
        }

        //NavMesh 초기 설정
        ai = GetComponent<NavMeshAgent>();
        ai.enabled = true;
        
        speed = stateSpeed[2];
        ai.speed = speed;

        Debug.Log("targetPlayer + ChasedRpc");
        SoundManager.Instance.SFX_loop_Play_rpc("femaleScream", netObj);
        targetPos = targetPlayer.gameObject.transform.position;

        SetDestination(targetPos);

        nowState = MobState.Chase;

        StartCoroutine(MobFunc());
    }

    public Player[] DetectPlayers() {
        //플레이어 감지
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10.0f, playerLayer);
        List<Player> players = new List<Player>();

        foreach (Collider collider in hitColliders) {
            // TryGetComponent() 호출
            Player player = collider.TryGetComponent<Player>(out Player p) ? p : null;
            if (player != null && !players.Contains(player)) {
                players.Add(player);  // 중복된 Player는 제외
            }
        }

        return players.ToArray();
    }

    public override void FixedUpdateNetwork() {
        if(despawnTrigger) {
            Runner.Despawn(gameObject.GetComponent<NetworkObject>());
        }
    }

    public IEnumerator Disappear() {
        if(!HasStateAuthority) yield break;

        //소리 중지
        SoundManager.Instance.SFX_loop_Stop_rpc("femaleScream", netObj);

        //collider 비활성화
        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        //추격 중지
        foreach(Player player in detectedPlayers) {
            player.StopChasedRpc();
        }

        //이동 중지
        ai.isStopped = true;
        ai.velocity = Vector3.zero;
        ai.ResetPath();

        //애니메이션 재생
        SetAnimatorRpc("ChaseEnd");
        SpreadParticleRpc();
        yield return new WaitForSeconds(4.0f);

        GameManager.Instance._mapCreator.ReclaimPositionFemaleRpc(_spawnPositionIndex);

        despawnTrigger = true;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void SpreadParticleRpc() {
       StartCoroutine(PlayParticle());
    }

    public IEnumerator PlayParticle() {
        GameObject obj = Instantiate(bloodParticlePrefab, transform.position, Quaternion.Euler(90f, 0f, 0f));
        yield return new WaitForSeconds(5.5f);
        Destroy(obj);
    }

    public override IEnumerator IdleFunc() {
        SetState(MobState.Chase);
        yield break;
    }

    public override IEnumerator ChaseFunc() {
        for(int i=0; i<75; i++) {
            //공격 처리
            if(_attackEvent.attackFlag) {
                Debug.Log(gameObject + "공격처리");
                yield return StartCoroutine(AttackFunc());
                yield break;
            }
            
            //추격 처리
            if(targetPlayer != null) {
                Debug.Log("targetPos 재설정 / female");
                if(targetPlayer.isHidden) {
                    Debug.Log("플레이어가 숨었음");
                    yield return StartCoroutine(Disappear());
                    yield break;
                }
                if(targetPlayer._deathFlag) {
                    yield return StartCoroutine(Disappear());
                    yield break;
                }
                targetPos = targetPlayer.gameObject.transform.position;
                SetDestination(targetPos);
            }
            else {
                Debug.Log("targetPlayer가 null인데 Chase임");
            }

            Debug.Log(gameObject + "Chasing...");
            yield return new WaitForSeconds(0.2f);
        }
        yield return StartCoroutine(Disappear());
    }

    public IEnumerator AttackFunc() {
        SetAnimatorRpc("Attack");
        Attack(_attackEvent.damagedPlayer, 100, (int)mobID.female);

        ai.isStopped = true;
        yield return StartCoroutine(Disappear());
    }

    private void OnTriggerStay(Collider other) {
        //그 외 충돌 판정은 StateAuthority를 가진 클라만 실행
        if(!HasStateAuthority) return;
        if(other.gameObject.CompareTag("Player") && nowState == MobState.Chase) {
            if(targetPlayer.isHidden == false) {
                Debug.Log(other.gameObject + "Catch!");
                
                _attackEvent.damagedPlayer = other.gameObject.GetComponentInParent<Player>();
                _attackEvent.attackFlag = true;
            }
        }
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public override void ResetAnimationTriggerRpc() {
        animator.ResetTrigger("ChaseEnd");
    }
}
