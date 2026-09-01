using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlinkingTextAlpha : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float blinkSpeed = 1f; // 깜빡이는 속도

    void Start()
    {
        StartCoroutine(BlinkAlpha());
    }

    IEnumerator BlinkAlpha()
    {
        while (true)
        {
            for (float t = 0; t <= 1; t += Time.deltaTime * blinkSpeed)
            {
                Color color = textComponent.color;
                color.a = Mathf.Lerp(1, 0, t); // 점점 투명하게
                textComponent.color = color;
                yield return null;
            }

            for (float t = 0; t <= 1; t += Time.deltaTime * blinkSpeed)
            {
                Color color = textComponent.color;
                color.a = Mathf.Lerp(0, 1, t); // 다시 선명하게
                textComponent.color = color;
                yield return null;
            }
        }
    }
}
