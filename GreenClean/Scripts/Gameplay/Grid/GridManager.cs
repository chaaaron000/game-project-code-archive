using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// [역할] 타일 그리드 생성, 타일 좌표 관리 및 클리어 조건 체크
/// </summary>
public class GridManager : MonoBehaviour
{
    [Header("그리드 설정")]
    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private int width = 6;

    [SerializeField]
    private int height = 6;

    [SerializeField]
    private float spacing = 1.1f;

    public bool IsAnimationPlaying { get; private set; }

    private Vector2 centerPosition =>
        new Vector2(
            transform.position.x - (spacing / 2) + (float)width / 2 * spacing,
            transform.position.y - (spacing / 2) + (float)height / 2 * spacing
        );

    public Tile[,] _tileGrid;

    void Start() => GenerateGrid();

    /// <summary>
    /// 지정된 크기로 타일을 배치하고 초기화합니다.
    /// </summary>
    void GenerateGrid()
    {
        _tileGrid = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 spawnPos = transform.position + new Vector3(x * spacing, y * spacing, 0);
                GameObject newTileObj = Instantiate(
                    tilePrefab,
                    spawnPos,
                    Quaternion.identity,
                    transform
                );

                Tile tileScript = newTileObj.GetComponent<Tile>();
                tileScript.Setup(x, y, centerPosition);
                tileScript.SetState(Random.value > 0.5f, false); // 초기 오염 상태 랜덤
                _tileGrid[x, y] = tileScript;
                tileScript.Hide(true);
            }
        }

        foreach (var tile in _tileGrid)
        {
            tile.Show();
        }
    }

    /// <summary>
    /// 특정 좌표의 타일을 반환합니다. (범위 밖이면 null)
    /// </summary>
    public Tile GetTile(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return _tileGrid[x, y];
        }

        return null;
    }

    /// <summary>
    /// 모든 타일이 정화되었는지 확인하고, 클리어 시 즉시 리셋합니다.
    /// </summary>
    public void CheckBoardClear()
    {
        foreach (var tile in _tileGrid)
        {
            // 오염된 타일이 단 하나라도 있다면 클리어 실패
            if (!tile.IsPurified)
            {
                return;
            }
        }

        // 모든 타일이 정화되었다면 보너스 지급 후 새 구역 로드
        GameManager.Instance.AddBoardClearScore();
        ResetGrid();
    }

    /// <summary>
    /// [타임어택] 새 판을 위해 타일 상태를 다시 랜덤하게 섞습니다.
    /// </summary>
    private void ResetGrid()
    {
        DebugConsole.Log("새 구역으로 이동 중... 오염 발생!");
        StartCoroutine(ResetGridCoroutine());
    }

    private IEnumerator ResetGridCoroutine()
    {
        IsAnimationPlaying = true;

        var waitTileAnimation = new WaitUntil(() =>
            _tileGrid.Cast<Tile>().All(tile => !tile.IsFlipAnimatePlaying)
        );

        foreach (var tile in _tileGrid)
        {
            tile.Hide();
        }

        yield return waitTileAnimation;

        foreach (var tile in _tileGrid)
        {
            tile.SetState(Random.value > 0.5f, false);
            tile.ResetHistory(); // 새로운 구역이므로 정화 꼼수 이력도 깨끗하게 리셋
            tile.Show();
        }

        yield return waitTileAnimation;

        IsAnimationPlaying = false;
    }

#if UNITY_EDITOR
    public void Debug_SetAllPurified()
    {
        foreach (var tile in _tileGrid)
        {
            tile.SetState(true);
        }

        CheckBoardClear();
    }
#endif
}
