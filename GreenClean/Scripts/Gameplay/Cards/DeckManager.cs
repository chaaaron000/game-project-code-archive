using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI Image 컴포넌트를 제어하기 위해 추가!

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }

    [Header("데이터 풀 (Scriptable Objects)")]
    [SerializeField] private List<CardData> cardDataPool;
    [SerializeField] private Card[] _currentHand;

    [Header("UI 생성 설정")]
    [SerializeField] private Card baseCardPrefab;

    [Header("슬롯 설정")]
    [SerializeField] private Transform[] cardSlots;

    // 🌟 새로 추가된 '다음 카드' 미리보기 관련 변수들
    [Header("넥스트 카드 미리보기")]
    [SerializeField, Tooltip("다음 카드의 아이콘을 띄워줄 UI Image")]
    private Image nextCardPreviewImage;
    private CardData _nextCardData; // 다음번에 나올 카드 데이터를 미리 저장해두는 주머니

    [Header("보관함 설정")]
    [SerializeField] private Image storedCardPreviewImage;
    private CardData _storedCardData = null;

    void Awake()
    {
        Instance = this;
        _currentHand = new Card[cardSlots.Length];
    }

    void Start() => DrawCards();

    public void DrawCards()
    {
        ClearHand();

        // 1. 게임 시작 시, 일단 3개의 슬롯을 랜덤으로 꽉 채웁니다.
        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (cardDataPool.Count == 0) return;
            CardData randomData = cardDataPool[Random.Range(0, cardDataPool.Count)];
            CreateCardInSlot(randomData, i);
        }

        // 2. 패를 다 채웠으면, '다음 카드'를 미리 뽑아서 화면에 보여줍니다.
        PrepareNextCard();
    }

    /// <summary>
    /// 특정 슬롯에 지정된 데이터를 기반으로 카드를 생성하는 공통 함수
    /// </summary>
    private void CreateCardInSlot(CardData data, int index)
    {
        Transform targetSlot = cardSlots[index];
        Card newCard = Instantiate(baseCardPrefab, targetSlot);
        newCard.Setup(data);
        _currentHand[index] = newCard;
    }

    /// <summary>
    /// 다음 카드를 미리 랜덤으로 뽑아두고 UI 이미지를 업데이트합니다.
    /// </summary>
    private void PrepareNextCard()
    {
        if (cardDataPool.Count == 0) return;

        // 주머니에 다음 카드 데이터 저장
        _nextCardData = cardDataPool[Random.Range(0, cardDataPool.Count)];

        // UI에 다음 카드 이미지 띄우기
        if (nextCardPreviewImage != null && _nextCardData.cardIcon != null)
        {
            nextCardPreviewImage.sprite = _nextCardData.cardIcon;
            nextCardPreviewImage.enabled = true;
        }
    }

    public void ReplaceCard(Card usedCard)
    {
        for (int i = 0; i < _currentHand.Length; i++)
        {
            if (_currentHand[i] == usedCard)
            {
                // 기존 카드 파괴 및 자리 비우기
                Destroy(usedCard.gameObject);
                _currentHand[i] = null;

                // 1. 아까 미리 준비해둔 '다음 카드(_nextCardData)'를 빈 자리에 쏙 넣습니다!
                CreateCardInSlot(_nextCardData, i);

                // 2. 방금 '다음 카드'를 써버렸으니, 넥스트 슬롯을 위한 새 카드를 다시 뽑습니다!
                PrepareNextCard();
                break;
            }
        }
    }

    public void ClearHand()
    {
        for (int i = 0; i < _currentHand.Length; i++)
        {
            if (_currentHand[i] != null)
            {
                Destroy(_currentHand[i].gameObject);
                _currentHand[i] = null;
            }
        }
    }

    public Card GetCardAt(int index)
    {
        if (index >= 0 && index < _currentHand.Length)
        {
            return _currentHand[index];
        }
        return null;
    }

    public void SwapWithStoredCard(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _currentHand.Length) return;

        Card currentCard = _currentHand[slotIndex];
        if (currentCard == null) return;

        // 1. 현재 핸드에 있는 카드의 데이터를 임시 저장
        CardData cardInHandData = currentCard.GetCardData(); // Card.cs에 데이터를 가져오는 public 함수가 필요합니다.

        // 2. 현재 핸드의 카드 오브젝트 파괴
        Destroy(currentCard.gameObject);
        _currentHand[slotIndex] = null;

        if (_storedCardData == null)
        {
            // 보관함이 비어있었다면: 보관하고 새로운 카드 뽑기 (NextCard 로직 활용)
            _storedCardData = cardInHandData;

            // 지난번에 만든 '넥스트 카드' 로직을 사용하여 빈 자리를 채웁니다.
            CreateCardInSlot(_nextCardData, slotIndex);
            PrepareNextCard();
        }
        else
        {
            // 보관함에 카드가 있었다면: 서로 교체
            CardData tempStored = _storedCardData;
            _storedCardData = cardInHandData;

            // 보관되어 있던 카드를 핸드로 가져오기
            CreateCardInSlot(tempStored, slotIndex);
        }

        // 3. 보관함 UI 업데이트
        UpdateStoredCardUI();
    }

    private void UpdateStoredCardUI()
    {
        if (storedCardPreviewImage == null) return;

        if (_storedCardData != null)
        {
            storedCardPreviewImage.sprite = _storedCardData.cardIcon;
            storedCardPreviewImage.enabled = true;
        }
        else
        {
            storedCardPreviewImage.enabled = false;
        }
    }
}