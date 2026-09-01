using System;
using System.Collections;
using System.Collections.Generic;
using Nostal.Util;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class GameOverUIView : MonoBehaviour
{
    [Header("View Settings")] 
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float fadeInInterval;
    
    [Header ("Camera")]
    [SerializeField] private Camera gameOverCamera;
    
    [Header ("UI Canvas")]
    [SerializeField] private Canvas canvas;
    
    [Header ("UI Text")]
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private TMP_Text retryText;
    [SerializeField] private TMP_Text exitText;
    [SerializeField] private Image youDiedText;
    
    [Header ("UI Button")]
    [SerializeField] public Button retryButton;
    [SerializeField] public Button exitButton;

    private void OnEnable()
    {
        gameOverCamera.enabled = false;
        
        canvas.enabled = false;
        retryButton.interactable = false;
        exitButton.interactable = false;

        //알파값 0으로 초기화
        //gameOverText.color = new Color(255, 255, 255, 0);
        Color currentColor = gameOverText.color;
        currentColor.a = 0f;
        gameOverText.color = currentColor;

        currentColor = retryText.color;
        currentColor.a = 0f;
        retryText.color = currentColor;

        currentColor = exitText.color;
        currentColor.a = 0f;
        exitText.color = currentColor;

        currentColor = youDiedText.color;
        currentColor.a = 0f;
        youDiedText.color = currentColor;

        //retryText.color = new Color(255, 255, 255, 0);
        //exitText.color = new Color(255, 255, 255, 0);
    }

    public void Init() {
        OnEnable();
    }

    /// <summary>
    /// 게임 오버 화면의 UI를 페이드인 하는 메소드
    /// </summary>
    public void FadeIn()
    {
        canvas.enabled = true;
        
        // 커서 활성화
        CursorController.SetEnableCursor(true);
        if (Camera.main.TryGetComponent<FirstPersonCamera>(out FirstPersonCamera fpc))
        {
            fpc.LockCameraRotate(true);
        }
        
        StartCoroutine(FadeInCoroutine());
    }

    IEnumerator FadeInCoroutine()
    {
        yield return new WaitForSeconds(1f);

        SetRetryButtonColor();

        yield return StartCoroutine(FadeInTextCoroutine(gameOverText));
        yield return StartCoroutine(FadeInImageCoroutine(youDiedText));
        yield return StartCoroutine(FadeInTextCoroutine(retryText));
        yield return StartCoroutine(FadeInTextCoroutine(exitText));
        
        //Server만 retryButton 활성화
        if(GameManager.Instance.IsServer) {
            retryButton.interactable = true;
        }
        else {
            retryButton.interactable = false;
        }

        exitButton.interactable = true;
    }

    IEnumerator FadeInTextCoroutine(TMP_Text tmpText)
    {
        Color color = tmpText.color;
        float elapsedTime = 0f;  // 경과 시간

        // 0 -> 1로 증가
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeInDuration);
            tmpText.color = color;
            yield return null;
        }

        // 보정
        color.a = 1f;
        tmpText.color = color;
        
        yield return new WaitForSeconds(fadeInInterval);
    }

    IEnumerator FadeInImageCoroutine(Image image)
    {
        Color color = image.color;
        float elapsedTime = 0f;  // 경과 시간

        // 0 -> 1로 증가
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeInDuration);
            image.color = color;
            yield return null;
        }

        // 보정
        color.a = 1f;
        image.color = color;
        
        yield return new WaitForSeconds(fadeInInterval);
    }

    public void SetRetryButtonColor() {
        bool isServer = GameManager.Instance.IsServer;

        //Server면 버튼 색깔 하얀색, client면 회색
        if(isServer) {
            ColorBlock colors = retryButton.colors;
            colors.normalColor = Color.white;
            retryButton.colors = colors;
        }
        else {
            ColorBlock colors = retryButton.colors;
            colors.normalColor = Color.gray;
            colors.highlightedColor = Color.gray;
            colors.pressedColor = Color.gray;
            retryButton.colors = colors;
        }
    }
}
