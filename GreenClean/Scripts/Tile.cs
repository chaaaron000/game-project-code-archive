using UnityEngine;

/// <summary>
/// [역할] 단일 타일의 상태(오염/정화) 관리 및 시각적 표현
/// </summary>
[RequireComponent(typeof(TileView))]
public class Tile : MonoBehaviour
{
    public bool IsFlipAnimatePlaying { get; private set; }

    public bool IsPurified { get; set; }

    // 꼼수 방지용: 이 타일이 한 번이라도 정화된 적이 있는지 기록
    public bool HasBeenPurified { get; private set; }

    public int X { get; private set; }

    public int Y { get; private set; }

    private TileView view;
    private Vector2 centerPosition;
    private float distanceFromCenter;

    /// <summary>
    /// 타일 좌표 부여: GridManager가 처음 타일을 맵에 깔 때 (x, y) 좌표를 알려주며 세팅합니다.
    /// </summary>
    public void Setup(int x, int y, Vector2 centerPos)
    {
        view = GetComponent<TileView>();

        X = x;
        Y = y;
        HasBeenPurified = false;
        centerPosition = centerPos;
        Vector2 currentPos = transform.position;
        distanceFromCenter = Vector2.Distance(currentPos, centerPos);
    }

    /// <summary>
    /// 오염 상태를 설정합니다
    /// </summary>
    /// <param name="isPurify">설정할 오염 상태</param>
    public void SetState(bool isPurify, bool playAnimation = true)
    {
        IsPurified = isPurify;
        view.PlayChangeAnimation(isPurify, !playAnimation);
        if (isPurify)
        {
            HasBeenPurified = true;
        }
    }

    /// <summary>
    /// 오염 상태를 토글합니다.
    /// </summary>
    public void ToggleState()
    {
        IsPurified = !IsPurified;
        view.PlayChangeAnimation(IsPurified);
        if (IsPurified)
        {
            HasBeenPurified = true;
        }
    }

    /// <summary>
    /// 타일 하이라이트(미리보기): 마우스가 보드 위에 올라왔을 때, 패가 적용될 범위에 파란색 반투명 효과를 씌웁니다.
    /// </summary>
    public void SetHighlight(bool isHighlight)
    {
        view.SetHighlightColor(isHighlight);
    }

    public void Show(bool immediately = false)
    {
        IsFlipAnimatePlaying = true;
        view.PlayFlipAnimation(
            true,
            distanceFromCenter,
            () =>
            {
                IsFlipAnimatePlaying = false;
            },
            immediately
        );
    }

    public void Hide(bool immediately = false)
    {
        IsFlipAnimatePlaying = true;
        view.PlayFlipAnimation(
            false,
            distanceFromCenter,
            () =>
            {
                IsFlipAnimatePlaying = false;
            },
            immediately
        );
    }

    public void ResetHistory()
    {
        HasBeenPurified = IsPurified;
    }
}
