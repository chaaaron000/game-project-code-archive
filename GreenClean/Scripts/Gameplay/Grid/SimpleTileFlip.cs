using UnityEngine;
using DG.Tweening; // DOTween 플러그인 추천

public class SimpleTileFlip : MonoBehaviour
{
    public SpriteRenderer baseRenderer;
    public SpriteRenderer circleMaskRenderer;

    private bool isGreen = true;

    void Start()
    {
        UpdateColors();
        circleMaskRenderer.transform.localScale = Vector3.zero;
    }

    public void FlipTile()
    {
        isGreen = !isGreen;

        // 1. 자식 원의 크기를 키워 타일을 덮음
        circleMaskRenderer.transform.localScale = Vector3.zero;
        circleMaskRenderer.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f) // 타일을 확실히 덮을 크기
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                // 2. 애니메이션 종료 후, 원형 마스크는 숨기고 베이스 색상을 변경
                circleMaskRenderer.transform.localScale = Vector3.zero;
                UpdateColors();
            });
    }

    private void UpdateColors()
    {
        if (isGreen)
        {
            baseRenderer.color = Color.green;
            circleMaskRenderer.color = Color.white; // 퍼지는 원은 흰색
        }
        else
        {
            baseRenderer.color = Color.white;
            circleMaskRenderer.color = Color.green; // 퍼지는 원은 초록색
        }
    }
}