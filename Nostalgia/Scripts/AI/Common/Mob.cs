using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Fusion;
using TMPro;

public enum MobState
{
    Idle = 0,
    Alert = 1,
    Chase = 2
}

public class Mob : Entity
{
    private static int maxTargetNum = 5;
    public Vector3[] pos = new Vector3[maxTargetNum];  // Map 기본 위치들이 담기는 배열
    private int nowTargetNum = 0;   // Map 기본 위치를 지정하는 index 

    //현재 mob의 목표 위치
    [SerializeField] protected Vector3 targetPos;

    //NavMesh 기반 Mob Stat
    protected NavMeshAgent ai;
    [SerializeField] protected MobState nowState;
    public MobState NowState => nowState;
    
    [SerializeField] protected float speed = 2.0f;
    [SerializeField] protected float[] stateSpeed = {2.0f, 3.0f, 5.0f};
    [SerializeField] protected float viewRadius = 1.5f;
    [SerializeField] protected float viewAngle = 60f;

    protected LayerMask playerLayer;
    protected LayerMask obstacleLayer;

    //시야 관측에 필요한 변수들
    private Vector3 viewTargetPos;
    private Vector3 viewTargetDir;
    private float viewTargetAngle;
    private Vector3 lookDir;
    protected Collider[] hitLists;
    protected Player hitPlayer;

    // animator을 담는 변수
    [SerializeField] public Animator animator;

    protected float attackCoolTime = 0;
    // 공격 쿨타임
    public float AttackCoolTime => attackCoolTime;

    protected Coroutine nowFunc;
    protected SoundController soundController;

    private Player[] players;
    private bool _isPlayersSetup = false;

    public float NavMeshRemainingDistance => ai.remainingDistance;
    protected Coroutine mobFuncCoroutine;

    protected NetworkObject networkObject;

    // 몹이 벽을 뚫지 않도록 하기 위한 필드들
    float checkDistance = 0.5f;
    public GameObject wallObstaclePrefab;
    public float obstacleLifetime = 5f;

    /*public void GetanimatorRpc() {
        //animator = GetComponent<animator>();
    }*/

    public override void Spawned()
    {
        networkObject = GetComponent<NetworkObject>();
        Init();
    }

    public virtual void Init()
    {
        Debug.Log("Init");

        GameManager.OnReadyLevel += SetPlayers;

        // 시야각 관련 초기 설정
        hitLists = new Collider[3];
        playerLayer = 1<<3;
        obstacleLayer = LayerMask.NameToLayer("Obstacle");

        // mob state 초기 설정
        nowState = MobState.Idle;
        soundController = GetComponent<SoundController>();

        // 이제부터 몹 알고리즘 관련 내용. StateAuthority만 적용
        if (!HasStateAuthority) return;

        // NavMesh 초기 설정
        ai = GetComponent<NavMeshAgent>();
        ai.enabled = true;
        targetPos = pos[nowTargetNum];
        SetDestination(targetPos);
        speed = stateSpeed[0];
        ai.speed = speed;

        mobFuncCoroutine = StartCoroutine(MobFunc());
    }

    public void SetDestination(Vector3 targetPos)
    {
        this.targetPos = targetPos;
        ai.SetDestination(this.targetPos);
    }

