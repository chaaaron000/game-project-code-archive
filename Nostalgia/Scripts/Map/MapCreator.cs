using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using Fusion;
using Nostal.Util;
using UnityEngine.Events;
using System;
using Nostal.Map;
using Nostal.Network;

[Serializable]
public struct Area
{
    [SerializeField] public NetworkObject[] Tiles;
    public Vector3[]    Positions;
    public Quaternion[] Rotations;
}

[Serializable]
public struct ItemConfig
{
    public GameObject ItemPrefab;
    public int        ItemSpawnCount;
}

public class MapCreator : NetworkBehaviour
{
    private NetworkRunner m_runner => NetworkManager.Instance.Runner;
    
    [Networked] public bool bMapCreated { get; private set; } = false;
    
    // 타일을 object pool느낌으로 position과 같이 저장
    [Header("Area 섞기")]
    [SerializeField] private Area m_area4;
    [SerializeField] private Area m_area3;
    [SerializeField] private Area m_area2;

    [Header("아이템")]
    [SerializeField] private ItemConfig[] m_itemConfigs;

    [Header("탈출구 설정")]
    [SerializeField] public GameObject _exitPrefab;
    [SerializeField] public NostalgiaGameLevel NextSceneName;

    [Header("스포너")] 
    [SerializeField] private MobSpawner m_mobSpawner;
    
    [Header("Jumpscares")]
    [SerializeField] public Jumpscare[] jumpscares;

    [Header("NavMeshSurface")]
    [SerializeField] private NavMeshSurface navMeshSurface;
    
    [SerializeField] private List<Transform> m_spawnPositions = new List<Transform>();
    [SerializeField] private List<Transform> m_itemPositions = new List<Transform>();
    [SerializeField] private List<Transform> m_exitPositions = new List<Transform>();

    public static UnityAction OnAllTilesShuffleComplete;

    public override void Spawned()
    {
        base.Spawned();

        if (!HasStateAuthority)
        {
            return;
        }

        StartCoroutine(CreateMap());
    }

    private IEnumerator CreateMap()
    {
        yield return StartCoroutine(ShuffleAreaCoroutine(m_area4));
        yield return StartCoroutine(ShuffleAreaCoroutine(m_area3));
        yield return StartCoroutine(ShuffleAreaCoroutine(m_area2));
        
        navMeshSurface.BuildNavMesh();
        
        m_mobSpawner.SpawnMobs();
        StartCoroutine(SetItems());
        StartCoroutine(SetExit());
        
        bMapCreated = true;
        
        StartCoroutine(SetPlayers());
    }

    private IEnumerator ShuffleAreaCoroutine(Area area)
    {
        int length = area.Tiles.Length;
        int[] random = Utility.GetRandomIntArray(length, 0, length - 1);
        
        for (int i = 0; i < length; i++)
        {
            NetworkObject obj = m_runner.Spawn(area.Tiles[i], area.Positions[random[i]], area.Rotations[random[i]]);
            if (obj.TryGetComponent(out NostalgiaTile tile))
            {
                m_spawnPositions.AddRange(tile.PlayerSpawnPositions);
                m_itemPositions.AddRange(tile.ItemPositions);
                m_exitPositions.AddRange(tile.ExitPositions);
                m_mobSpawner.AddSpawnPositions(tile.MobPositions, tile.AngrySpawners, tile.FemaleSpawners);
            }
            
            yield return null;
        }
    }

    private IEnumerator SetPlayers() 
    {
        PlayerSpawner playerSpawner = GameManager.Instance.PlayerSpawner;
        int[] random = Utility.GetRandomIntArray(1, 0, m_spawnPositions.Count-1);
        Transform spawnPosition = m_spawnPositions[random[0]];
        GameManager.Instance.SetSpawnPositionRpc(spawnPosition.position);

        playerSpawner.PlayerSpawnRpc(GameManager.Instance.FatherPlayerRef, spawnPosition.position);
        yield return null;
        playerSpawner.PlayerSpawnRpc(
            GameManager.Instance.DaughterPlayerRef, 
            new Vector3(
                spawnPosition.position.x, 
                spawnPosition.position.y, 
                spawnPosition.position.z + 1));
        
        yield return null;
    }

    private IEnumerator SetItems()
    {
        Queue<Transform> random = Utility.ArrayToShuffledQueue(m_itemPositions.ToArray());
        // int[] random = Utility.GetRandomIntArray(_itemPositions.Length, 0, _itemPositions.Length-1);
        // int cnt = 0;
        
        foreach (ItemConfig itemConfig in m_itemConfigs)
        {
            for (int i = 0; i < itemConfig.ItemSpawnCount && random.Count != 0; ++i)
            {
                Transform spawnTransform = random.Dequeue();
                m_runner.Spawn(
                    itemConfig.ItemPrefab,
                    spawnTransform.position,
                    spawnTransform.rotation,
                    m_runner.LocalPlayer
                    );

                yield return null;
            }
        }
    }

    private IEnumerator SetExit() 
    {
        int[] random = Utility.GetRandomIntArray(1, 0, m_exitPositions.Count-1);
        
        m_runner.Spawn(
            _exitPrefab,
            m_exitPositions[random[0]].position, 
            m_exitPositions[random[0]].rotation, 
            m_runner.LocalPlayer,
            OnBeforeExitSpawned);
        
        yield return null;
    }

    public void OnBeforeExitSpawned(NetworkRunner runner, NetworkObject obj) 
    {
        ExitDoor exitDoor = obj.GetComponent<ExitDoor>();
        exitDoor.nextScene = NextSceneName;
    }

    public Jumpscare GetJumpscare(int index) 
    {
        if (index < 0 || index >= jumpscares.Length) 
        {
            Debug.LogError("Invalid index from GetJumpscare: " + index);
            return null;
        } 
        
        return jumpscares[index];
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ReclaimPositionAngryRpc(int index) 
    {
        m_mobSpawner.ReclaimAngryPosition(index);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ReclaimPositionFemaleRpc(int index) 
    {
        m_mobSpawner.ReclaimFemalePosition(index);
    }
}