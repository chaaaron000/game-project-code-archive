using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugQAController))]
public sealed class DebugQAControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug Actions", EditorStyles.boldLabel);

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Debug actions are available in Play Mode.", MessageType.Info);
            return;
        }

        if (!DebugQAController.Debug_TryGetCurrentSceneType(out SceneType sceneType))
        {
            EditorGUILayout.HelpBox("Current scene is not registered in SceneType.", MessageType.Warning);
            return;
        }

        var controller = (DebugQAController)target;
        switch (sceneType)
        {
            case SceneType.GAME:
                DrawGameDebugActions(controller);
                break;

            case SceneType.INTRO:
            case SceneType.TITLE:
            case SceneType.SETTINGS:
                EditorGUILayout.HelpBox($"No debug actions for {sceneType}.", MessageType.Info);
                break;

            default:
                EditorGUILayout.HelpBox($"Unsupported scene type: {sceneType}.", MessageType.Warning);
                break;
        }
    }

    private static void DrawGameDebugActions(DebugQAController controller)
    {
        if (GUILayout.Button("모든 타일 정화", GUILayout.Height(24f)))
        {
            controller.Debug_SetAllTilesPurified();
        }

        if (GUILayout.Button("게임 오버", GUILayout.Height(24f)))
        {
            controller.Debug_TriggerGameOver();
        }
    }
}
