using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Localization.Components;

public class ChapterCardUI : MonoBehaviour {
    [Header("Title")]
    public TextMeshProUGUI titleText;
    [SerializeField] private LocalizeStringEvent m_titleLocalStrEvent;
    
    [Header("Stage Image")]
    public Image stageImage;
    public GameObject lockOverlay;

    [Header("클리어 타임 UI")]
    [SerializeField] private GameObject m_bestClearTimePanel;
    [SerializeField] private TextMeshProUGUI m_bestClearTimeText;
    [Header("스테이지 부가 설명")]
    public TextMeshProUGUI stageText;
    [SerializeField] private LocalizeStringEvent m_stageLocalStrEvent;

    public void Setup(ChapterData data)
    {
        m_titleLocalStrEvent.StringReference = data.title;
        m_titleLocalStrEvent.RefreshString();

        m_stageLocalStrEvent.StringReference = data.stage;
        m_stageLocalStrEvent.RefreshString();
        
        stageImage.sprite = data.image;
        stageImage.color = new Color(1, 1, 1, 0.5f);
        
        lockOverlay.SetActive(data.isLocked);
        m_bestClearTimePanel.SetActive(data.isLocked);
    }

    public void Unlock() 
    {
        lockOverlay.SetActive(false);
        stageImage.color = new Color(1, 1, 1, 1);
        
        m_bestClearTimePanel.SetActive(true);
    }

    public void SetBestClearTimeText(string bestClearTime)
    {
        m_bestClearTimeText.text = bestClearTime;
    }
}