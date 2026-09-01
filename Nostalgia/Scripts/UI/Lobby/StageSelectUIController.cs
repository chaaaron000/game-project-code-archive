using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;


public class StageSelectUIController : MonoBehaviour
{
    [Header("Data & Prefabs")]
    public List<ChapterData> chapterList;
    public RectTransform cardContainer;  // ChapterSlider
    public GameObject cardPrefab; // RectTransform 포함된 카드 프리팹

    [Header("UI Controls")]
    public Button startButton;
    public Button LeftButton;
    public Button RightButton;
    public Image background;

    public Canvas canvas;
    public CanvasGroup canvasGroup;

    private int clearLevel = 0;
    private int currentIndex = 0;
    private float slideDuration = 0.3f;
    private float cardSpacing = 350f; // 카드 간 가로 간격 (카드 너비)
    private float fadeDuration = 1.0f; //스테이지 선택 UI 페이드 시간

    private List<GameObject> cardObjects = new List<GameObject>();
    private List<ChapterCardUI> m_chapterCardUIs = new List<ChapterCardUI>();

    [SerializeField] private float focusScale = 1f;
    [SerializeField] private float sideScale = 0.8f;
    private bool isInit = false;

    public void Init(bool isMaster)
    {
        InitCards();
        isInit = true;
        // Debug.Log("StageSelectUIController Init: " + isMaster);

        StartCoroutine(RefreshBestClearTime());
        
        // server 쪽
        if (isMaster) 
        {
            startButton.gameObject.SetActive(true);
            LeftButton.gameObject.SetActive(true);
            RightButton.gameObject.SetActive(true);
            background.gameObject.SetActive(true);

            StartCoroutine(LoadSave());
            
            LeftButton.onClick.AddListener(() => Slide(-1));
            RightButton.onClick.AddListener(() => Slide(1));
            startButton.onClick.AddListener(OnStart);

            LeftButton.interactable = true;
            RightButton.interactable = true;
        }
        // client 쪽
        else 
        {
            //SaveManager.Instance.RequireStageImageRpc();
            background.gameObject.SetActive(true);
        }
    }

    // chapterList의 각 챕터에 대응되는 모든 카드 생성
    private void InitCards() {
        for (int i = 0; i < chapterList.Count; i++) {
            ChapterData data = chapterList[i];

            GameObject card = Instantiate(cardPrefab, cardContainer);
            
            ChapterCardUI cardUI = card.GetComponent<ChapterCardUI>();
            cardUI.Setup(data);

            // 위치 배치
            RectTransform rt = card.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(i * cardSpacing, cardContainer.anchoredPosition.y);
            
            cardObjects.Add(card);
            m_chapterCardUIs.Add(cardUI);
        }

        // 초기 컨테이너 위치 보정
        cardContainer.anchoredPosition = new Vector2(-currentIndex * cardSpacing, cardContainer.anchoredPosition.y);
    }

    public void ShowCanvas(bool isShow)
    {
        if (canvas == null)
        {
            Debug.LogError("Canvas is null");
            return;
        }
        
        canvas.enabled = isShow;
    }

    public IEnumerator FadeCanvas(bool isShow)
    {
        float startAlpha = canvasGroup.alpha;
        float endAlpha = isShow ? 1f : 0f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        // 완전히 사라졌을 경우 Canvas 비활성화
        if (!isShow)
        {
            canvas.enabled = false;
        }
    }

    private IEnumerator LoadSave()
    {
        yield return new WaitUntil(() =>
            SaveManager.Instance != null &&
            SaveManager.Instance.ClearCountsValid);
        
        List<int> clearCounts = SaveManager.Instance.StageClearCounts;
        
        clearLevel = 0;
        
        // 클리어한 스테이지 다음까지 언락
        foreach (int clearCount in clearCounts)
        {
            if (clearCount > 0)
            {
                ++clearLevel;
            }
            else
            {
                break;
            }
        }

        // 최대 레벨 조정
        clearLevel = Mathf.Min(clearLevel, chapterList.Count - 1);

        // int tmpLevel = (int)SaveManager.Instance.CheckClearLevel();
        // //최대 레벨 조정
        // clearLevel = tmpLevel >= 2 ? 2 : tmpLevel;

        //서버는 직접 UnlockStage를 호출
        StartCoroutine(UnlockStage(clearLevel));
        
        // 클라에게는 Rpc를 보내서 UnlockStage를 호출하게 함
        // Debug.Log("UnlockStageRpc Send Index: " + clearLevel);
        SaveManager.Instance.UnlockStageRpc(clearLevel);
        
        // 가장 최대 레벨에 맞춰 카드 크기 및 알파값 조정
        ApplyCardVisuals(clearLevel);
    }

