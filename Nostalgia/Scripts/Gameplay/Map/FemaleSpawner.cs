using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class FemaleSpawner : NetworkBehaviour
{
    [Networked] public bool m_bCanSpawnMob { get; set; } = false;
    public GameObject _femaleMobPrefab;
    public Transform _femaleMobPosition;
    private int spawnPositionIndex;
    public NetworkObject targetPlayer;
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void SpawnRpc(NetworkObject obj) {
        //중복 스폰 방지
        if(m_bCanSpawnMob == false) return;

        m_bCanSpawnMob = false;
        targetPlayer = obj;
        Debug.Log("SpawnRpc: Female CAlled");
        var mob = Runner.Spawn(_femaleMobPrefab, _femaleMobPosition.position, _femaleMobPosition.rotation, Runner.LocalPlayer, OnBeforeSpawned);
        mob.GetComponent<Female>()._spawnPositionIndex = this.spawnPositionIndex;
    }

    public void OnBeforeSpawned(NetworkRunner runner, NetworkObject obj) {
        Debug.Log("FemaleSpawner: OnBeforeSpawned: " + targetPlayer);
        obj.GetComponent<Female>().targetPlayer = targetPlayer.GetComponent<Player>();
    }

    public void SpawnTrigger(int spawnPositionIndex) {
        if (HasStateAuthority == false) return;
        this.spawnPositionIndex = spawnPositionIndex;
        m_bCanSpawnMob = true;
    }

    // Update is called once per frame
    public void OnTriggerEnter(Collider other)
    {
        if(!HasStateAuthority) return;
        if (other.gameObject.tag == "Player" && m_bCanSpawnMob == true)
        {
            SpawnRpc(other.GetComponent<NetworkObject>());
        }
    }
}
