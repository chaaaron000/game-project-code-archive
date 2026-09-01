using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;


public class TutorialUIController : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public CanvasGroup goalCanvasGroup;

    [Header("Localize String Events")]
    [SerializeField] private LocalizeStringEvent m_tipLocalStrEvent;
    [SerializeField] private LocalizeStringEvent m_goalLocalStrEvent;

    public void Show() {
        canvasGroup.gameObject.GetComponent<Canvas>().enabled = true;
        goalCanvasGroup.gameObject.GetComponent<Canvas>().enabled = true;
    }

    public void Hide() {
        canvasGroup.gameObject.GetComponent<Canvas>().enabled = false;
        goalCanvasGroup.gameObject.GetComponent<Canvas>().enabled = false;
    }

    public void ShowText(LocalizedString stringReference) 
    {
        StartCoroutine(ShowTextCoroutine(stringReference));
    }

    public void ShowGoalText(LocalizedString stringReference) 
    {
        StartCoroutine(ShowGoalTextCoroutine(stringReference));
    }

    //받은 text를 UI로 띄우고 5초 후 사라지게 하는 코루틴
    private IEnumerator ShowTextCoroutine(LocalizedString stringReference) {
        // text 변경
        m_tipLocalStrEvent.StringReference = stringReference;

        //fade in -> 5초 대기 -> fade out
        yield return StartCoroutine(FadeInOut(canvasGroup, 0f, 1f));
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(FadeInOut(canvasGroup, 1f, 0f));
    }

    private IEnumerator ShowGoalTextCoroutine(LocalizedString stringReference) 
    {
        //fade out -> text 변경 -> fade in
        yield return StartCoroutine(FadeInOut(goalCanvasGroup, 1f, 0f));
        //text 변경
        m_goalLocalStrEvent.StringReference = stringReference;
        yield return StartCoroutine(FadeInOut(goalCanvasGroup, 0f, 1f));
    }

    private IEnumerator FadeInOut(CanvasGroup canvasGroup, float startAlpha, float endAlpha)
    {
        // Debug.Log("FadeInOut: " + startAlpha + " / " + endAlpha);
        float elapsedTime = 0f;
        float fadeDuration = 1f;

        // 시작 알파값 설정
        canvasGroup.alpha = startAlpha;

        // 지정된 시간 동안 알파값을 변경
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        // 최종 알파값 설정
        canvasGroup.alpha = endAlpha;
    }

}
