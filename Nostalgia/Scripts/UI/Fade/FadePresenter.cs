using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FadePresenter : MonoBehaviour
{
    [FormerlySerializedAs("fadeController")] 
    [SerializeField] private FadeView fadeView;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager.OnReadyLevel += OnReadyLevel;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.OnReadyLevel -= OnReadyLevel;
    }

    public void FadeOutWithCallback(UnityAction onFadeOutComplete, float fadeOutDuration = 1, Color fadingColor = default)
    {
        fadeView.SetColor(fadingColor);
        fadeView.FadeOut(fadeOutDuration, onFadeOutComplete);
        //fadeView.ShowLoadingButterfly();
    }

    public void FadeOutWithColor(float fadeOutDuration = 1, Color fadingColor = default){
        fadeView.SetColor(fadingColor);
        fadeView.FadeOut(fadeOutDuration);
    }

    public void FadeIn(float fadeInDuration = 1){
        fadeView.FadeIn(fadeInDuration);
    }

    private void OnReadyLevel()
    {
        fadeView.HideLoadingIcon();
        fadeView.FadeIn(3);
    }

    public void ShowLoadingIcon()
    {
        fadeView.ShowLoadingIcon();
        //fadeView.ShowLoadingButterfly();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 0) return;
        fadeView.FadeIn(3);
    }
}
