using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageSelectUIController))]
public class StageSelectUIUnlockButton : Editor
{
    public override void OnInspectorGUI()
    {
        StageSelectUIController ui = (StageSelectUIController)target;
        
        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        if (GUILayout.Button("스테이지 언락"))
        {
            ui.UnlockAllStage();
        }
        EditorGUI.EndDisabledGroup();
        
        base.OnInspectorGUI();
    }
}
