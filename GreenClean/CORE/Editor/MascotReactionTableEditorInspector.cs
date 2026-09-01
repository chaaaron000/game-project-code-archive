using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MascotReactionTableEditor))]
public class MascotReactionTableEditorInspector : Editor
{
    private const string DefaultBlackboardSchemaPath =
        "Assets/Resources/GameData/Blackboard/SO_BlackboardSchema.asset";
    private const string LoadButtonText = "Json 로드";
    private const string SaveButtonText = "Json으로 저장하기";
    private const string CancelText = "취소";
    private static readonly GUIContent ConditionLabel = new("Condition");
    private static readonly GUIContent TargetKeyLabel = new("Target Key");

    private SerializedProperty mascotReactionTableProperty;
    private BlackboardSchema blackboardSchema;
    private BlackboardSchema cachedBlackboardSchema;
    private BlackboardKey[] cachedSchemaKeys = Array.Empty<BlackboardKey>();
    private GUIContent[] cachedSchemaLabels = Array.Empty<GUIContent>();
    private readonly Dictionary<BlackboardKey, BlackboardValueKind> cachedValueKinds = new();

    private void OnEnable()
    {
        mascotReactionTableProperty = serializedObject.FindProperty("mascotReactionTable");
        LoadDefaultSchemaIfNeeded();
        RebuildSchemaCacheIfNeeded();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawSchemaField();
        EditorGUILayout.Space();
        DrawMascotReactionTable();
        EditorGUILayout.Space();
        DrawJsonButtons();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSchemaField()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Schema", GUILayout.Width(70f));
            BlackboardSchema nextSchema = (BlackboardSchema)
                EditorGUILayout.ObjectField(blackboardSchema, typeof(BlackboardSchema), false);

            if (GUILayout.Button("Default", GUILayout.Width(70f)))
            {
                nextSchema = AssetDatabase.LoadAssetAtPath<BlackboardSchema>(
                    DefaultBlackboardSchemaPath
                );
            }

            if (nextSchema != blackboardSchema)
            {
                blackboardSchema = nextSchema;
                RebuildSchemaCache();
            }
        }
    }

    private void DrawMascotReactionTable()
    {
        MascotReactionTableEditor tableEditor = (MascotReactionTableEditor)target;
        if (tableEditor.mascotReactionTable == null)
        {
            EditorGUILayout.HelpBox("Mascot reaction table is null.", MessageType.Warning);

            if (GUILayout.Button("Create Mascot Reaction Table"))
            {
                Undo.RecordObject(tableEditor, "Create Mascot Reaction Table");
                tableEditor.mascotReactionTable = new MascotReactionTable();
                EditorUtility.SetDirty(tableEditor);
                serializedObject.Update();
            }

            return;
        }

        SerializedProperty versionProperty = mascotReactionTableProperty.FindPropertyRelative(
            "version"
        );
        SerializedProperty reactionsProperty = mascotReactionTableProperty.FindPropertyRelative(
            "reactions"
        );

        EditorGUILayout.PropertyField(versionProperty);

        EditorGUILayout.Space();
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Reactions", EditorStyles.boldLabel);

            if (GUILayout.Button("Expand All", GUILayout.Width(80f)))
            {
                SetAllReactionsExpanded(reactionsProperty, true);
            }

            if (GUILayout.Button("Collapse All", GUILayout.Width(90f)))
            {
                SetAllReactionsExpanded(reactionsProperty, false);
            }
        }

        for (int i = 0; i < reactionsProperty.arraySize; i++)
        {
            SerializedProperty reactionProperty = reactionsProperty.GetArrayElementAtIndex(i);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    reactionProperty.isExpanded = EditorGUILayout.Foldout(
                        reactionProperty.isExpanded,
                        $"Reaction {i + 1}",
                        true,
                        EditorStyles.foldout
                    );

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Remove", GUILayout.Width(70f)))
                    {
                        reactionsProperty.DeleteArrayElementAtIndex(i);
                        break;
                    }
                }

                if (reactionProperty.isExpanded)
                {
                    DrawReaction(reactionProperty);
                }
            }
        }

        if (GUILayout.Button("Add Reaction"))
        {
            int index = reactionsProperty.arraySize;
            reactionsProperty.InsertArrayElementAtIndex(index);
            SerializedProperty reactionProperty = reactionsProperty.GetArrayElementAtIndex(index);
            InitializeReaction(reactionProperty);
            reactionProperty.isExpanded = true;
        }
    }

    private void DrawReaction(SerializedProperty reactionProperty)
    {
        DrawCondition(reactionProperty.FindPropertyRelative("condition"));
        EditorGUILayout.PropertyField(reactionProperty.FindPropertyRelative("messages"), true);
        EditorGUILayout.PropertyField(reactionProperty.FindPropertyRelative("priority"));
        EditorGUILayout.PropertyField(reactionProperty.FindPropertyRelative("cooldown"));
        EditorGUILayout.PropertyField(reactionProperty.FindPropertyRelative("oncePerStage"));
    }

    private void DrawCondition(SerializedProperty conditionProperty)
    {
        bool isCheckBlackboardValue =
            conditionProperty.managedReferenceValue
            is BlackboardCondition.CheckBlackboardValue;

        if (isCheckBlackboardValue)
        {
            conditionProperty.isExpanded = false;
        }

        EditorGUILayout.PropertyField(
            conditionProperty,
            ConditionLabel,
            !isCheckBlackboardValue
        );

        if (!isCheckBlackboardValue)
        {
            return;
        }

        EditorGUI.indentLevel++;

        SerializedProperty targetKeyProperty = conditionProperty.FindPropertyRelative("targetKey");
        SerializedProperty compareTypeProperty = conditionProperty.FindPropertyRelative(
            "compareType"
        );
        SerializedProperty valueProperty = conditionProperty.FindPropertyRelative("value");

        DrawSchemaKeyPopup(targetKeyProperty);
        EditorGUILayout.PropertyField(compareTypeProperty);

        BlackboardKey targetKey = GetEnumValue<BlackboardKey>(targetKeyProperty);
        if (TryGetSchemaValueKind(targetKey, out BlackboardValueKind valueKind))
        {
            EditorGUILayout.LabelField("Type", valueKind.ToString());
            BlackboardSerializedValueEditorGUI.Draw(valueProperty, valueKind);
        }
        else
        {
            EditorGUILayout.HelpBox(
                $"'{targetKey}' is not defined in the assigned BlackboardSchema.",
                MessageType.Warning
            );
            EditorGUILayout.PropertyField(valueProperty, true);
        }

        EditorGUI.indentLevel--;
    }

    private void DrawSchemaKeyPopup(SerializedProperty keyProperty)
    {
        RebuildSchemaCacheIfNeeded();

        if (cachedSchemaKeys.Length == 0)
        {
            EditorGUILayout.PropertyField(keyProperty);
            return;
        }

        BlackboardKey currentKey = GetEnumValue<BlackboardKey>(keyProperty);
        int selectedIndex = -1;

        for (int i = 0; i < cachedSchemaKeys.Length; i++)
        {
            if (cachedSchemaKeys[i] == currentKey)
            {
                selectedIndex = i;
                break;
            }
        }

        if (selectedIndex < 0)
        {
            EditorGUILayout.PropertyField(keyProperty);
            return;
        }

        int nextIndex = EditorGUILayout.Popup(
            TargetKeyLabel,
            selectedIndex,
            cachedSchemaLabels
        );
        SetEnum(keyProperty, cachedSchemaKeys[nextIndex]);
    }

    private void RebuildSchemaCacheIfNeeded()
    {
        if (cachedBlackboardSchema == blackboardSchema)
        {
            return;
        }

        RebuildSchemaCache();
    }

    private void RebuildSchemaCache()
    {
        cachedBlackboardSchema = blackboardSchema;
        cachedValueKinds.Clear();

        if (blackboardSchema == null)
        {
            cachedSchemaKeys = Array.Empty<BlackboardKey>();
            cachedSchemaLabels = Array.Empty<GUIContent>();
            return;
        }

        var registeredKeys = new HashSet<BlackboardKey>();
        var keys = new List<BlackboardKey>();
        var labels = new List<GUIContent>();

        foreach (BlackboardSchemaEntry entry in blackboardSchema.Entries)
        {
            if (entry == null || !registeredKeys.Add(entry.Key))
            {
                continue;
            }

            keys.Add(entry.Key);
            labels.Add(new GUIContent($"{entry.Key} ({entry.ValueKind})", entry.Description));
            cachedValueKinds[entry.Key] = entry.ValueKind;
        }

        cachedSchemaKeys = keys.ToArray();
        cachedSchemaLabels = labels.ToArray();
    }

    private bool TryGetSchemaValueKind(BlackboardKey key, out BlackboardValueKind valueKind)
    {
        RebuildSchemaCacheIfNeeded();

        if (cachedValueKinds.TryGetValue(key, out valueKind))
        {
            return true;
        }

        valueKind = default;
        return false;
    }

    private static void SetAllReactionsExpanded(SerializedProperty reactionsProperty, bool isExpanded)
    {
        for (int i = 0; i < reactionsProperty.arraySize; i++)
        {
            reactionsProperty.GetArrayElementAtIndex(i).isExpanded = isExpanded;
        }
    }

    private void InitializeReaction(SerializedProperty reactionProperty)
    {
        reactionProperty.FindPropertyRelative("condition").managedReferenceValue = null;

        SerializedProperty messagesProperty = reactionProperty.FindPropertyRelative("messages");
        messagesProperty.arraySize = 0;

        reactionProperty.FindPropertyRelative("priority").intValue = 0;
        reactionProperty.FindPropertyRelative("cooldown").floatValue = 0f;
        reactionProperty.FindPropertyRelative("oncePerStage").boolValue = false;
    }

    private void DrawJsonButtons()
    {
        MascotReactionTableEditor tableEditor = (MascotReactionTableEditor)target;

        if (GUILayout.Button(LoadButtonText))
        {
            bool shouldLoad = EditorUtility.DisplayDialog(
                LoadButtonText,
                "작업하고 있는 내용이 덮어씌워질 수 있습니다.\nJson에서 데이터를 불러오시겠습니까?",
                "불러오기",
                CancelText
            );

            if (!shouldLoad)
            {
                return;
            }

            Undo.RecordObject(tableEditor, "Load Mascot Reaction Table From Json");
            tableEditor.LoadFromJson();
            EditorUtility.SetDirty(tableEditor);
            serializedObject.Update();
        }

        if (GUILayout.Button(SaveButtonText))
        {
            bool shouldSave = EditorUtility.DisplayDialog(
                SaveButtonText,
                "Json을 덮어씌우시겠습니까?",
                "저장",
                CancelText
            );

            if (!shouldSave)
            {
                return;
            }

            tableEditor.SaveToJson();
        }
    }

    private void LoadDefaultSchemaIfNeeded()
    {
        if (blackboardSchema != null)
        {
            return;
        }

        blackboardSchema = AssetDatabase.LoadAssetAtPath<BlackboardSchema>(
            DefaultBlackboardSchemaPath
        );
    }

    private static T GetEnumValue<T>(SerializedProperty property)
        where T : Enum
    {
        return (T)Enum.Parse(typeof(T), property.enumNames[property.enumValueIndex]);
    }

    private static void SetEnum<T>(SerializedProperty property, T value)
        where T : Enum
    {
        string[] names = property.enumNames;
        string valueName = value.ToString();

        for (int i = 0; i < names.Length; i++)
        {
            if (names[i] == valueName)
            {
                property.enumValueIndex = i;
                return;
            }
        }
    }
}
