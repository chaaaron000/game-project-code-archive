using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUIController : MonoBehaviour
{
    [SerializeField] private DiarySystem diarySystem;
    [SerializeField] private ScoreUIView scoreUIView;

    private void OnEnable()
    {
        diarySystem = FindObjectOfType<DiarySystem>();
        if (diarySystem == null)
            Debug.LogError("FindObjectOfType<DiarySystem>() is null.");
        
        scoreUIView.Initialize(diarySystem.MaxDiaryNum);
        diarySystem.OnScoreChanged += scoreUIView.UpdateScore;
    }

    private void OnDisable()
    {
        diarySystem.OnScoreChanged -= scoreUIView.UpdateScore;
    }
}
