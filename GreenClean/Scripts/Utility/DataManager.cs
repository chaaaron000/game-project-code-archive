using UnityEngine;

// 팀의 SingletonComponent를 상속받아 파괴되지 않음!
public class DataManager : SingletonComponent<DataManager>
{
    public GameSaveData SaveData { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        LoadData(); // 게임이 켜지면 무조건 데이터부터 불러옵니다.
    }

    private void LoadData()
    {
        if (global::JsonUtility.TryLoad(out GameSaveData data))
        {
            SaveData = data;
        }
        else
        {
            // 파일이 없으면 초기 기본값 세팅
            SaveData = new GameSaveData()
            {
                highestScore = 0,    
                totalClearedBoards = 0,
                resolutionIndex = 0, // 1920x1080
                masterVolume = 0.8f,
                bgmVolume = 0.9f,
                sfxVolume = 0.7f
            };
        }
    }

    public void SaveGameData()
    {
        global::JsonUtility.Save(SaveData);
        DebugConsole.Log("[DataManager] 모든 데이터 JSON 저장 완료!");
    }
}