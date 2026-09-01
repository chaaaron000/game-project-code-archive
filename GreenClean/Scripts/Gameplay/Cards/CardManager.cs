using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // 새로운 인풋 시스템 사용

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    [SerializeField]
    private GridManager gridManager;

    public event Action CardUsed;

    private Card _selectedCard;
    private List<Tile> _previewTiles = new List<Tile>();

    //  인풋 시스템 변수
    private InputSystem_Actions _inputActions;

    void Awake()
    {
        Instance = this;

        //  인풋 시스템 세팅 및 이벤트 연결
        _inputActions = new InputSystem_Actions();

        // 1,2,3번 키를 누르면 해당 순서(0,1,2)의 카드를 선택합니다.
        _inputActions.Player.SelectCard1.performed += ctx => TrySelectCardByIndex(0);
        _inputActions.Player.SelectCard2.performed += ctx => TrySelectCardByIndex(1);
        _inputActions.Player.SelectCard3.performed += ctx => TrySelectCardByIndex(2);
        _inputActions.Player.HoldCard.performed += ctx => TryHoldSelectedCard();

        // 마우스 좌클릭 시 카드 효과 적용
        _inputActions.Player.ApplyCard.performed += ctx => ApplyCardEffect();
    }

    //  인풋 시스템 활성화/비활성화
    private void OnEnable() => _inputActions?.Enable();

    private void OnDisable() => _inputActions?.Disable();

    //  마우스나 키보드로 카드를 선택했을 때 실행
    public void SelectCard(Card card)
    {
        // 1. 이미 다른 카드가 켜져 있었다면 그 카드의 파티클 끄기
        if (_selectedCard != null)
        {
            _selectedCard.SetSelectedVisual(false);
        }

        // 2. 새로운 카드로 교체
        _selectedCard = card;

        // 3. 새로 선택된 카드의 파티클 켜기
        if (_selectedCard != null)
        {
            _selectedCard.SetSelectedVisual(true);
        }

        DebugConsole.Log($"[CardManager] 선택된 패: {card.GetCardName()}");
    }

    //  키보드 입력 시 덱에서 카드를 찾아오는 함수
    private void TrySelectCardByIndex(int index)
    {
        // DeckManager의 배열에서 정확한 위치의 카드를 가져옵니다.
        Card targetCard = DeckManager.Instance.GetCardAt(index);

        if (targetCard != null)
        {
            SelectCard(targetCard);
            DebugConsole.Log($"키보드 {index + 1}번 입력: {targetCard.name} 선택됨");
        }
    }

    void Update()
    {
        if (_selectedCard == null)
        {
            return;
        }

        UpdatePreview();
    }

    void UpdatePreview()
    {
        if (gridManager.IsAnimationPlaying)
        {
            return;
        }

        foreach (var tile in _previewTiles)
        {
            tile.SetHighlight(false);
        }

        _previewTiles.Clear();

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); //  인풋시스템용 마우스 위치 읽기
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null && hit.collider.TryGetComponent<Tile>(out Tile centerTile))
        {
            foreach (var offset in _selectedCard.GetPatternOffsets())
            {
                Tile target = gridManager.GetTile(centerTile.X + offset.x, centerTile.Y + offset.y);
                if (target != null)
                {
                    target.SetHighlight(true);
                    _previewTiles.Add(target);
                }
            }
        }
    }

    void ApplyCardEffect()
    {
        if (_previewTiles.Count == 0 || GameManager.Instance.IsPaused)
        {
            return;
        }

        CardUsed?.Invoke();

        SoundManager.Instance.PlaySFX("FlipSFX");

        // --- [핵심: 여기서부터 점수 계산을 합니다] ---
        int totalTiles = _previewTiles.Count;
        int freshPurify = 0;
        int duplicatePurify = 0;

        foreach (var tile in _previewTiles)
        {
            if (tile.IsPurified)
            {
                continue;
            }

            // 오염 -> 정화로 바뀌는 경우
            if (tile.HasBeenPurified)
            {
                duplicatePurify++;
            }
            else
            {
                freshPurify++;
            }
        }

        int effectivePurify = freshPurify;

        // 예외 조항: 과반수 이하 중복은 눈감아주기
        if (duplicatePurify <= totalTiles / 4)
        {
            effectivePurify += duplicatePurify;
        }

        float efficiency = (float)effectivePurify / totalTiles;

        if (effectivePurify == totalTiles)
        {
            GameManager.Instance.AddScore(5);
            GameManager.Instance.AddCombo();
        }
        else if (efficiency >= 0.5f)
        {
            GameManager.Instance.AddScore(3);
            GameManager.Instance.ResetCombo();
        }
        else if (efficiency >= 0.3f)
        {
            GameManager.Instance.AddScore(1);
            GameManager.Instance.ResetCombo();
        }
        else
        {
            GameManager.Instance.AddScore(0);
            GameManager.Instance.ResetCombo();
        }

        //  타일 상태 변경
        foreach (var tile in _previewTiles)
        {
            tile.ToggleState();
        }

        //  카드 사용 후 파티클 끄기
        if (_selectedCard != null)
        {
            _selectedCard.SetSelectedVisual(false);
        }
        //  사용한 패를 덱에서 교체
        DeckManager.Instance.ReplaceCard(_selectedCard);
        _selectedCard = null; //  선택 해제

        foreach (var tile in _previewTiles)
        {
            tile.SetHighlight(false);
        }

        _previewTiles.Clear();

        // 4. 클리어 확인
        gridManager.CheckBoardClear();
    }

    private void TryHoldSelectedCard()
    {
        if (_selectedCard == null)
            return;

        int selectedSlotIndex = -1;
        for (int i = 0; i < 3; i++)
        {
            if (DeckManager.Instance.GetCardAt(i) == _selectedCard)
            {
                selectedSlotIndex = i;
                break;
            }
        }

        if (selectedSlotIndex != -1)
        {
            // 1. 선택 비주얼(파티클) 끄기
            _selectedCard.SetSelectedVisual(false);

            // 🌟 [추가된 부분] 보관함으로 카드가 사라지기 전에, 보드에 떠 있던 미리보기 잔상도 전부 꺼줍니다!
            foreach (var tile in _previewTiles)
            {
                tile.SetHighlight(false);
            }
            _previewTiles.Clear();

            // 2. 덱매니저에게 스왑 요청
            DeckManager.Instance.SwapWithStoredCard(selectedSlotIndex);

            // 3. 스왑 후에는 선택이 해제되도록 처리
            _selectedCard = null;

            DebugConsole.Log("카드가 보관함과 교체되었습니다.");
        }
    }
}
