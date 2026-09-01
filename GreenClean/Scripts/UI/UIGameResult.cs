using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameResult : MonoBehaviour
{
    [Header("결과 화면")]
    [SerializeField]
    private RectTransform resultPanel;

    [SerializeField]
    private CanvasGroup resultCanvasGroup;

    [SerializeField]
    private TMP_Text finalScoreText;

    [SerializeField]
    private Button restartButton;

    [SerializeField]
    private Button goTitleButton;

    [Header("Animation")]
    [SerializeField]
    private float animationDuration = 1f;

    [SerializeField]
    private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private float Height => ((RectTransform)transform).rect.height;

    private Tween tween;

    private void Start()
    {
        resultPanel.anchoredPosition = new Vector2(0f, -Height);

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                GameSceneManager.Instance.ChangeScene(SceneType.GAME);
            });
        }

        if (goTitleButton != null)
        {
            goTitleButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                GameSceneManager.Instance.ChangeScene(SceneType.TITLE);
            });
        }
    }

    public void ShowResult(int finalScore, int clearedBoards)
    {
        int highScore = DataManager.Instance.SaveData.highestScore;
        finalScoreText.text =
            $"최종 성과 평가: {finalScore}점\n"
            + $"(정화 완료: {clearedBoards}구역)\n\n"
            + $"<color=red>역대 최고 점수: {highScore}점</color>";

        tween?.Kill();

        resultCanvasGroup.alpha = 1f;
        resultPanel.anchoredPosition = new Vector2(0f, -Height);
        tween = resultPanel.DOAnchorPosY(0f, animationDuration).SetEase(animationCurve);
    }

    public void Hide() => resultCanvasGroup.alpha = 0f;
}
