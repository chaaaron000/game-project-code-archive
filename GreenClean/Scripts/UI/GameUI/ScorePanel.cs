using QQbry;
using TMPro;
using UnityEngine;

public class ScorePanel : MonoBehaviour
{
    public static ScorePanel Instance { get; private set; }

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private TextMeshProUGUI boardClearScoreText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // public void UpdateScoreUI(int currentScore)
    // {
    //     if (scoreText != null)
    //     {
    //         scoreText.text = $"{currentScore}";
    //     }
    // }

    public void UpdateBoardClearScore(int currentScore)
    {
        if (boardClearScoreText != null)
        {
            boardClearScoreText.text = $"보드 클리어 횟수: {currentScore}";
        }
    }
}
