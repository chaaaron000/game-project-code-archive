using Fusion;
using UnityEngine;
using System.Collections;
using Nostal.Network;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{
    private NetworkRunner m_runner => NetworkManager.Instance.Runner;
    
    public GameObject FatherPrefab;
    public GameObject DaughterPrefab;
    
    public Vector3 _spawnPosition;

    [Rpc]
    public void PlayerSpawnRpc([RpcTarget] PlayerRef targetPlayer, Vector3 spawnPosition)
    {
        // StartCoroutine(Initialization(targetPlayer, spawnPosition));
        
        _spawnPosition = spawnPosition;
        
        if (targetPlayer == GameManager.Instance.FatherPlayerRef)
        {
            m_runner.Spawn(FatherPrefab, inputAuthority: targetPlayer, position: spawnPosition);
        }
        else if (targetPlayer == GameManager.Instance.DaughterPlayerRef)
        {
            m_runner.Spawn(DaughterPrefab, inputAuthority: targetPlayer, position: spawnPosition);
        }
    }

    private IEnumerator Initialization(PlayerRef targetPlayer, Vector3 spawnPosition)
    {
        NetworkObject localPlayer = null;
        _spawnPosition = spawnPosition;
        if (targetPlayer == GameManager.Instance.FatherPlayerRef)
        {
            localPlayer = m_runner.Spawn(
                    FatherPrefab,
                    inputAuthority: targetPlayer,
                    position: spawnPosition);        
        }
        else if (targetPlayer == GameManager.Instance.DaughterPlayerRef)
        {
            localPlayer = m_runner.Spawn(
                    DaughterPrefab,
                    inputAuthority: targetPlayer,
                    position: spawnPosition);

        }
        yield return null;
    }

    public void OnBeforeSpawned(NetworkRunner runner, NetworkObject obj) {
        PlayerMovement playerMovement = obj.GetComponent<PlayerMovement>();
        playerMovement._spawnPosition = _spawnPosition;
    }
}