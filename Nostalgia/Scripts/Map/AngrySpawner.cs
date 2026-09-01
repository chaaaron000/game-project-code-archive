using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class AngrySpawner : NetworkBehaviour
{
    public GameObject _angryMobPrefab;
    public Transform _angryMobPosition;
    [SerializeField] private int soundDistance = 10;
    
    private int spawnPositionIndex;
    
    [Networked] private bool m_bCanSpawnMob { get; set; } = false;

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void SpawnRpc()
    {
        m_bCanSpawnMob = false;
        
        var mob = Runner.Spawn(
            _angryMobPrefab, 
            _angryMobPosition.position, 
            _angryMobPosition.rotation,
            Runner.LocalPlayer
        );
        
        mob.GetComponent<Angry>()._spawnPositionIndex = this.spawnPositionIndex;
    }

    public void SpawnTrigger(int spawnPositionIndex)
    {
        if (HasStateAuthority == false)
        {
            return;
        }
        
        this.spawnPositionIndex = spawnPositionIndex;
        m_bCanSpawnMob = true;
    }

    // Update is called once per frame
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && m_bCanSpawnMob)
        {
            SpawnRpc();
            
            float distanceToPlayer = Vector3.Distance(
                GameManager.Instance.GetLocalPlayer().transform.position,
                _angryMobPosition.position
            );
        
        }
    }
}