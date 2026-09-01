using System.Collections;
using System.Collections.Generic;
using Nostal.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingsPanelsController : MonoBehaviour, UIController
{
    [Header("Canvas")] 
    [SerializeField] private Canvas m_canvas;
    
    [Header("Panels")]
    [SerializeField] private GameObject m_graphicTabPanel;
    [SerializeField] private GameObject m_soundTabPanel;
    [SerializeField] private GameObject m_gamePlayTabPanel;
    [SerializeField] private GameObject m_controlTabPanel;
    
    [Header("Buttons")] 
    [SerializeField] private Button m_graphicTabButton;
    [SerializeField] private Button m_soundTabButton;
    [SerializeField] private Button m_gamePlayTabButton;
    [SerializeField] private Button m_controlTabButton;
    [SerializeField] private Button m_confirmButton;
    
    [Header("Texts")]
    [SerializeField] private TMP_Text m_graphicTabText;
    [SerializeField] private TMP_Text m_soundTabText;
    [SerializeField] private TMP_Text m_gamePlayTabText;
    [SerializeField] private TMP_Text m_controlTabText;

    public void Show()
    {
        m_canvas.enabled = true;
        CursorController.SetEnableCursor(true);
        if (Camera.main.TryGetComponent(out FirstPersonCamera fpc))
        {
            fpc.LockCameraRotate(true);
        }
    }

    public void Hide()
    {
        m_canvas.enabled = false;
        CursorController.SetEnableCursor(false);
        if (Camera.main.TryGetComponent(out FirstPersonCamera fpc))
        {
            fpc.LockCameraRotate(false);
        }
        UIManager.Instance.Clear();
    }

    public void OnClickPanelButton(int clickedButton)
    {
        AllPanelSetActiveFalse();

        GameObject panel = m_graphicTabPanel;
        Button button = m_graphicTabButton;
        TMP_Text text = m_graphicTabText;

        switch (clickedButton)
        {
            case 0:  // 그래픽 패널
                panel = m_graphicTabPanel;
                button = m_graphicTabButton;
                text = m_graphicTabText;
                break;
            
            case 1:  // 사운드 패널
                panel = m_soundTabPanel;
                button = m_soundTabButton;
                text = m_soundTabText;
                break;
            
            case 2:  // 게임 플레이 패널
                panel = m_gamePlayTabPanel;
                button = m_gamePlayTabButton;
                text = m_gamePlayTabText;
                break;
            
            case 3:  // 컨트롤 패널
                panel = m_controlTabPanel;
                button = m_controlTabButton;
                text = m_controlTabText;
                break;
            
            default:
                Debug.LogError("잘못된 버튼 매개변수 입력입니다.");
                break;
        }
        
        panel.SetActive(true);
        SetHighlightedColor(button, 0f);
        text.color = Color.black;
    }

    private void AllPanelSetActiveFalse()
    {
        m_graphicTabPanel.SetActive(false);
        m_soundTabPanel.SetActive(false);
        m_gamePlayTabPanel.SetActive(false);
        m_controlTabPanel.SetActive(false);
           
        SetHighlightedColor(m_graphicTabButton,  0.1f);
        SetHighlightedColor(m_soundTabButton,    0.1f);
        SetHighlightedColor(m_gamePlayTabButton, 0.1f);
        SetHighlightedColor(m_controlTabButton,  0.1f);
        
        m_graphicTabText.color  = Color.white;
        m_soundTabText.color    = Color.white;
        m_gamePlayTabText.color = Color.white;
        m_controlTabText.color  = Color.white;
    }

    private void SetHighlightedColor(Button button, float alpha)
    {
        ColorBlock colors = button.colors;
        Color highlightedColor = colors.highlightedColor;
        highlightedColor.a = alpha;
        colors.highlightedColor = highlightedColor;
        button.colors = colors;
    }
}