    protected IEnumerator MobFunc() {
        while(true) {
            //State별로 속도 설정
            ai.speed = stateSpeed[(int)nowState];
            ResetAnimationTriggerRpc();

            //현재 State에따라 Animation을 전환하고 해당 State에 맞는 함수 실행 
            if(nowState == MobState.Idle) {
                SetAnimatorRpc("Idle");
                yield return nowFunc = StartCoroutine(IdleFunc());
            }
            else if(nowState == MobState.Alert) {
                SetAnimatorRpc("Alert");
                yield return nowFunc = StartCoroutine(AlertFunc());
            }
            else if(nowState == MobState.Chase) {
                SetAnimatorRpc("Chase");
                yield return nowFunc = StartCoroutine(ChaseFunc());
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator PreventThroughWallCoroutine()
    {
        RaycastHit hit;
        Vector3 origin;
        Vector3[] directions; 
        
        if(!HasStateAuthority) yield break;

        while (true) {
            origin = transform.position + Vector3.up * 0.5f;
            directions = new Vector3[] {
                transform.forward,
                -transform.forward,
                transform.right,
                -transform.right
            };  

            foreach (var dir in directions)
            {
                if (Physics.Raycast(origin, dir, out hit, checkDistance, LayerMask.GetMask("Obstacle")))
                {
                    Vector3 spawnPos = hit.point + hit.normal * 0.1f;

                    GameObject obs = Instantiate(wallObstaclePrefab, spawnPos, Quaternion.identity);
                    obs.transform.forward = hit.normal; // 벽 표면 방향

                    Destroy(obs, obstacleLifetime);
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// 순찰을 수행하는 코루틴 메소드.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator IdleFunc() {
        // 다음 목적지까지 걸어가기
        // 거리가 일정 범위 이내면 1.5초 대기 후 그 다음 목적지 설정
        float dist = ai.remainingDistance;
        if (dist <= 1.0f) {
            SetAnimatorRpc("IdleIsStopped", true);
            yield return new WaitForSeconds(2.5f);
            SetNextTarget();
            SetAnimatorRpc("IdleIsStopped", false);
        }
        
    }

    public virtual IEnumerator AlertFunc() {
        float dist = ai.remainingDistance;
        if(dist <= 1.0f) {
            SetAnimatorRpc("AlertIsStopped", true);
            yield return new WaitForSeconds(3.0f);
            SetNextTarget();
            SetAnimatorRpc("AlertIsStopped", false);
        }
    }

    public virtual IEnumerator ChaseFunc() {
        // float dist = ai.remainingDistance;
        // if(dist <= 0.5f) {
        //     yield return new WaitForSeconds(2.5f);
        //     SetNextTarget();
        // }
        yield return null;
    }

    public void SetNextTarget() {
        nowTargetNum += 1;
        nowTargetNum %= maxTargetNum;
        targetPos = pos[nowTargetNum];

        SetDestination(targetPos);
    }

    public void SetState(MobState value) 
    {
        if (!HasStateAuthority)
        {
            return;
        }
        
        nowState = value;
        ai.speed = stateSpeed[(int)nowState];
        if (hitPlayer != null && (nowState == MobState.Idle || nowState == MobState.Alert))
        {
            hitPlayer.StopChasedRpc();
            hitPlayer = null;
        }
    }

    //일정 시간 대기하게 하는 함수
    public bool Wait(ref float coolTime, float target) {
        coolTime += Time.deltaTime;
        Debug.Log(coolTime);
        if(coolTime >= target) {
            coolTime = 0;
            return true;
        }
        return false;
    }

    //각도를 벡터값으로 바꿔주는 함수
    public Vector3 AngleToDir(float angle){
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }

    // 몹의 시야로 플레이어를 감지하는 함수
    public bool Observe()
    {
        if (!_isPlayersSetup)
        {
            return false;
        }
        
        Vector3 pos = transform.position;
        
        // 오름차순 정렬
        Array.Sort(players, (c1, c2) =>
            Vector3.Distance(pos, c1.transform.position).CompareTo(Vector3.Distance(pos, c2.transform.position))
        );

        foreach (var player in players)
        {
            Transform playerTransform = player.transform;
            
            // 거리 체크
            float distanceToTarget = Vector3.Distance(pos, playerTransform.position);
            if (distanceToTarget > viewRadius)
            {
                continue;
            }
            
            // 각도 체크
            Vector3 dirToTarget = (playerTransform.position - transform.position).normalized;
            float angleBetween = Vector3.Angle(transform.forward, dirToTarget);
            if (angleBetween > viewAngle / 2f)
            {
                continue;
            }

            // 중간 장애물 체크
            // layerMask를 obstacleLayer로 설정하고, Raycast가 감지되지 않으면 중간에 장애물이 있는 것.
            if (Physics.Raycast(pos, dirToTarget, viewRadius, obstacleLayer))
            {
                continue;
            }

            // 플레이어 상태 확인
            if (player.isHidden || player._deathFlag) 
            {
                continue;
            }

            if (hitPlayer != player)
            {
                if (hitPlayer != null)
                {
                    hitPlayer.StopChasedRpc();
                }

                hitPlayer = player;
                hitPlayer.ChasedRpc();
            }
            
            targetPos = hitPlayer.gameObject.transform.position;
            
            return true;
        }

        return false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetAnimatorRpc(string name, bool value) {
        if(animator != null)
            animator.SetBool(name, value);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetAnimatorRpc(string name) {
        if (animator != null)
            animator.SetTrigger(name);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public virtual void ResetAnimationTriggerRpc() {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Alert");
        animator.ResetTrigger("Chase");
        animator.ResetTrigger("Attack");
    }

    public void SetMobSpeedByNowState()
    {
        if (ai == null)
        {
            return;
        }
        
        ai.speed = stateSpeed[(int)nowState];
    }

    public void SetNavMeshIsStopped(bool isStopped)
    {
        if (ai == null)
        {
            return;
        }
        
        ai.isStopped = isStopped;
    }

    public void ClearHitPlayer()
    {
        if (hitPlayer != null)
        {
            hitPlayer.StopChasedRpc();
        }

        hitPlayer = null;
    }

    public Player GetHitPlayer()
    {
        return this.hitPlayer;
    }

    private void SetPlayers()
    {
        // Player 저장
        players = new Player[2];
        players[0] = GameManager.Instance.FatherNetworkObject.GetComponent<Player>();
        players[1] = GameManager.Instance.DaughterNetworkObject.GetComponent<Player>();
        _isPlayersSetup = true;
    }
}
