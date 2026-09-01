using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUIController : MonoBehaviour
{
    public void Start() {
        Canvas canvas = gameObject.GetComponent<Canvas>();
        GameObject uiCamera = GameObject.Find("UICamera");

        canvas.worldCamera = uiCamera.GetComponent<Camera>();
    }
}
