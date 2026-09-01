using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class DiaryUIController : MonoBehaviour
{
    [SerializeField] private DiarySystem diarySystem;
    [SerializeField] private DiaryUIView diaryUIView;
    
    private UnityAction<int> showDiaryPageLambda;
    private bool isChanging = false;

    private void Awake()
    {
        showDiaryPageLambda = (page) => diaryUIView.ShowDiaryPage(
            page,
            diarySystem.GetCurrentDiarySprite(),
            diarySystem.GetCurrentDiaryContent(),
            diarySystem.GetCurrentDiaryCollectNum());
    }

    private void OnEnable()
    {
        diarySystem = FindObjectOfType<DiarySystem>();
        if (diarySystem == null)
            Debug.LogError("FindObjectOfType<DiarySystem>() is null.");
        
        diaryUIView.Initialize(0);
        diaryUIView.leftButton.onClick.AddListener(OnClickLeftButton);
        diaryUIView.rightButton.onClick.AddListener(OnClickRightButton);
        diarySystem.OnDiaryModeChanged += diaryUIView.SetCanvasEnabled;
        diarySystem.OnScoreChanged += showDiaryPageLambda;

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        diaryUIView.leftButton.onClick.RemoveAllListeners();
        diaryUIView.rightButton.onClick.RemoveAllListeners();
        diarySystem.OnDiaryModeChanged -= diaryUIView.SetCanvasEnabled;
        diarySystem.OnScoreChanged -= showDiaryPageLambda;
        
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnClickLeftButton()
    {
        if (isChanging) return; // Prevent multiple clicks

        isChanging = true;
        diarySystem.currentPageNum = diarySystem.currentPageNum - 1 < 0
            ? diarySystem.collectDiaryNum - 1
            : diarySystem.currentPageNum - 1;
        
        diaryUIView.ShowDiaryPage(
            diarySystem.currentPageNum + 1,
            diarySystem.GetCurrentDiarySprite(),
            diarySystem.GetCurrentDiaryContent(),
            diarySystem.GetCurrentDiaryCollectNum());

        //일기장 넘기는 소리
        SoundManager.Instance.SFX_Play("diaryPageTurn");
        isChanging = false; // Allow changing again
    }

    private void OnClickRightButton()
    {
        if( isChanging) return; // Prevent multiple clicks

        isChanging = true;
        diarySystem.currentPageNum = diarySystem.currentPageNum + 1 >= diarySystem.collectDiaryNum
            ? 0
            : diarySystem.currentPageNum + 1;
        
        diaryUIView.ShowDiaryPage(
            diarySystem.currentPageNum + 1,
            diarySystem.GetCurrentDiarySprite(),
            diarySystem.GetCurrentDiaryContent(),
            diarySystem.GetCurrentDiaryCollectNum());

        //일기장 넘기는 소리
        SoundManager.Instance.SFX_Play("diaryPageTurn");
        isChanging = false; // Allow changing again
    }

    private void OnLocaleChanged(Locale newLocale)
    {
        diaryUIView.ShowDiaryPage(
            diarySystem.currentPageNum + 1,
            diarySystem.GetCurrentDiarySprite(),
            diarySystem.GetCurrentDiaryContent(),
            diarySystem.GetCurrentDiaryCollectNum());
    }

}
