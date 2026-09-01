using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fusion;
using System;
using _Scripts.UI.Diary;
using UnityEngine.Serialization;

public class DiarySystem : NetworkBehaviour
{
    [SerializeField] private GameObject DiaryUIPrefab;
    [SerializeField] private GameObject ScoreUIPrefab;
    
    public bool bIsDiaryEnabled { get; private set; } = false;

    [SerializeField] private LevelDiaryContentSO m_levelDiaryContentSO;

    private String[] diaryTextArray;

    //diaryPage들의 획득여부를 동기화 하는 NetworkArray
    [Networked, Capacity(5)]
    public NetworkArray<bool> diaryData { get; } = MakeInitializer(new bool[]
    {
        false,
        false,
        false,
        false,
        false
    });
    
    //게임에 존재하는 전체 page를 나타내는 dictionary
    public List<DiaryData> diaryDictionary;

    public int currentPageNum = 0;
    [Networked, OnChangedRender(nameof(OnDiaryChanged))] public int collectDiaryNum { get; private set; } = 0;

    private int m_maxDiaryNum = 0;
    public int MaxDiaryNum => m_maxDiaryNum;
    
    //diary가 처음 열릴 때의 버그를 방지하기 위한 flag
    private bool initFlag = false;

    public UnityAction<bool> OnDiaryModeChanged;
    public UnityAction<int> OnScoreChanged;
    public GameObject diaryUI;
    public GameObject scoreUI;

    public override void Spawned()
    {
        CSVReader reader = GetComponent<CSVReader>();
        diaryTextArray = reader.GetThirdColumnData();
        
        if (GameManager.Instance != null) {
            GameManager.Instance.DiarySystem = this;
        }

        m_maxDiaryNum = m_levelDiaryContentSO.GetContentsSize();

        diaryUI = Instantiate(DiaryUIPrefab);
        scoreUI = Instantiate(ScoreUIPrefab);

        bIsDiaryEnabled = false;
    }

    private void LateUpdate()
    {
        if (bIsDiaryEnabled && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleDiaryMode();
        }
    }

    // Diary Mode를 키고 끄는 함수
    public void ToggleDiaryMode()
    {
        bIsDiaryEnabled = !bIsDiaryEnabled;
        OnDiaryModeChanged?.Invoke(bIsDiaryEnabled);
    }

    public Sprite GetCurrentDiarySprite()
    {
        return m_levelDiaryContentSO.GetDiaryPageSprite(currentPageNum);
    }

    public string GetCurrentDiaryContent()
    {
        return m_levelDiaryContentSO.GetDiaryContent(currentPageNum);
    }

    public int GetCurrentDiaryCollectNum()
    {
        return collectDiaryNum;
    }

    public void ChangeCurrentPage(int num) {
        // 획득한 적 없는 diary를 클릭했을 때 return
        if(diaryData[num] == false) {
            return;
        }
        currentPageNum = num;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void GetDiaryRpc() {
        // 한번이라도 접속상태로 diary를 먹으면 버그가 해결
        if (initFlag == false) initFlag = true;

        // 이미 일기장을 최대로 획득했을 시 return
        if (collectDiaryNum == MaxDiaryNum) return;
        
        diaryData.Set(collectDiaryNum, true);
        collectDiaryNum += 1;
        SetCurrentPageNumRpc(collectDiaryNum - 1);

        if (collectDiaryNum == MaxDiaryNum) {
            if(GameManager.Instance._exitDoor != null)
                GameManager.Instance._exitDoor = FindObjectOfType<ExitDoor>();
            //door open
            GameManager.Instance._exitDoor.OpenDoor();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DestroyDiaryRpc(NetworkObject diaryNetworkObject)
    {
        Runner.Despawn(diaryNetworkObject);
    }

    public void OnDiaryChanged() {
        if (collectDiaryNum == MaxDiaryNum) {
            // door open
            if(GameManager.Instance._exitDoor != null)
                GameManager.Instance._exitDoor = FindObjectOfType<ExitDoor>();
            GameManager.Instance._exitDoor.OpenDoor();
        }

        OnScoreChanged?.Invoke(collectDiaryNum);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void SetCurrentPageNumRpc(int page)
    {
        currentPageNum = page;
    }

    public void SetLevelDiaryContentSO(LevelDiaryContentSO levelDiaryContentSO)
    {
        m_levelDiaryContentSO = levelDiaryContentSO;
    }
    
#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, Screen.height - 40, 100, 30), "Get All Diarys"))
        {
            for (int i = 0; i < 5; i++)
            {
                GetDiaryRpc();
            }
        }
    }
#endif
}
