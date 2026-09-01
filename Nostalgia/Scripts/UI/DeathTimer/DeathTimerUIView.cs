using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class DeathTimerUIView : MonoBehaviour
{
    [SerializeField] private TMP_Text deathTimerText;
    [SerializeField] private TMP_Text deathText;
    [SerializeField] private Color startColor = Color.white;
    [SerializeField] private Color endColor = Color.red;

    private void OnEnable()
    {
        if (deathTimerText == null)
            throw new NullReferenceException("DeathTimerText가 할당되지 않았습니다.");

        // 타이머가 다시 시작될 때 초기 색상으로 리셋
        deathTimerText.color = startColor;
        deathText.color = startColor;
    }

    /// <summary>
    /// 타이머에 숫자를 표시하는 메소드입니다.
    /// 10초 이상일 때는 정수만 표시합니다.
    /// 10초 미만일 때는 소숫점 첫 번째까지 표시합니다.
    /// </summary>
    /// <param name="time">표시할 시간입니다.</param>
    public void SetTime(float time) {
        string printTime = (time >= 10f) ?
            Mathf.FloorToInt(time).ToString() :  // 정수만 표시
            time.ToString("F1");           // 소수점 1자리까지 표시

        // 색상 보간 (0초에 가까워질수록 빨간색으로 변경)
        float t = Mathf.Clamp01(time / 100f); // 100초 이상이면 1, 0초이면 0
        deathTimerText.color = Color.Lerp(endColor, startColor, t);
        deathText.color = Color.Lerp(endColor, startColor, t);

        deathTimerText.text = printTime;
    }
}
