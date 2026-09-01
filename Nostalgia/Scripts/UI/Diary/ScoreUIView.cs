using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class ScoreUIView : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private Canvas scoreCanvas;
    
    [Header("Prefab")]
    [SerializeField] private GameObject butterflyPrefab;
    
    [Header("Sprite")]
    [SerializeField] private Sprite butterflyEmptySprite;
    [SerializeField] private Sprite butterflyGetSprite;
    
    [Header("Material")]
    [SerializeField] private Material butterflyMT;
    
    [Header("Images")]
    [SerializeField] private Image[] butterflyImages;

    public void Initialize(int totalDiaryNum)
    {
        scoreCanvas.worldCamera = GameObject.Find("UICamera").GetComponent<Camera>();
        butterflyImages = new Image[totalDiaryNum];
        for (int i = 0; i < totalDiaryNum; i++) {
            GameObject butterflyObj = Instantiate(butterflyPrefab, scoreCanvas.transform);
            RectTransform rectTransform = butterflyObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1f, 1f);  // 오른쪽 위 앵커
            rectTransform.anchorMax = new Vector2(1f, 1f);  // 오른쪽 위 앵커
            rectTransform.pivot = new Vector2(1f, 1f);      // 피벗도 오른쪽 위로 설정
            rectTransform.anchoredPosition = new Vector3(-15.0f + (-25.0f * (totalDiaryNum - i - 1)), -20f, 0f);

            rectTransform.sizeDelta = new Vector2(40f, 40f); // 너비 40, 높이 40

            butterflyImages[i] = butterflyObj.GetComponent<Image>();
            butterflyImages[i].sprite = butterflyEmptySprite;
            butterflyImages[i].material = null;
        }
    }
    
    public void UpdateScore(int score) 
    {
        for (int i = 0; i < butterflyImages.Length; i++) 
        {
            if (i < score) 
            {
                butterflyImages[i].sprite = butterflyGetSprite;
                butterflyImages[i].material = butterflyMT;
            } 
            else 
            {
                butterflyImages[i].sprite = butterflyEmptySprite;
                butterflyImages[i].material = null;
            }
        }
    }
}
