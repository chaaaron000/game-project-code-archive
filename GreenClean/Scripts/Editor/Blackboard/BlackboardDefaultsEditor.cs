using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlackboardDefaults))]
public sealed class BlackboardDefaultsEditor : Editor
{
    private SerializedProperty schemaProperty;
    private SerializedProperty entriesProperty;

    private void OnEnable()
    {
        schemaProperty = serializedObject.FindProperty("schema");
        entriesProperty = serializedObject.FindProperty("entries");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(schemaProperty);

        BlackboardSchema schema = schemaProperty.objectReferenceValue as BlackboardSchema;
        if (schema == null)
        {
            EditorGUILayout.HelpBox(
                "Assign a BlackboardSchema before editing defaults.",
                MessageType.Warning
            );
            serializedObject.ApplyModifiedProperties();
            return;
        }

        DrawValidation(schema);
        EditorGUILayout.Space();
        DrawEntries(schema);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawValidation(BlackboardSchema schema)
    {
        List<BlackboardKey> missingKeys = GetMissingDefaultKeys(schema);
        List<BlackboardKey> duplicateKeys = GetDuplicateDefaultKeys();
        List<BlackboardKey> unknownKeys = GetUnknownDefaultKeys(schema);

        if (missingKeys.Count > 0)
        {
            EditorGUILayout.HelpBox(
                $"Missing default keys: {string.Join(", ", missingKeys)}",
                MessageType.Warning
            );

            if (GUILayout.Button("Add Missing Defaults", GUILayout.Height(24f)))
            {
                AddMissingDefaults(missingKeys);
            }
        }

        if (duplicateKeys.Count > 0)
        {
            EditorGUILayout.HelpBox(
                $"Duplicate default keys: {string.Join(", ", duplicateKeys)}",
                MessageType.Error
            );
        }

        if (unknownKeys.Count > 0)
        {
            EditorGUILayout.HelpBox(
                $"Defaults without schema: {string.Join(", ", unknownKeys)}",
                MessageType.Warning
            );
        }
    }

    private void DrawEntries(BlackboardSchema schema)
    {
        EditorGUILayout.LabelField("Defaults", EditorStyles.boldLabel);

        for (int i = 0; i < entriesProperty.arraySize; i++)
        {
            SerializedProperty entryProperty = entriesProperty.GetArrayElementAtIndex(i);
            SerializedProperty keyProperty = entryProperty.FindPropertyRelative("Key");
            BlackboardKey key = (BlackboardKey)
                Enum.Parse(
                    typeof(BlackboardKey),
                    keyProperty.enumNames[keyProperty.enumValueIndex]
                );

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(keyProperty);

                    if (GUILayout.Button("Remove", GUILayout.Width(70f)))
                    {
                        entriesProperty.DeleteArrayElementAtIndex(i);
                        break;
                    }
                }

                if (!schema.TryGetValueKind(key, out BlackboardValueKind valueKind))
                {
                    EditorGUILayout.HelpBox(
                        "This key is not defined in the assigned schema.",
                        MessageType.Warning
                    );
                    continue;
                }

                EditorGUILayout.LabelField("Type", valueKind.ToString());
                DrawValueField(entryProperty, valueKind);
            }
        }
    }

    private static void DrawValueField(
        SerializedProperty entryProperty,
        BlackboardValueKind valueKind
    )
    {
        SerializedProperty valueProperty = entryProperty.FindPropertyRelative("Value");
        BlackboardSerializedValueEditorGUI.Draw(valueProperty, valueKind);
    }

    private void AddMissingDefaults(List<BlackboardKey> missingKeys)
    {
        foreach (BlackboardKey key in missingKeys)
        {
            int index = entriesProperty.arraySize;
            entriesProperty.InsertArrayElementAtIndex(index);

            SerializedProperty entryProperty = entriesProperty.GetArrayElementAtIndex(index);
            SetEnum(entryProperty.FindPropertyRelative("Key"), key);

            SerializedProperty valueProperty = entryProperty.FindPropertyRelative("Value");
            BlackboardSerializedValueEditorGUI.Reset(valueProperty);
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    private List<BlackboardKey> GetMissingDefaultKeys(BlackboardSchema schema)
    {
        var result = new List<BlackboardKey>();
        var defaultKeys = GetDefaultKeySet();

        foreach (BlackboardSchemaEntry entry in schema.Entries)
        {
            if (!defaultKeys.Contains(entry.Key))
            {
                result.Add(entry.Key);
            }
        }

        return result;
    }

    private List<BlackboardKey> GetDuplicateDefaultKeys()
    {
        var result = new List<BlackboardKey>();
        var defaultKeys = new HashSet<BlackboardKey>();
        var duplicateKeys = new HashSet<BlackboardKey>();

        BlackboardDefaults defaultsAsset = (BlackboardDefaults)target;
        foreach (BlackboardDefaultEntry entry in defaultsAsset.Entries)
        {
            if (!defaultKeys.Add(entry.Key))
            {
                duplicateKeys.Add(entry.Key);
            }
        }

        result.AddRange(duplicateKeys);
        return result;
    }

    private List<BlackboardKey> GetUnknownDefaultKeys(BlackboardSchema schema)
    {
        var result = new List<BlackboardKey>();

        BlackboardDefaults defaultsAsset = (BlackboardDefaults)target;
        foreach (BlackboardDefaultEntry entry in defaultsAsset.Entries)
        {
            if (!schema.TryGetValueKind(entry.Key, out _))
            {
                result.Add(entry.Key);
            }
        }

        return result;
    }

    private HashSet<BlackboardKey> GetDefaultKeySet()
    {
        var result = new HashSet<BlackboardKey>();

        BlackboardDefaults defaultsAsset = (BlackboardDefaults)target;
        foreach (BlackboardDefaultEntry entry in defaultsAsset.Entries)
        {
            result.Add(entry.Key);
        }

        return result;
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
