using System.IO;
using UnityEditor;
using UnityEngine;

public sealed class JsonUtilEditorWindow : EditorWindow
{
    private const string MenuPath = "GreenClean/JsonUtilEditor";
    private const float SectionSpacing = 12f;

    private Vector2 scrollPosition;
    private string[] jsonFilePaths = new string[0];

    [MenuItem(MenuPath)]
    private static void Open()
    {
        JsonUtilEditorWindow window = GetWindow<JsonUtilEditorWindow>("Json 유틸 에디터");
        window.minSize = new Vector2(360f, 220f);
        window.Show();
    }

    private void OnEnable()
    {
        RefreshJsonFileList();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawHeader();
        DrawJsonFileSection();
        DrawJsonFileTable();
        DrawFutureFeatureSection();

        EditorGUILayout.EndScrollView();
    }

    private static void DrawHeader()
    {
        EditorGUILayout.LabelField("Json 유틸리티", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("GreenClean의 영구 저장 Json 데이터를 관리합니다.", EditorStyles.wordWrappedMiniLabel);
        EditorGUILayout.Space(SectionSpacing);
    }

    private void DrawJsonFileSection()
    {
        EditorGUILayout.LabelField("Json 파일", EditorStyles.boldLabel);

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.TextField("저장 경로", GetSaveDirectory());
        }

        EditorGUILayout.Space(4f);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("새로고침", GUILayout.Height(24f), GUILayout.Width(90f)))
            {
                RefreshJsonFileList();
            }

            if (GUILayout.Button("폴더 열기", GUILayout.Height(24f), GUILayout.Width(110f)))
            {
                OpenSaveDirectoryInFileExplorer();
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("전체 Json 삭제", GUILayout.Height(24f), GUILayout.Width(160f)))
            {
                DeleteAllJsonFilesWithConfirmation();
                RefreshJsonFileList();
            }
        }

        EditorGUILayout.Space(SectionSpacing);
    }

    private void DrawJsonFileTable()
    {
        EditorGUILayout.LabelField($"Json 파일 목록 ({jsonFilePaths.Length})", EditorStyles.boldLabel);

        DrawTableHeader();

        if (jsonFilePaths.Length == 0)
        {
            EditorGUILayout.HelpBox("Json 파일이 없습니다.", MessageType.Info);
            EditorGUILayout.Space(SectionSpacing);
            return;
        }

        foreach (string path in jsonFilePaths)
        {
            DrawTableRow(path);
        }

        EditorGUILayout.Space(SectionSpacing);
    }

    private static void DrawTableHeader()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            GUILayout.Label("파일명", EditorStyles.miniBoldLabel, GUILayout.MinWidth(140f));
            GUILayout.Label("크기", EditorStyles.miniBoldLabel, GUILayout.Width(70f));
            GUILayout.Label("수정일", EditorStyles.miniBoldLabel, GUILayout.Width(140f));
            GUILayout.Label("작업", EditorStyles.miniBoldLabel, GUILayout.Width(92f));
        }
    }

    private static void DrawTableRow(string path)
    {
        var fileInfo = new FileInfo(path);

        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.SelectableLabel(fileInfo.Name, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.MinWidth(140f));
            GUILayout.Label(FormatFileSize(fileInfo.Length), GUILayout.Width(70f));
            GUILayout.Label(fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm"), GUILayout.Width(140f));

            if (GUILayout.Button("열기", GUILayout.Width(44f)))
            {
                EditorUtility.RevealInFinder(path);
            }

            if (GUILayout.Button("복사", GUILayout.Width(44f)))
            {
                EditorGUIUtility.systemCopyBuffer = path;
            }
        }
    }

    private static void DrawFutureFeatureSection()
    {
        EditorGUILayout.LabelField("추가 예정 기능", EditorStyles.boldLabel);

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.TextField("예약 영역", "Json 조회/내보내기/가져오기 기능을 추가할 수 있습니다.");
        }
    }

    private void DeleteAllJsonFilesWithConfirmation()
    {
        string saveDirectory = GetSaveDirectory();
        bool exists = Directory.Exists(saveDirectory);
        string message = exists
            ? $"아래 경로의 모든 Json 파일을 삭제할까요?\n\n{saveDirectory}"
            : $"Json 저장 경로가 존재하지 않습니다.\n\n{saveDirectory}";

        if (!exists)
        {
            EditorUtility.DisplayDialog("전체 Json 삭제", message, "확인");
            return;
        }

        bool confirmed = EditorUtility.DisplayDialog(
            "전체 Json 삭제",
            message,
            "삭제",
            "취소");

        if (!confirmed)
        {
            return;
        }

        global::JsonUtility.DeleteAllJsonFiles();
        Debug.Log($"[JsonUtilEditor] Json 저장 경로를 삭제했습니다: {saveDirectory}");
    }

    private void RefreshJsonFileList()
    {
        string saveDirectory = GetSaveDirectory();
        jsonFilePaths = Directory.Exists(saveDirectory)
            ? Directory.GetFiles(saveDirectory, "*.json", SearchOption.TopDirectoryOnly)
            : new string[0];
    }

    private static void OpenSaveDirectoryInFileExplorer()
    {
        string saveDirectory = GetSaveDirectory();

        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        EditorUtility.RevealInFinder(saveDirectory);
    }

    private static string FormatFileSize(long bytes)
    {
        if (bytes < 1024)
        {
            return $"{bytes} B";
        }

        float kilobytes = bytes / 1024f;
        if (kilobytes < 1024f)
        {
            return $"{kilobytes:0.##} KB";
        }

        return $"{kilobytes / 1024f:0.##} MB";
    }

    private static string GetSaveDirectory()
    {
        return Path.Combine(Application.persistentDataPath, "json_data");
    }
}
