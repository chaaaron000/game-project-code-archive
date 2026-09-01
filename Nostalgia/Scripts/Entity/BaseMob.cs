using System;
using System.Collections;
using Fusion;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public abstract class BaseMob : Entity
{
    private const int MAX_PATROL_POINT_NUM = 5;
    
    [Header("Mob Scriptable Object")]
    [SerializeField] protected MobSO mobSO;
    
    [Header("Nav Mesh Agent")]
    [SerializeField] private NavMeshAgent m_navMeshAgent;
    
    [Header("Patrol Points")]
    public Vector3[] PatrolPoints = new Vector3[MAX_PATROL_POINT_NUM];

    [Header("Sound Controller")] 
    [SerializeField] private SoundController m_soundController;

    private MobState m_currentState = MobState.Idle;
    public  MobState   CurrentState
    {
        get => m_currentState;
        set
        {
            m_currentState = value;
            m_navMeshAgent.speed = mobSO.StateSpeeds[(int)m_currentState];
            
            if (TargetPlayer != null && m_currentState is MobState.Idle or MobState.Alert)
            {
                TargetPlayer.StopChasedRpc();
            }
        }
    }
    
    private bool mb_isPlayersSetup = false;
    private Player[] m_players = new Player[2];  // MapCreator의 플레이어 생성 부분에서 초기화 됨.
    
    private Vector3 m_currentNavMeshDestination = default;
    private int m_currentPatrolIndex = 0;

    public float AttackCoolTime => mobSO.AttackCoolTime;
    public int AttackDamage => mobSO.AttackDamage;
    public float NavMeshRemainingDistance => m_navMeshAgent.remainingDistance;
    public Player TargetPlayer { get; private set; }
    protected NetworkObject mobNetworkObject { get; private set; }

    // 몹이 벽을 뚫지 않도록 하기 위한 필드들
    float checkDistance = 0.5f;
    public GameObject wallObstaclePrefab;
    public float obstacleLifetime = 5f;

    public override void Spawned()
    {
        base.Spawned();

        StartCoroutine(SetPlayersCoroutine());
        //StartCoroutine(PreventThroughWallCoroutine());
        
        mobNetworkObject = GetComponent<NetworkObject>();
        m_navMeshAgent.enabled = true;
        m_navMeshAgent.speed   = mobSO.StateSpeeds[(int)m_currentState];
        
        if (!HasStateAuthority)
        {
            return;
        }
        
        SetNavMeshDestination(PatrolPoints[m_currentPatrolIndex]);
    }

    public void SetNavMeshDestination(Vector3 destination)
    {
        this.m_currentNavMeshDestination = destination;
        m_navMeshAgent.SetDestination(this.m_currentNavMeshDestination);
    }

    public void SetNextPatrolPoint()
    {
        m_currentPatrolIndex += 1;
        m_currentPatrolIndex %= MAX_PATROL_POINT_NUM;
        SetNavMeshDestination(PatrolPoints[m_currentPatrolIndex]);
    }

    public void SetNavMeshIsStopped(bool isStopped)
    {
        m_navMeshAgent.isStopped = isStopped;
    }

    // 몹의 시야로 플레이어를 감지하는 함수
    public bool Observe()
    {
        if (!mb_isPlayersSetup)
        {
            return false;
        }

        if (m_players[0] == null || m_players[1] == null)
        {
            m_players[0] = GameManager.Instance.FatherNetworkObject?.GetComponent<Player>();
            m_players[1] = GameManager.Instance.DaughterNetworkObject?.GetComponent<Player>();

            if (m_players[0] == null || m_players[1] == null)
            {
                return false;
            }
        }
        
        Vector3 pos = transform.position;
        
        // 오름차순 정렬
        Array.Sort(m_players, (c1, c2) =>
            Vector3.Distance(pos, c1.transform.position).CompareTo(Vector3.Distance(pos, c2.transform.position))
        );

        foreach (var player in m_players)
        {
            Transform playerTransform = player.transform;
            
            // 거리 체크
            float distanceToTarget = Vector3.Distance(pos, playerTransform.position);
            if (distanceToTarget > mobSO.ViewRadius)
            {
                continue;
            }
            
            // 각도 체크
            Vector3 dirToTarget = (playerTransform.position - transform.position).normalized;
            float angleBetween = Vector3.Angle(transform.forward, dirToTarget);
            if (angleBetween > mobSO.ViewAngle / 2f)
            {
                continue;
            }

            // 중간 장애물 체크
            // layerMask를 obstacleLayer로 설정하고, Raycast가 감지되지 않으면 중간에 장애물이 있는 것.
            if (Physics.Raycast(pos, dirToTarget, mobSO.ViewRadius, mobSO.ObstacleLayerMask))
            {
                continue;
            }

            // 플레이어 상태 확인
            if (player.isHidden || player._deathFlag) 
            {
                continue;
            }

            TargetPlayer = player;
            m_currentNavMeshDestination = TargetPlayer.gameObject.transform.position;
            
            return true;
        }

        return false;
    }

    private IEnumerator SetPlayersCoroutine()
    {
        while (GameManager.Instance.FatherNetworkObject == null || GameManager.Instance.DaughterNetworkObject == null)
        {
            yield return null;
        }
        
        m_players[0] = GameManager.Instance.FatherNetworkObject.GetComponent<Player>();
        m_players[1] = GameManager.Instance.DaughterNetworkObject.GetComponent<Player>();
        mb_isPlayersSetup = true;
    }    
}