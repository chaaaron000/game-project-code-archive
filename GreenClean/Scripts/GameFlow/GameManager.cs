using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private GameProgressBlackboard blackboard;

    [Header("게임 설정")]
    [SerializeField]
    private float timeLimit = 120f;

    [SerializeField]
    private AudioClip mainGameBGM;

    [SerializeField]
    QQbry.NumberDisplay numberDisplay;

    [Header("UI 연결")]
    public GameObject floatingTextPrefab;

    [Header("리롤 및 콤보 시스템")]
    public int rerollCount = 2;

    private int CurrentCombo
    {
        get => currentCombo;
        set
        {
            currentCombo = value;
            blackboard.TrySet(BlackboardKey.COMBO_COUNT, currentCombo);
        }
    }

    // 추가된 통합 점수
    public int beforeTotalScore;
    public int afterTotalScore { get; set; }

    private int currentCombo;
    private float _currentTime;
    private int _clearedBoards = 0;
    private bool _isGameActive = true;
    private bool _isPaused = false;

    public bool IsPaused => _isPaused;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        _currentTime = timeLimit;
        afterTotalScore = 0;
    }

    private void OnEnable()
    {
        var cardManager = FindFirstObjectByType<CardManager>();
        if (cardManager != null)
        {
            cardManager.CardUsed += AddUsedCardsCount;
        }
        else
        {
            DebugConsole.LogError("[GameManager] CardManager not found.");
        }
    }

    private void Start()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayBGM("GameBGM");

        if (blackboard != null)
        {
            blackboard.ResetBlackboard();
        }
    }

    private void Update()
    {
        if (!_isGameActive || _isPaused)
            return;

        _currentTime -= Time.deltaTime;
        if (_currentTime <= 0)
            GameOver();
    }

    private void OnDisable()
    {
        if (CardManager.Instance != null)
        {
            CardManager.Instance.CardUsed -= AddUsedCardsCount;
        }
    }

    // --- 점수 추가 함수 ---
    public void AddScore(int amount, bool isBoardClear = false, bool isCombo = false)
    {
        beforeTotalScore = afterTotalScore;
        afterTotalScore += amount;
        DebugConsole.Log(afterTotalScore);
        DebugConsole.Log($"[점수 획득] +{amount}점! (현재 점수: {afterTotalScore})");
        numberDisplay.ChangeDigit();
        DebugConsole.Log("ChangeDigit 실행함");
        //ScorePanel.Instance.UpdateScoreUI(afterTotalScore); // UI 만들면 주석 해제하세요
        // 보드 클리어가 아닐 때만 마우스 위치에 일반 텍스트 생성
        if (!isBoardClear && !isCombo && amount > 0)
        {
            SpawnFloatingText(amount, Input.mousePosition);
        }
    }

    public void AddBoardClearScore()
    {
        _clearedBoards++;
        // 1. 점수 추가 (텍스트 생성을 막기 위해 true 전달)
        AddScore(10, true);
        // 2. 화면 중앙 좌표 계산 (Screen 중심은 해상도의 절반)
        Vector3 centerScreenPos = new Vector3(Screen.width / 2f, Screen.height / 2f, 10f);
        // 3. 중앙에 보드 클리어 전용 텍스트 생성
        SpawnBoardClearText(10, centerScreenPos);
        DebugConsole.Log($"[보고] 구역 정화 완료! 누적: {_clearedBoards}");
        ScorePanel.Instance.UpdateBoardClearScore(_clearedBoards);
    }

    public float GetTimeRatio() => _currentTime / timeLimit;

    public void TogglePause()
    {
        if (!_isGameActive)
            return;
        _isPaused = !_isPaused;
        Time.timeScale = _isPaused ? 0f : 1f;
        UIManager.Instance.TogglePauseUI(_isPaused);
    }

    public void AddCombo()
    {
        CurrentCombo++;
        DebugConsole.Log($"연속 완벽 정화! 현재 콤보: {CurrentCombo}");

        int scoreBonus = 0;
        int rerollReward = 0;

        switch (CurrentCombo)
        {
            case 3:
                scoreBonus = 3;
                rerollReward = 1;
                break;
            case 5:
                scoreBonus = 5;
                rerollReward = 1;
                break;
            case 7:
                scoreBonus = 7;
                rerollReward = 2;
                break;
            case 10:
                scoreBonus = 20;
                rerollReward = 3;
                CurrentCombo = 0;
                DebugConsole.Log("10콤보 달성! (콤보 초기화)");
                break;
        }

        if (scoreBonus > 0)
        {
            AddScore(scoreBonus, false, true);
        }

        if (rerollReward > 0)
        {
            rerollCount += rerollReward;
            UIManager.Instance.UpdateRerollUI(rerollCount);
        }
    }

    public void ResetCombo()
    {
        if (CurrentCombo > 0)
        {
            CurrentCombo = 0;
            DebugConsole.Log("효율 저하... 콤보가 0으로 초기화되었습니다.");
        }
    }

    public void UseReroll()
    {
        if (rerollCount > 0)
        {
            rerollCount--;
            UIManager.Instance.UpdateRerollUI(rerollCount);
            DeckManager.Instance.DrawCards();
        }
    }

    private void SpawnBoardClearText(int score, Vector3 screenPos)
    {
        if (floatingTextPrefab == null)
            return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = -2f; // 일반 점수보다 더 앞에 보이게 설정

        GameObject popup = Instantiate(floatingTextPrefab, worldPos, Quaternion.identity);
        FloatingText ft = popup.GetComponent<FloatingText>();

        // Setup 함수를 호출하되, 보드 클리어임을 알려서 더 크게 만듦 (아래 FloatingText 수정 참고)
        ft.Setup(score, true);
    }

    private void SpawnFloatingText(int score, Vector3 screenPos) // 🌟 매개변수(Vector3) 추가!
    {
        if (floatingTextPrefab == null)
            return;

        // 카메라로부터 얼마나 떨어진 월드 좌표로 변환할지
        screenPos.z = 10f;

        // Z값이 포함된 스크린 좌표를 월드 좌표로 변환합니다.
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        // 실제 오브젝트의 Z축 위치 고정 (-1f)
        worldPos.z = -1f;

        // 정확한 월드 좌표에 프리팹 생성 후 점수 입력
        GameObject popup = Instantiate(floatingTextPrefab, worldPos, Quaternion.identity);

        //  수정됨: 마우스 위치에 뜨는 일반 점수이므로 false를 같이 넘겨줍니다!
        popup.GetComponent<FloatingText>().Setup(score, false);
    }

    private void GameOver()
    {
        _isGameActive = false;
        _currentTime = 0;

        // --- JSON 저장 로직 시작 ---
        bool isNewRecord = false;

        // 1. 최고 점수 갱신 확인
        if (afterTotalScore > DataManager.Instance.SaveData.highestScore)
        {
            DataManager.Instance.SaveData.highestScore = afterTotalScore;
            isNewRecord = true;
        }

        // 2. 클리어한 보드 횟수 누적
        DataManager.Instance.SaveData.totalClearedBoards += _clearedBoards;

        // 3. 기록이 갱신됐거나 보드를 하나라도 깼다면 JSON 저장
        if (isNewRecord || _clearedBoards > 0)
        {
            DataManager.Instance.SaveGameData();
            DebugConsole.Log("최고 점수 또는 누적 클리어 횟수 갱신 저장 완료!");
        }
        // --- JSON 저장 로직 끝 ---

        UIManager.Instance.ShowGameOver(afterTotalScore, _clearedBoards);
    }

    private void AddUsedCardsCount()
    {
        blackboard.TrySet(
            BlackboardKey.USED_CARD_COUNT,
            blackboard.Get<int>(BlackboardKey.USED_CARD_COUNT) + 1
        );
    }

#if UNITY_EDITOR
    public void Debug_GameOver() => GameOver();
#endif
}
