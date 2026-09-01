using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// [데이터] 카드의 고유 정보(이름, 패턴 좌표, 아이콘 등)를 저장합니다.
/// 에셋 형태로 프로젝트 폴더에 생성하여 관리합니다.
/// </summary>
[CreateAssetMenu(fileName = "NewCardData", menuName = "GreenClean/Card Data")]
public class CardData : ScriptableObject
{
    [Header("카드 기본 정보")]
    public string cardName;          // 카드 이름 (예: 십자형 패)
    public Sprite cardIcon;          // 나중에 UI에 띄울 카드 이미지용 (지금은 비워두셔도 됩니다)

    [Header("분사 패턴 좌표")]
    public List<Vector2Int> offsets; // 타일에 적용될 상대 좌표 목록
}