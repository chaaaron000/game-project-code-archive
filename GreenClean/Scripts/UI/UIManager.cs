using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// [역할] 게임의 HUD(타이머, 점수) 및 결과 UI, 퍼즈 UI 관리
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD 요소")]
    [SerializeField]
    private Slider timeSlider;

    [Tooltip("HUD에 표시되는 현재 점수 텍스트")]
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private ScorePanel scorePanel;

    [Header("통합 퍼즈 버튼 UI")]
    [SerializeField]
    private Button pauseToggleButton;

    [SerializeField]
    private TextMeshProUGUI pauseButtonText;

    [Header("퍼즈(일시정지) 화면")]
    [SerializeField]
    private UIPause pauseUI;

    [Header("결과 화면")]
    [SerializeField]
    private UIGameResult resultUI;

    [Header("리롤 UI")]
    [SerializeField]
    private Button rerollButton;

    [SerializeField]
    private TextMeshProUGUI rerollButtonText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (pauseToggleButton != null)
        {
            pauseToggleButton.onClick.AddListener(() => GameManager.Instance.TogglePause());
        }

        if (rerollButton != null)
        {
            rerollButton.onClick.AddListener(() => GameManager.Instance.UseReroll());
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            UpdateRerollUI(GameManager.Instance.rerollCount);
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && timeSlider != null)
        {
            timeSlider.value = GameManager.Instance.GetTimeRatio();
        }
    }

    public void TogglePauseUI(bool isPaused)
    {
        pauseUI.SetActiveUI(isPaused);

        if (pauseButtonText != null)
            pauseButtonText.text = isPaused ? "작업 복귀" : "일시 정지";
    }

    public void ShowGameOver(int finalScore, int clearedBoards)
    {
        if (resultUI == null)
        {
            DebugConsole.LogWarning("[UIManager] UIGameResult UI가 초기화되지 않았습니다.");
            return;
        }

        resultUI.ShowResult(finalScore, clearedBoards);
    }

    public void UpdateRerollUI(int currentRerolls)
    {
        if (rerollButtonText != null)
            rerollButtonText.text = $"{currentRerolls}";
        if (rerollButton != null)
            rerollButton.interactable = currentRerolls > 0;
    }
}
