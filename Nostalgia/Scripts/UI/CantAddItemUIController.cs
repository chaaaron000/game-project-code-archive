using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class CantAddItemUIController : MonoBehaviour
{

    private void OnEnable()
    {
        if (GameManager.Instance == null)
            throw new NullReferenceException("GameManager.Instance가 null입니다.");
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null)
            throw new NullReferenceException("GameManager.Instance가 null입니다.");
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
