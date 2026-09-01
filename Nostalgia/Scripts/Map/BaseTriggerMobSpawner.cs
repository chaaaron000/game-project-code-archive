using Fusion;
using UnityEngine;

public abstract class BaseTriggerMobSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject m_mobPrefab;
    [SerializeField] private Transform  m_spawnPoint;
    
    [Networked] public bool bIsSpawned { get; set; } = false;
    
    private int m_spawnPositionIndex;
    
    public void SpawnTrigger(int spawnPositionIndex) 
    {
        if (HasStateAuthority == false)
        {
            return;
        }
        
        this.m_spawnPositionIndex = spawnPositionIndex;
        bIsSpawned = true;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && bIsSpawned)
        {
            SpawnRpc();
        }
    }
    
    public abstract void SpawnRpc();
}