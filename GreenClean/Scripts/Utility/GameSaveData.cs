using System;

[System.Serializable]
public class GameSaveData
{
    // --- 플레이 기록 ---
    public int highestScore;         // 최고 점수
    public int totalClearedBoards;   // 총 누적 정화 구역 수

    // --- 환경 설정 ---
    public int resolutionIndex;      // 해상도 옵션 (0, 1, 2)
    public float masterVolume;
    public float bgmVolume;
    public float sfxVolume;
}