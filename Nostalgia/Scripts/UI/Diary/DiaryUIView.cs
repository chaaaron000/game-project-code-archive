using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiaryUIView : MonoBehaviour
{
    [Header("Diary")]
    [SerializeField] private Canvas diaryCanvas;

    [Header("Button")] 
    [SerializeField] public Button leftButton;
    [SerializeField] public Button rightButton;

    [Header("Image")]
    [SerializeField] private Image diaryImage;

    [Header("Text")] 
    [SerializeField] private TextMeshProUGUI currentPageText;
    [SerializeField] private TextMeshProUGUI totalDiaryText;
    [SerializeField] private TextMeshProUGUI diaryContentText;

    public void Initialize(int totalDiaryNum)
    {
        totalDiaryText.text = totalDiaryNum.ToString();
        diaryCanvas.enabled = false;
    }

    public void SetCanvasEnabled(bool isEnabled)
    {
        diaryCanvas.enabled = isEnabled;
        if(isEnabled == true) {
            UIManager.Instance.SetCameraLock(true);
        }
        else {
            UIManager.Instance.SetCameraLock(false);
        }
    }

    public void ShowDiaryPage(int page, Sprite diarySprite, string diaryContent, int nowDiaryNum)
    {
        diaryImage.sprite = diarySprite;
        currentPageText.text = page.ToString();
        diaryContentText.text = diaryContent;
        totalDiaryText.text = nowDiaryNum.ToString();
    }

}
