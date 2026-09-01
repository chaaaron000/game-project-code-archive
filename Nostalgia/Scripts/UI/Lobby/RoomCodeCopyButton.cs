using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomCodeCopyButton : MonoBehaviour
{
    public TextMeshProUGUI roomCodeText; // 방코드 텍스트
    public Button copyButton;          // UI 버튼

    void Start()
    {
        copyButton.onClick.AddListener(CopyToClipboard);
    }

    void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = roomCodeText.text;
        Debug.Log("방코드가 클립보드에 복사되었습니다: " + roomCodeText.text);
    }
}

