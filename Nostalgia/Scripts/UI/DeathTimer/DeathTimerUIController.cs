using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class DeathTimerUIController : MonoBehaviour
{
    [SerializeField] private DeathTimerUIView deathTimerUIView;

    private void OnEnable()
    {
        if (GameManager.Instance == null)
            throw new NullReferenceException("GameManager.Instance가 null입니다.");

        deathTimerUIView = GetComponent<DeathTimerUIView>();
        GameManager.Instance.OnDeathTimerUpdated += deathTimerUIView.SetTime;
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null)
            throw new NullReferenceException("GameManager.Instance가 null입니다.");
        
        GameManager.Instance.OnDeathTimerUpdated -= deathTimerUIView.SetTime;
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
