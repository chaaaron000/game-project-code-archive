using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenuController : MonoBehaviour, UIController
{
    public GameObject BackgroundBlur;
    public GameObject panelImage;
    public Button resumeButton;
    public Button settingsButton;
    public Button exitButton;
    public Button retryButton;
    public TextMeshProUGUI retryCountText;
    public GameObject retryPanel;
    private bool isRetryPossible = true;

    public void Show()
    {
        UIManager.Instance.PauseMenuActive = true;
        BackgroundBlur.SetActive(true);
        panelImage.SetActive(true);
        ActiveButtons(true);
        
        UIManager.Instance.SetCameraLock(true);
    }

    public void Hide()
    {
        UIManager.Instance.PauseMenuActive = false;
        BackgroundBlur.SetActive(false);
        panelImage.SetActive(false);
        ActiveButtons(false);
        
        UIManager.Instance.SetCameraLock(false);
    }
    
    private void Awake()
    {
        if (resumeButton == null)
            resumeButton = transform.GetChild(2).GetComponent<Button>();

        if (settingsButton == null)
            settingsButton = transform.GetChild(3).GetComponent<Button>();

        if (exitButton == null)
            exitButton = transform.GetChild(4).GetComponent<Button>();

        if (retryButton == null)
            retryButton = transform.GetChild(5).GetComponent<Button>();
        
        if (retryPanel == null)
            retryPanel = retryButton.transform.GetChild(0).gameObject;

        if (retryCountText == null)
            retryCountText = retryPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
        resumeButton.onClick.AddListener(OnClickResumeButton);
        settingsButton.onClick.AddListener(OnClickSettingsButton);
        exitButton.onClick.AddListener(OnClickExitButton);

        //retry 관련
        retryButton.onClick.AddListener(OnClickRetryButton);
        retryPanel.SetActive(false);
        RetryCountChange(GameManager.Instance.retryCount);
        GameManager.OnRetryCountChangedEvent += RetryCountChange;

        if(SceneManager.GetActiveScene().name == "TutorialS") {
            Debug.Log("PauseMenuController: Tutorial scene detected, checking tutorial stage state...");
            TutorialManager m_tutorialManager = FindObjectOfType<TutorialManager>();
            if(m_tutorialManager != null)
            {
                if(m_tutorialManager.isStageStarted == false)
                {
                    Debug.Log("PauseMenuController: Tutorial stage not started, disabling retry button.");
                    // 튜토리얼 스테이지 전에는 리트라이 버튼을 비활성화
                    retryButton.gameObject.GetComponent<TextMeshProUGUI>().color = Color.gray;
                    isRetryPossible = false;
                }
            }
        }
        
    }

    public void OnDestroy()
    {
        retryPanel.SetActive(false);
        GameManager.OnRetryCountChangedEvent -= RetryCountChange;
    }

    public void RetryCountChange(int count)
    {
        if(count > 0)
            retryPanel.SetActive(true);
        if (retryCountText != null)
        {
            retryCountText.text = count.ToString() + "/" + 2;
        }
    }

    private void OnClickResumeButton()
    {
        UIManager.Instance.PauseMenuActive = false;
        UIManager.Instance.Pop();
    }

    private void OnClickSettingsButton()
    {
        UIManager.Instance.Push("SettingUIController");
    }

    private void OnClickExitButton()
    {
        // 게임 나가기 기능
        GameManager.Instance?.BackToMainMenuRpc();
        
        UIManager.Instance.PauseMenuActive = false;
        UIManager.Instance.Pop();
    }

    private void OnClickRetryButton()
    {
        if(!isRetryPossible)
        {
            return;
        }
        // 리트라이 기능
        GameManager.Instance.TryRetry();
    }

    void ActiveButtons(bool active)
    {
        resumeButton.gameObject.SetActive(active);
        settingsButton.gameObject.SetActive(active);
        exitButton.gameObject.SetActive(active);
        retryButton.gameObject.SetActive(active);
    }
}
