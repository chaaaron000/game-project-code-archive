using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FadeView : MonoBehaviour
{
    private const float defaultFadeDuration = 1;

    [Header("Canvas")] 
    [SerializeField] private Canvas canvas;
    
    [Header("Panel")]
    [SerializeField] private Image fadePanel;
    [SerializeField] private GameObject butterflyPanel;
    
    [Header("Image")] 
    //[SerializeField] private Image[] butterflyImages;

    private bool isFadeInRunning = false;
    private bool isFadeOutRunning = false;
    // private bool isButterflyRunning = false;
    private UnityAction afterFadeInAction;

    private void Start()
    {
        butterflyPanel.SetActive(false);
        SetCanvasActive(false);
    }
    
    public void SetCanvasActive(bool active)
    {
        canvas.enabled = active;
    }

    public void SetColor(Color color)
    {
        fadePanel.color = color;
    }

    public void SetColor(float r, float g, float b)
    {
        var color = new Color(r, g, b);
        fadePanel.color = color;
    }

    /// <summary>
    /// fadeInDuration 동안 페이드아웃 됩니다.
    /// </summary>
    /// <param name="fadeOutDuration">페이드아웃이 진행될 시간초입니다. 기본값은 1입니다.</param>
    /// <param name="afterFadeIn">페이드아웃이 끝나고 진행할 UnityAction입니다.</param>
    public void FadeOut(float fadeOutDuration = defaultFadeDuration, UnityAction afterFadeIn = null)
    {
        afterFadeInAction = afterFadeIn;
        StartCoroutine(FadeOutCoroutine(fadeOutDuration));
    }

    /// <summary>
    /// fadeOutDuration 동안 페이드인 됩니다.
    /// </summary>
    /// <param name="fadeInDuration">페이드인이 진행될 시간초입니다. 기본값은 1입니다.</param>
    public void FadeIn(float fadeInDuration = defaultFadeDuration)
    {
        StartCoroutine(FadeInCoroutine(fadeInDuration));
    }

    /// <summary>
    /// 페이드아웃이 되고 대기 후 페이드인 됩니다.
    /// </summary>
    /// <param name="fadeOutDuration">페이드인이 진행될 시간초입니다. 기본값은 1입니다.</param>
    /// <param name="waitingTime">페이드인이 진행되고 대기할 시간초입니다. 기본값은 1입니다.</param>
    /// <param name="fadeInDuration">페이드아웃이 진행될 시간초입니다. 기본값은 1입니다.</param>
    public void FadeOutAndIn(
        float fadeOutDuration = defaultFadeDuration,
        float waitingTime = 1,
        float fadeInDuration = defaultFadeDuration)
    {
        StartCoroutine(FadeOutAndInCoroutine(fadeOutDuration, waitingTime, fadeInDuration));
    }
    
    private IEnumerator FadeOutCoroutine(float fadeOutDuration)
    {
        // 페이드인이 진행되고 있으면 취소
        if (isFadeInRunning) yield break;
        
        // 만약 페이드아웃이 진행되고 있으면 대기
        if (isFadeOutRunning)
            while (isFadeOutRunning)
                yield return null;

        isFadeInRunning = true;
        
        Color color = fadePanel.color;
        color.a = 0f;
        SetCanvasActive(true);
        
        // 0 -> 1로 증가
        float elapsedTime = 0f;  // 경과 시간
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeOutDuration);
            fadePanel.color = color;
            yield return null;
        }

        // 보정
        color.a = 1f;
        fadePanel.color = color;
        
        isFadeInRunning = false;
        afterFadeInAction?.Invoke();
    }

    private IEnumerator FadeInCoroutine(float fadeInDuration)
    {
        // 페이드아웃이 진행되고 있으면 취소
        if (isFadeOutRunning) yield break;
        
        // 만약 페이드인이 진행되고 있으면 대기
        if (isFadeInRunning)
            while (isFadeInRunning)
                yield return null;

        isFadeOutRunning = true;
        
        Color color = fadePanel.color;
        color.a = 1f;
        SetCanvasActive(true);
        
        // 1 -> 0로 감소
        float elapsedTime = 0f;  // 경과 시간
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = 1 - Mathf.Clamp01(elapsedTime / fadeInDuration);
            fadePanel.color = color;
            yield return null;
        }

        // 보정
        color.a = 0f;
        fadePanel.color = color;
        SetCanvasActive(false);
        
        isFadeOutRunning = false;
    }

    private IEnumerator FadeOutAndInCoroutine(float fadeOutDuration, float waitingTime, float fadeInDuration)
    {
        yield return StartCoroutine(FadeOutCoroutine(fadeOutDuration));
        yield return new WaitForSeconds(waitingTime);
        yield return StartCoroutine(FadeInCoroutine(fadeInDuration));
    }

    public void ShowLoadingIcon()
    {
        butterflyPanel.SetActive(true);
    }

    public void HideLoadingIcon()
    {
        butterflyPanel.SetActive(false);
    }
}
