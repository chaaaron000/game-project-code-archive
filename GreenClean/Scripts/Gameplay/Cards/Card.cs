using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Card : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private Image cardImage;
    [SerializeField, Tooltip("클릭을 감지할 버튼 컴포넌트")]
    private Button cardButton;

    // 🌟 파티클 이펙트 연결용 변수 추가
    [Header("이펙트 연결")]
    [SerializeField, Tooltip("선택되었을 때 켜질 파티클(이펙트)")]
    private GameObject selectionEffect;

    private CardData _cardData;

    public void Setup(CardData data)
    {
        _cardData = data;

        if (cardImage != null && data.cardIcon != null)
        {
            cardImage.sprite = data.cardIcon;
        }

        if (cardButton != null)
        {
            cardButton.onClick.RemoveAllListeners();
            cardButton.onClick.AddListener(OnCardClicked);
        }

        //  처음 카드가 뽑혔을 때는 파티클 무조건 꺼두기
        SetSelectedVisual(false);
    }

    private void OnCardClicked()
    {
        CardManager.Instance.SelectCard(this);
    }

    //  외부에서 파티클을 껐다 켰다 할 수 있게 해주는 함수
    public void SetSelectedVisual(bool isSelected)
    {
        if (selectionEffect != null)
        {
            selectionEffect.SetActive(isSelected);
        }
    }

    public List<Vector2Int> GetPatternOffsets()
    {
        if (_cardData == null) return new List<Vector2Int>();
        return _cardData.offsets;
    }

    public string GetCardName()
    {
        if (_cardData == null) return "Unknown";
        return _cardData.cardName;
    }
    public CardData GetCardData() { return _cardData; }
}