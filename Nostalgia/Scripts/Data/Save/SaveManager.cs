using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Nostal.Steam;
using Nostal.Util;

public class SaveManager : NetworkSingleton<SaveManager>
{
    [SerializeField] private List<ESteamStat> m_stageClearCountStats = new List<ESteamStat>();
    [SerializeField] private List<ESteamStat> m_stageBestClearTimeStats = new List<ESteamStat>();

    private List<int> m_stageClearCounts = new List<int>();
    private List<int> m_stageBestClearTimes = new List<int>();

    public List<int> StageClearCounts => m_stageClearCounts;
    public List<int> StageBestClearTimes => m_stageBestClearTimes;
    
    public bool ClearCountsValid { get; private set; }
    public bool BestClearTimesValid { get; private set; }
    
    public string saveFilePath;
    public SaveData saveData;

    [SerializeField] private AudioSource m_gameStartSound;

    public override void Spawned() 
    {
        base.Spawned();

        StartCoroutine(GetStageClearCountCoroutine());
        StartCoroutine(GetStageBestClearTimeCoroutine());
        
        //초기화
        // saveFilePath = Application.persistentDataPath + "/save.json";
        // LoadGame();
    }

    public void StageSelectUIInitialize() 
    {
        StageSelectUIController stageSelectUIController = UIManager.Instance.StageSelectUIController;
        StartCoroutine(showStageSelectUI(stageSelectUIController));
        //stageSelectUIController.Init(HasStateAuthority);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void SetStageImageRpc(int direction) {
        //서버는 바꿀 필요 없음 
        if(HasStateAuthority) return;

        //서버가 보낸 정보에 따라 클라입장에서 이미지를 수정함
        StageSelectUIController stageSelectUIController = UIManager.Instance.StageSelectUIController;
        stageSelectUIController.Slide(direction);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void UnlockStageRpc(int index) {
        //서버는 바꿀 필요 없음 
        if(HasStateAuthority) return;

        Debug.Log("UnlockStageRpc Index: " + index);
        //서버가 보낸 정보에 따라 클라입장에서 이미지를 수정함
        StageSelectUIController stageSelectUIController = UIManager.Instance.StageSelectUIController;
        StartCoroutine(stageSelectUIController.UnlockStage(index));
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void StartGameRpc() 
    {
        m_gameStartSound.Stop();
        m_gameStartSound.Play();
        
        StartCoroutine(StartGameWithDelay(1));
    }
    
    private IEnumerator GetStageClearCountCoroutine()
    {
        ClearCountsValid = false;
        
        yield return new WaitUntil(() =>
            SteamStatsAndAchievements.Instance != null && SteamStatsAndAchievements.Instance.StatsValid);

        m_stageClearCounts.Clear();
        foreach (ESteamStat stat in m_stageClearCountStats)
        {
            m_stageClearCounts.Add(SteamStatsAndAchievements.Instance.GetSteamStat(stat)?.IntValue ?? -1);
        }

        ClearCountsValid = true;
    }
    
    private IEnumerator GetStageBestClearTimeCoroutine()
    {
        BestClearTimesValid = false;
        
        yield return new WaitUntil(() =>
            SteamStatsAndAchievements.Instance != null && SteamStatsAndAchievements.Instance.StatsValid);

        m_stageBestClearTimes.Clear();
        foreach (ESteamStat stat in m_stageBestClearTimeStats)
        {
            m_stageBestClearTimes.Add(SteamStatsAndAchievements.Instance.GetSteamStat(stat)?.IntValue ?? -1);
        }

        BestClearTimesValid = true;
    }

    private IEnumerator showStageSelectUI(StageSelectUIController stageSelectUIController)
    {
        yield return new WaitForSeconds(1.0f);
        stageSelectUIController.ShowCanvas(true);
        stageSelectUIController.Init(HasStateAuthority);
        StartCoroutine(stageSelectUIController.FadeCanvas(true));
    }

    private IEnumerator StartGameWithDelay(int countDownTime)
    {
        int currentTime = countDownTime;
        while (currentTime > 0)
        {
            Debug.Log($"Game Start Countdown: {currentTime}, Father: {GameManager.Instance.FatherPlayerRef}, Daughter: {GameManager.Instance.DaughterPlayerRef}");
            yield return new WaitForSeconds(1);
            currentTime--;
        }
        
        if (GameManager.Instance.GameReady == false) {
            Debug.Log("Something is wrong, GameStarted is false. Maybe other player left.");
            yield break;
        }

        NostalgiaGameLevel level = (NostalgiaGameLevel)UIManager.Instance.StageSelectUIController.GetSelectedStage();
        //NostalgiaGameLevel level = NostalgiaGameLevel.Chase;
        GameManager.Instance.StartGame(level);
    }
    
    // public void SaveGame()
    // {
    //     string json = JsonUtility.ToJson(saveData, true);
    //     System.IO.File.WriteAllText(saveFilePath, json);
    // }
    //
    // public void LoadGame()
    // {
    //     if (System.IO.File.Exists(saveFilePath))
    //     {
    //         string json = System.IO.File.ReadAllText(saveFilePath);
    //         saveData = JsonUtility.FromJson<SaveData>(json);
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Save file not found. Creating a new one.");
    //         saveData = new SaveData();
    //         SaveGame();
    //     }
    // }
    //
    // public NostalgiaGameLevel CheckClearLevel() {
    //     return (int)saveData.clearLevel > 2 ? NostalgiaGameLevel.LevelOne : saveData.clearLevel;
    // }
    //
    // [Rpc(RpcSources.StateAuthority, RpcTargets.All)]   
    // public void SetClearLevelRpc(NostalgiaGameLevel level) {
    //     if(saveData.clearLevel >= level) return;
    //
    //     //기존보다 높은 레벨로 클리어한 경우에만 저장
    //     saveData.clearLevel = level;
    //     SaveGame();
    // }
}
