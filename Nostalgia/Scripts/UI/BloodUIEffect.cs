using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodUIEffect : MonoBehaviour
{
    public Image[] images; // UI 이미지 배열    
    public float fadeDuration = 1f; // 서서히 나타나는 시간


    public void ShowImage(int index)
    {
        if (index >= 0 && index < images.Length)
        {
            StartCoroutine(FadeIn(images[index], fadeDuration));
        }
        else
        {
            Debug.LogWarning("잘못된 인덱스입니다.");
        }
    }

    public void HideImage(int index)
    {
        if (index >= 0 && index < images.Length)
        {
            StartCoroutine(FadeOut(images[index], fadeDuration));
        }
        else
        {
            Debug.LogWarning("잘못된 인덱스입니다.");
        }
    }

    IEnumerator FadeIn(Image image, float duration)
    {
        image.gameObject.SetActive(true); // 이미지 활성화
        Color color = image.color;
        float startAlpha = 0f; // 시작할 때 투명
        float endAlpha = 1f;   // 최종적으로 불투명
        float elapsedTime = 0f;

        color.a = startAlpha;
        image.color = color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            image.color = color;
            yield return null;
        }

        // 최종적으로 완전한 불투명 상태로 설정
        color.a = endAlpha;
        image.color = color;
    }

    IEnumerator FadeOut(Image image, float duration)
    {
        Color color = image.color;
        float startAlpha = image.color.a; // 현재 알파 값
        float endAlpha = 0f;   // 최종적으로 투명
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            image.color = color;
            yield return null;
        }

        color.a = endAlpha;
        image.color = color;
        image.gameObject.SetActive(false); // 이미지 비활성화
    }
}
