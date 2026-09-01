
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private TextMeshPro textMesh;
    private Color textColor;

    [Header("애니메이션 설정")]
    public float moveSpeed = 1.3f;   // 위로 올라가는 속도
    public float fadeSpeed = 1.0f;   // 투명해지는 속도
    public float destroyTime = 1.0f; // 삭제되기까지 걸리는 시간

    public void Setup(int score, bool isBoardClear)
    {
        textMesh = GetComponent<TextMeshPro>();
        textMesh.text = "+" + score.ToString();
        if (isBoardClear)
        {
            // 🌟 판 전체 클리어 시 연출 (중앙 고정용)
            textMesh.fontSize = 15; // 일반 점수보다 훨씬 크게
            textMesh.color = Color.cyan; // 청록색 등으로 차별화
            textMesh.fontStyle = FontStyles.Bold;
            textMesh.fontMaterial.SetFloat("_FaceDilate", 0.3f); // 아주 두껍게

            // 중앙 고정 효과를 위해 위로 올라가는 속도를 줄이거나 없앨 수 있습니다.
            moveSpeed = 0.5f;
            destroyTime = 2.0f; // 더 오래 머물게 함
        }
        else
        {
            transform.position += new Vector3(0.23f, 0.3f, 0f); //콤보 뜨는 마우스 위치
            // 점수에 따라 폰트 크기와 색상을 다르게 설정합니다!
            if (score >= 20) // 10콤보 (20점)
            {
                textMesh.fontSize = 8;
                textMesh.color = Color.magenta; // 자홍색 (아주 튀게!)
                textMesh.fontStyle = FontStyles.Bold;       // 스타일: 굵게
                textMesh.fontMaterial.SetFloat("_FaceDilate", 0.5f);
            }
            else if (score >= 5) // 5콤보, 7콤보 (5점, 7점)
            {
                textMesh.fontSize = 7;
                textMesh.color = Color.red; // 주황색 (RGB 값으로 직접 만들기)
                textMesh.fontStyle = FontStyles.Bold;       // 스타일: 굵게
                                                            // [핵심] 글자 자체의 살을 찌워서 엄청 두껍게 만듭니다(기본값은 0)
                textMesh.fontMaterial.SetFloat("_FaceDilate", 0.4f);
            }
            else if (score >= 3) // 3콤보 (3점)
            {
                textMesh.fontSize = 6;
                textMesh.color = Color.purple; // 
                textMesh.fontStyle = FontStyles.Bold;       // 스타일: 굵게
                                                            // [핵심] 1점보다는 두껍고 5점보다는 얇게
                textMesh.fontMaterial.SetFloat("_FaceDilate", 0.3f);
            }
            else // 기본 (1점 등)
            {
                textMesh.fontSize = 5;
                textMesh.color = Color.blue;
                textMesh.fontStyle = FontStyles.Bold;
                textMesh.fontMaterial.SetFloat("_FaceDilate", 0.2f);
            }
        }
        // 셋팅된 색상을 textColor에 저장해두어야 아래 Update에서 투명도를 조절할 수 있습니다.
        textColor = textMesh.color;

        // 생성된 지 'destroyTime' 초가 지나면 자동으로 스스로를 삭제합니다.
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        if (textMesh == null) return;

        // 1. 위로 올라가기
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // 2. 서서히 투명해지기
        textColor.a -= fadeSpeed * Time.deltaTime;
        textMesh.color = textColor;
    }
}