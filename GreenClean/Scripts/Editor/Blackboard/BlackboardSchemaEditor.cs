using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlackboardSchema))]
public sealed class BlackboardSchemaEditor : Editor
{
    private SerializedProperty entriesProperty;

    private void OnEnable()
    {
        entriesProperty = serializedObject.FindProperty("entries");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawValidation();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(entriesProperty, true);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawValidation()
    {
        BlackboardSchema schema = (BlackboardSchema)target;
        List<BlackboardKey> missingKeys = GetMissingKeys(schema);
        List<BlackboardKey> duplicateKeys = GetDuplicateKeys(schema);

        if (missingKeys.Count > 0)
        {
            EditorGUILayout.HelpBox(
                $"Missing schema keys: {string.Join(", ", missingKeys)}",
                MessageType.Warning
            );

            if (GUILayout.Button("Add Missing Keys", GUILayout.Height(24f)))
            {
                AddMissingKeys(missingKeys);
            }
        }

        if (duplicateKeys.Count > 0)
        {
            EditorGUILayout.HelpBox(
                $"Duplicate schema keys: {string.Join(", ", duplicateKeys)}",
                MessageType.Error
            );
        }
    }

    private void AddMissingKeys(List<BlackboardKey> missingKeys)
    {
        foreach (BlackboardKey key in missingKeys)
        {
            int index = entriesProperty.arraySize;
            entriesProperty.InsertArrayElementAtIndex(index);

            SerializedProperty entryProperty = entriesProperty.GetArrayElementAtIndex(index);
            SetEnum(entryProperty.FindPropertyRelative("Key"), key);
            SetEnum(entryProperty.FindPropertyRelative("ValueKind"), BlackboardValueKind.INT);
            entryProperty.FindPropertyRelative("Description").stringValue = string.Empty;
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    private static List<BlackboardKey> GetMissingKeys(BlackboardSchema schema)
    {
        var result = new List<BlackboardKey>();
        var registeredKeys = new HashSet<BlackboardKey>();

        foreach (BlackboardSchemaEntry entry in schema.Entries)
        {
            registeredKeys.Add(entry.Key);
        }

        foreach (BlackboardKey key in Enum.GetValues(typeof(BlackboardKey)))
        {
            if (!registeredKeys.Contains(key))
            {
                result.Add(key);
            }
        }

        return result;
    }

    private static List<BlackboardKey> GetDuplicateKeys(BlackboardSchema schema)
    {
        var result = new List<BlackboardKey>();
        var registeredKeys = new HashSet<BlackboardKey>();
        var duplicateKeys = new HashSet<BlackboardKey>();

        foreach (BlackboardSchemaEntry entry in schema.Entries)
        {
            if (!registeredKeys.Add(entry.Key))
            {
                duplicateKeys.Add(entry.Key);
            }
        }

        result.AddRange(duplicateKeys);
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
