using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.AI;

public class LobbyScriptSample : MonoBehaviour
{
    public CanvasGroup buttonCanvasGroup; // CanvasGroup을 연결
    public CanvasGroup mainUICanvasGroup;
    public CanvasGroup LobbyUICanvasGroup;
    //public GameObject LobbyUICanvasGroup_p;

    public float fadeDuration = 1.0f; // 페이드 지속 시간

    public Material material; // 셰이더가 적용된 머티리얼을 참조
    //public string propertyName = "_MyFloat"; // 제어할 프로퍼티 이름


    public CinemachineVirtualCamera camera1;
    public CinemachineVirtualCamera camera2;

    private void Start()
    {
        //LobbyUICanvasGroup = GameObject.Find("LobyUI Camera").transform.GetChild(0).gameObject.GetComponent<CanvasGroup>();

        StartCoroutine(titleEfffect(1f, 0.01f, 2f));
 
        //StartCoroutine(FadeInOut(buttonCanvasGroup, 0f, 1f));
    }
    
    private IEnumerator FadeInOut(CanvasGroup canvasGroup, float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;

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

    private IEnumerator titleEfffect(float startValue, float endValue, float duration) // 1에서 0으로 변화하는 데 걸리는 시간) //애니메이션으로 할라 했는데 파라미터를 못찾겠어서 그냥 스크립트로 구현
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration); 
            material.SetFloat("_AlphaThreshold", newValue); // Shader의 프로퍼티 값 변경
            yield return null;
        }

        // 최종적으로 0으로 설정
        //material.SetFloat("_AlphaThreshold", endValue);
    }

    private IEnumerator FadeOutAndIn(CanvasGroup fadeOutUI, CanvasGroup fadeInUI)
    {
        // 페이드 아웃 UI 알파값을 1에서 0으로 줄임
        yield return StartCoroutine(FadeInOut(fadeOutUI, 1f, 0f));
        
        // 페이드 아웃이 끝난 후 UI를 비활성화
        fadeOutUI.interactable = false;
        fadeOutUI.blocksRaycasts = false;

        // 페이드 인할 UI를 활성화
        fadeInUI.interactable = true;
        fadeInUI.blocksRaycasts = true;

        // 페이드 인 UI 알파값을 0에서 1로 증가시킴
        yield return StartCoroutine(FadeInOut(fadeInUI, 0f, 1f));
    }


    public void SwitchToCamera2()
    {
        // camera1을 비활성화하고, camera2를 활성화
        camera2.enabled = true;
        camera1.enabled = false;

        StartCoroutine(titleEfffect(0f, 1f, 1f));
        StartCoroutine(FadeOutAndIn(mainUICanvasGroup, LobbyUICanvasGroup));
    }
}