    /// <summary>
    /// 스테이지 클리어 시간을 적용합니다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RefreshBestClearTime()
    {
        yield return new WaitUntil(() =>
            SaveManager.Instance != null && 
            SaveManager.Instance.ClearCountsValid &&
            SaveManager.Instance.BestClearTimesValid);
        
        List<int> clearCounts = SaveManager.Instance.StageClearCounts;
        List<int> bestClearTimes = SaveManager.Instance.StageBestClearTimes;

        for (int i = 0; i < m_chapterCardUIs.Count; i++)
        {
            // 스테이지를 클리어 했으면 시간 문자열로, 아니면 빈 문자열로
            string clearTime = (clearCounts[i] > 0)
                ? TimeSpan.FromMilliseconds(bestClearTimes[i])
                          .ToString(@"hh\:mm\:ss\.fff")
                : " - ";
            
            m_chapterCardUIs[i].SetBestClearTimeText(clearTime);
        }
    }

    public IEnumerator UnlockStage(int maxIndex)
    {
        yield return new WaitUntil(() => isInit);
        
        // maxIndex까지의 해당 카드의 Unlock() 메소드 호출
        for (int i = 0; i <= maxIndex; i++) 
        {
            m_chapterCardUIs[i].Unlock();
        }

        ApplyCardVisuals(maxIndex);
    }

    //index를 받아서 현재 카드 상황에 따라 카드 크기 및 알파값을 조정하는 함수
    private void ApplyCardVisuals(int index) {
        currentIndex = index;

        for (int i = 0; i < cardObjects.Count; i++) {
            float scale = (i == index) ? focusScale : sideScale;
            cardObjects[i].transform.localScale = Vector3.one * scale;

            CanvasGroup cg = cardObjects[i].GetComponent<CanvasGroup>();
            if (cg != null) {
                cg.alpha = (i == index) ? 1f : 0.5f;
            }
        }

        cardContainer.anchoredPosition = new Vector2(-index * cardSpacing, cardContainer.anchoredPosition.y);
    }

    public void Slide(int direction) {
        int nextIndex = currentIndex + direction;
        if (nextIndex < 0 || nextIndex >= cardObjects.Count) return;

        currentIndex = nextIndex;
        StopAllCoroutines();
        StartCoroutine(SlideAnimation());

        // TODO: 이거 재귀인데 클라 쪽에서 StateAuthority가 아니라서 어찌저찌 크래쉬는 안 나고 있음.
        //클라에 slide 이벤트 전달
        SaveManager.Instance.SetStageImageRpc(direction);
    }

    private IEnumerator SlideAnimation() {
        Vector2 startPos = cardContainer.anchoredPosition;
        Vector2 targetPos = new Vector2(-currentIndex * cardSpacing, startPos.y);
        float t = 0;

        // 현재 카드들의 초기 스케일 저장
        List<Vector3> startScales = new List<Vector3>();
        for (int i = 0; i < cardObjects.Count; i++) {
            startScales.Add(cardObjects[i].transform.localScale);
        }

        // 현재 카드들의 초기 알파값 저장
        List<float> startAlphas = new List<float>();
        for (int i = 0; i < cardObjects.Count; i++) {
            CanvasGroup cg = cardObjects[i].GetComponent<CanvasGroup>();
            startAlphas.Add(cg != null ? cg.alpha : 1f);
        }

        while (t < 1f) {
            t += Time.deltaTime / slideDuration;

            // 위치 이동
            cardContainer.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            // 스케일 변경
            for (int i = 0; i < cardObjects.Count; i++) {
                float target = (i == currentIndex) ? focusScale : sideScale;
                Vector3 targetScale = Vector3.one * target;
                cardObjects[i].transform.localScale = Vector3.Lerp(startScales[i], targetScale, t);

                CanvasGroup cg = cardObjects[i].GetComponent<CanvasGroup>();
                if (cg != null) {
                    float targetAlpha = (i == currentIndex) ? 1f : 0.5f;
                    cg.alpha = Mathf.Lerp(startAlphas[i], targetAlpha, t);
                }
            }

            yield return null;
        }

        // 보정 (Lerp에 의한 오차 제거)
        cardContainer.anchoredPosition = targetPos;
        
        // 슬라이드 끝났으니 정확한 시각 효과 재적용
        ApplyCardVisuals(currentIndex);   
    }

    public void SpreadSelectedStage(int direction) {
        SaveManager.Instance.SetStageImageRpc(direction);
    }

    public int GetSelectedStage() {
        return currentIndex + 1;
    }

    public void OnStart() 
    {
        if (currentIndex < 0 || currentIndex > clearLevel)
        {
            return;
        }
        //게임 시작

        LeftButton.interactable = false;
        RightButton.interactable = false;
        SaveManager.Instance.StartGameRpc();
    }
    
#if UNITY_EDITOR
    public void UnlockAllStage()
    {
        clearLevel = 1000;
        
        foreach (var card in m_chapterCardUIs)
        {
            card.Unlock();
        }
    }
#endif
}
