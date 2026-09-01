using Nostal.Steam;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SteamStatsAndAchievements))]
public class SteamStatsAndAchievementsButton : Editor
{
    public override void OnInspectorGUI()
    {
        SteamStatsAndAchievements steam = (SteamStatsAndAchievements)target;
        
        // 플레이 도중이 아닐 때 비활성화 그룹
        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        if (GUILayout.Button("스탯 및 도전과제 초기화"))
        {
            steam.ResetAllStats();
        }
        EditorGUI.EndDisabledGroup();
        // 그룹 끝

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("플레이 모드에서만 초기화할 수 있습니다.", MessageType.Info);
        }
        
        // 구분선
        EditorGUILayout.Space();
        
        base.OnInspectorGUI();
    }
}