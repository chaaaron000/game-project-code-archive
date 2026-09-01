using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// [역할] 씬 로딩 및 앱 종료 관리 (싱글톤)
/// </summary>
public class GameSceneManager : SingletonComponent<GameSceneManager>
{
    [System.Serializable]
    private readonly struct SceneInfo
    {
        public readonly int BuildIndex;
        public readonly string Name;
        public readonly string Path;

        public SceneInfo(int buildIndex, string name, string path)
        {
            BuildIndex = buildIndex;
            Name = name;
            Path = path;
        }
    }

    public event Action<SceneType, SceneType> SceneLoadStarted;
    public event Action<bool, SceneType> SceneLoadCompleted;

    public bool IsLoading { get; private set; } = false;

    private IReadOnlyList<SceneInfo> scenesInBuild;

#if UNITY_EDITOR
    private Dictionary<string, SceneInfo> scenesByName = new Dictionary<string, SceneInfo>();
#else
    private readonly Dictionary<string, SceneInfo> scenesByName =
        new Dictionary<string, SceneInfo>();
#endif

    private static bool IsValidBuildIndex(int buildIndex) =>
        buildIndex >= 0 && buildIndex < SceneManager.sceneCountInBuildSettings;

    protected override void Awake()
    {
        base.Awake();

        scenesInBuild = GetScenesInBuild();
        foreach (var sceneInfo in scenesInBuild)
        {
            DebugConsole.Log($"[GameSceneManager] {sceneInfo.BuildIndex} - {sceneInfo.Path}]");
            scenesByName[sceneInfo.Name] = sceneInfo;
        }
    }

    public void ChangeScene(int buildIndex)
    {
        if (!IsValidBuildIndex(buildIndex))
        {
            DebugConsole.LogError(
                $"[GameSceneManager] 올바른 빌드 인덱스가 아닙니다: {buildIndex}"
            );
            return;
        }

        LoadSceneAsync(buildIndex).Forget();
        // SceneManager.LoadScene(buildIndex);
    }

    public void ChangeScene(SceneType sceneType)
    {
        ChangeScene((int)sceneType);
    }

    public void ChangeScene(string sceneName)
    {
        if (!scenesByName.TryGetValue(sceneName, out SceneInfo sceneInfo))
        {
            DebugConsole.LogError($"[GameSceneManager] 올바른 Scene 이름이 아닙니다: {sceneName}");
            return;
        }

        int buildIndex = sceneInfo.BuildIndex;
        ChangeScene(buildIndex);
    }

    public void PlayClickSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX("SelectSFX");
        }
    }

    public void QuitGame()
    {
        Debug.Log("그린케어 업무 종료. 드론 회수 중...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IReadOnlyList<SceneInfo> GetScenesInBuild()
    {
        List<SceneInfo> result = new();
        int count = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < count; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(path);
            result.Add(new SceneInfo(i, sceneName, path));
        }

        return result;
    }

    private async UniTask LoadSceneAsync(int buildIndex)
    {
        if (IsLoading)
        {
            return;
        }

        IsLoading = true;
        SceneType loadSceneType = (SceneType)buildIndex;
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneLoadStarted?.Invoke((SceneType)currentBuildIndex, loadSceneType);

        var op = SceneManager.LoadSceneAsync(buildIndex);
        if (op == null)
        {
            DebugConsole.LogError($"[GameSceneManager] 씬 로드 실패: {buildIndex}");
            SceneLoadCompleted?.Invoke(false, loadSceneType);
            IsLoading = false;
            return;
        }

        await op.ToUniTask();

        SceneLoadCompleted?.Invoke(true, loadSceneType);
        IsLoading = false;
    }
}
