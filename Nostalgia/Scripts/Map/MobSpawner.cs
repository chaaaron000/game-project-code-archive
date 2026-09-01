using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Nostal.Util;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public struct MobSpawnConfig
{
    public GameObject Prefab;
    public int SpawnCount;
}

public class MobSpawner : NetworkBehaviour
{
    [Header("Expressionless / Smile / Sad Setup")]
    [SerializeField] private MobSpawnConfig[] m_mobSpawnConfigs;

    [Header("Angry")] 
    [SerializeField] private int m_angrySpawnCount;
    
    [Header("Female")]
    [SerializeField] private int m_femaleSpawnCount;

    [SerializeField] private List<Transform> m_mobPositions = new List<Transform>();
    [SerializeField] private List<AngrySpawner> m_angrySpawners = new List<AngrySpawner>();
    [SerializeField] private List<FemaleSpawner> m_femaleSpawners = new List<FemaleSpawner>();
    
    private List<int> m_availableAngryPositions;
    private List<int> m_availableFemalePositions;
    private int m_angryMobCnt = 0;
    private int m_femaleMobCnt = 0;

    public List<Transform> MobPositions => m_mobPositions;

    public void AddSpawnPositions(List<Transform>     mobPositions, List<AngrySpawner> angrySpawners,
                                  List<FemaleSpawner> femaleSpawners)
    {
        m_mobPositions.AddRange(mobPositions);
        m_angrySpawners.AddRange(angrySpawners);
        m_femaleSpawners.AddRange(femaleSpawners); 
    }

    /// <summary>
    /// 전체 몹 스폰
    /// </summary>
    public void SpawnMobs()
    {
        StartCoroutine(SpawnESSMobsCoroutine());
        StartCoroutine(SpawnAngryMobsCoroutine());
        StartCoroutine(SpawnFemaleMobsCoroutine());
    }

    /// <summary>
    /// Expressionless, Smile, Sad 스폰 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnESSMobsCoroutine()
    {
        int[] random = Utility.GetRandomIntArray(m_mobPositions.Count, 0, m_mobPositions.Count - 1);
        
        int cnt = 0;
        for (int i = 0; i < m_mobSpawnConfigs.Length; ++i)
        {
            for (int j = 0; j < m_mobSpawnConfigs[i].SpawnCount; ++j)
            {
                Runner.Spawn(
                    m_mobSpawnConfigs[i].Prefab, 
                    m_mobPositions[random[cnt]].position, 
                    Quaternion.identity,
                    Runner.LocalPlayer, OnBeforeSpawned
                );
                
                ++cnt;
                cnt %= m_mobPositions.Count;
                yield return null;
            }
        }
    }
    
    /// <summary>
    /// Angry 스폰 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnAngryMobsCoroutine() 
    {
        m_availableAngryPositions = Enumerable.Range(0, m_angrySpawners.Count).ToList();

        for (int i = 0; i < m_angrySpawnCount && m_availableAngryPositions.Count > 0; ++i)
        {
            // 랜덤으로 위치 선택
            int randomIndex        = UnityEngine.Random.Range(0, m_availableAngryPositions.Count);
            int spawnPositionIndex = m_availableAngryPositions[randomIndex];

            // 몬스터 스폰
            m_angrySpawners[spawnPositionIndex].SpawnTrigger(spawnPositionIndex);

            // 스폰된 위치 제거
            m_availableAngryPositions.RemoveAt(randomIndex);

            ++m_angryMobCnt;
            
            yield return null;
        }
    }

    /// <summary>
    /// Female 스폰 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnFemaleMobsCoroutine()
    {
        m_availableFemalePositions = Enumerable.Range(0, m_femaleSpawners.Count).ToList();

        for (int i = 0; i < m_femaleSpawnCount && m_availableFemalePositions.Count > 0; ++i)
        {
            // 랜덤으로 위치 선택
            int randomIndex        = UnityEngine.Random.Range(0, m_availableFemalePositions.Count);
            int spawnPositionIndex = m_availableFemalePositions[randomIndex];

            // 몬스터 스폰
            m_femaleSpawners[spawnPositionIndex].SpawnTrigger(spawnPositionIndex);

            // 스폰된 위치 제거
            m_availableFemalePositions.RemoveAt(randomIndex);

            ++m_femaleMobCnt;
            
            yield return null;
        }
    }

    private void OnBeforeSpawned(NetworkRunner runner, NetworkObject networkObject)
    {
        BaseMob baseMob = networkObject.GetComponent<BaseMob>();
        int[]   random  = Utility.GetRandomIntArray(baseMob.PatrolPoints.Length, 0, m_mobPositions.Count - 1);
        for (int i = 0; i < baseMob.PatrolPoints.Length; ++i)
        {
            baseMob.PatrolPoints[i] = m_mobPositions[random[i]].position;
        }

        NavMeshAgent agent = networkObject.GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }

    public void ReclaimAngryPosition(int index)
    {
        m_availableAngryPositions.Add(index);
        m_angryMobCnt--;
    }

    public void ReclaimFemalePosition(int index)
    {
        m_availableFemalePositions.Add(index);
        m_femaleMobCnt--;
    }
}
