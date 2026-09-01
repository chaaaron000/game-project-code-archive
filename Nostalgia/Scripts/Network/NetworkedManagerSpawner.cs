using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// 세션 나가기 시 사라지는 Manager를 다시 스폰하기 위한 클래스
/// </summary>
public class NetworkedManagerSpawner : MonoBehaviour
{
    [SerializeField] private NetworkRunner networkRunner;
    [SerializeField] private GameObject gameManagerPrefab;
    [SerializeField] private GameObject gameSceneManagerPrefab;
    [SerializeField] private GameObject soundManagerPrefab;
    [SerializeField] private GameObject saveManagerPrefab;

    [SerializeField] private List<NetworkPrefabRef> m_networkManagerPrefabs;

    private void OnEnable()
    {
        if (networkRunner == null)
            networkRunner = GetComponent<NetworkRunner>();
    }

    /// <summary>
    /// 다시 재생성이 필요한 매니저를 스폰함
    /// </summary>
    public void SpawnNetworkedManager(bool useNetworkRunner)
    {
        if (useNetworkRunner)
        {
            NetworkObject manager;
            
            if (FindObjectOfType<GameManager>() == null)
            {
                manager = networkRunner.Spawn(gameManagerPrefab);
                manager.name = gameManagerPrefab.name;
            }

            if (FindObjectOfType<GameSceneManager>() == null)
            {
                manager = networkRunner.Spawn(gameSceneManagerPrefab);
                manager.name = gameSceneManagerPrefab.name;
            }

            if (FindObjectOfType<SoundManager>() == null)
            {
                manager = networkRunner.Spawn(soundManagerPrefab);
                manager.name = soundManagerPrefab.name;
            }

            if (FindObjectOfType<SaveManager>() == null)
            {
                manager = networkRunner.Spawn(saveManagerPrefab);
                manager.name = saveManagerPrefab.name;
            }
        }
        else
        {
            GameObject manager;
            
            if (FindObjectOfType<GameManager>() == null)
            {
                manager = Instantiate(gameManagerPrefab);
                manager.name = gameManagerPrefab.name;
            }

            if (FindObjectOfType<GameSceneManager>() == null)
            {
                manager = Instantiate(gameSceneManagerPrefab);
                manager.name = gameSceneManagerPrefab.name;
            }

            if (FindObjectOfType<SoundManager>() == null)
            {
                manager = Instantiate(soundManagerPrefab);
                manager.name = soundManagerPrefab.name;
            }

            if (FindObjectOfType<SaveManager>() == null)
            {
                manager = Instantiate(saveManagerPrefab);
                manager.name = saveManagerPrefab.name;
            }
        }
        
    }
}
