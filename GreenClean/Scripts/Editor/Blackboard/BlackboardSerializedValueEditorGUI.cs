using UnityEditor;
using UnityEngine;

public static class BlackboardSerializedValueEditorGUI
{
    public static void Draw(
        SerializedProperty valueProperty,
        BlackboardValueKind valueKind,
        GUIContent label = null
    )
    {
        string propertyName = GetValuePropertyName(valueKind);
        if (string.IsNullOrEmpty(propertyName))
        {
            return;
        }

        EditorGUILayout.PropertyField(
            valueProperty.FindPropertyRelative(propertyName),
            label ?? new GUIContent("Value")
        );
    }

    public static void Reset(SerializedProperty valueProperty)
    {
        valueProperty.FindPropertyRelative("intValue").intValue = 0;
        valueProperty.FindPropertyRelative("floatValue").floatValue = 0f;
        valueProperty.FindPropertyRelative("boolValue").boolValue = false;
        valueProperty.FindPropertyRelative("stringValue").stringValue = string.Empty;
    }

    private static string GetValuePropertyName(BlackboardValueKind valueKind)
    {
        return valueKind switch
        {
            BlackboardValueKind.INT => "intValue",
            BlackboardValueKind.FLOAT => "floatValue",
            BlackboardValueKind.BOOL => "boolValue",
            BlackboardValueKind.STRING => "stringValue",
            _ => string.Empty,
        };
    }
}
