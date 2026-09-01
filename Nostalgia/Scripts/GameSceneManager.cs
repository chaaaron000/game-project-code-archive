using Fusion;
using System.Collections;
using System.Collections.Generic;
using Nostal.Network;
using Nostal.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

[System.Serializable]
public enum NostalgiaGameLevel
{
    MainMenu = 0,
    Tutorial = 1,
    LevelOne = 2,
    LevelTwo = 3,
    Chase = 4,
    Ending = 5
}

public class GameSceneManager : NetworkSingleton<GameSceneManager>
{
    private NetworkRunner m_runner => NetworkManager.Instance.Runner;
    
    [SerializeField] private List<string> _scenesPath = new List<string>();
    
    [SerializeField] private FadePresenter fadePresenter;
    
    public static UnityAction<int> OnSceneLoadComplete;
    
    public override void Spawned()
    {
        base.Spawned();
        
        fadePresenter = FindObjectOfType<FadePresenter>();
    }

    public void LoadScene(NostalgiaGameLevel scene)
    {
        Debug.Log($"{m_runner.LocalPlayer}, {GetComponent<NetworkObject>().StateAuthority}");
        LoadSceneRpc((int)scene);
    }

    public void ReloadCurrentScene()
    {
        ReloadCurrentSceneRpc();
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void LoadSceneRpc(int sceneIndex)
    {
        fadePresenter.FadeOutWithCallback(() => StartCoroutine(LoadSceneCoroutine(sceneIndex)));
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void ReloadCurrentSceneRpc()
    {
        UIManager.Instance.IsGameOver = false;
        fadePresenter.FadeOutWithCallback(() => StartCoroutine(ReloadCurrentSceneCoroutine()));
    }

    private bool _isSceneLoading;

    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        fadePresenter.ShowLoadingIcon();
        
        if (_isSceneLoading) yield break;
        if (m_runner == null) { Debug.LogError("Runner null"); yield break; }
        if (m_runner.SceneManager == null) { Debug.LogError("Runner.SceneManager null"); yield break; }
        if (!m_runner.IsRunning) { Debug.LogError("Runner not in session"); yield break; }

        if (!m_runner.IsSceneAuthority)
        {
            yield break;
        }

        _isSceneLoading = true;

        NetworkSceneAsyncOp asyncOp = m_runner.LoadScene(_scenesPath[sceneIndex]);
        float t0 = Time.realtimeSinceStartup;
        while (!asyncOp.IsDone) {
            if (Time.realtimeSinceStartup - t0 > 30f) {
                Debug.LogError($"LoadScene TIMEOUT.");
                break;
            }
            yield return null;
        }
        
        _isSceneLoading = false;
        //yield return new WaitUntil(() => asyncOp.IsDone);


        InvokeOnSceneLoadCompleteRpc(sceneIndex);
    }
    
    private IEnumerator UnloadSceneCoroutine(SceneRef sceneRef)
    {
        if (!m_runner.IsSceneAuthority) yield break;
        NetworkSceneAsyncOp asyncOp = m_runner.UnloadScene(sceneRef);
        yield return new WaitUntil(() => asyncOp.IsDone);
    }

    private IEnumerator UnloadSceneCoroutine(int sceneIndex)
    {
        if (!m_runner.IsSceneAuthority) yield break;
        SceneRef sceneRef = SceneRef.FromIndex(sceneIndex);
        NetworkSceneAsyncOp asyncOp = m_runner.UnloadScene(sceneRef);
        yield return new WaitUntil(() => asyncOp.IsDone);
    }

    /// <summary>
    /// 현재 씬을 리로드하는 코루틴 메소드입니다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReloadCurrentSceneCoroutine()
    {
        // 현재 씬의 인덱스 가져오기
        int sceneIndex = GetCurrentSceneIndex();
        
        // 씬 언로드
        yield return StartCoroutine(UnloadSceneCoroutine(sceneIndex));

        // 씬 로드
        yield return StartCoroutine(LoadSceneCoroutine(sceneIndex));
    }

    private int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    private SceneRef GetCurrentSceneRef()
    {
        return SceneRef.FromIndex(GetCurrentSceneIndex());
    }

    /// <summary>
    /// OnSceneLoadComplete를 발동하는 Rpc 메소드입니다.
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void InvokeOnSceneLoadCompleteRpc(int sceneIndex)
    {
        Debug.Log("Invoking OnSceneLoadComplete for scene index: " + sceneIndex);
        OnSceneLoadComplete?.Invoke(sceneIndex);
    }
}
