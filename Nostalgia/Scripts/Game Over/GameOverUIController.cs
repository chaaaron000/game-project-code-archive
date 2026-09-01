using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

public class GameOverUIController : MonoBehaviour
{
    [SerializeField] private GameOverUIView gameOverUIView;

    public Camera gameOverCamera;

    private Camera PlayerCamera;

    private void OnEnable()
    {
        Debug.Log("GameOverUIController OnEnable called");
        GameManager.Instance.OnGameOver += OnGameOver;
        gameOverUIView.retryButton.onClick.AddListener(OnClickRetry);
        gameOverUIView.exitButton.onClick.AddListener(OnClickExit);

        PlayerCamera = Camera.main;
    }
    
    private void OnDisable()
    {
        Debug.Log("GameOverUIController OnDisable called");
        GameManager.Instance.OnGameOver -= OnGameOver;
        gameOverUIView.retryButton.onClick.RemoveAllListeners();
        gameOverUIView.exitButton.onClick.RemoveAllListeners();
    }
    
    void OnGameOver()
    {
        Debug.Log("GameOverUIController OnGameOver called");

        // DeathTimerUIController가 있으면 숨김
        if(UIManager.Instance.DeathTimerUIController != null)
            UIManager.Instance.DeathTimerUIController.Hide();

        Debug.Log("Hide DeathTimerUIController");

        //카메라 전환
        PlayerCamera.GetComponent<AudioListener>().enabled = false;
        gameOverCamera.enabled = true;
        gameOverCamera.GetComponent<AudioListener>().enabled = true;

        gameOverUIView.FadeIn();
    }

    public void Hide() {
        PlayerCamera.GetComponent<AudioListener>().enabled = true;
        gameOverCamera.enabled = false;
        PlayerCamera.enabled = true;
        gameOverCamera.GetComponent<AudioListener>().enabled = false;

        gameOverUIView.Init();
    }


    void OnClickRetry()
    {
        // 리트라이 로직
        // Debug.Log("OnClickRetry");
        GameManager.Instance.RetryGame();
    }

    void OnClickExit()
    {
        // Exit 로직
        // Debug.Log("OnClickExit");
        GameManager.Instance.BackToMainMenuRpc();
    }
}
