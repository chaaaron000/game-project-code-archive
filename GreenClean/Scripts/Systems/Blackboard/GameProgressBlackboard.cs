using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SO_GameProgressBlackboard",
    menuName = "GreenClean/Game Progress Blackboard",
    order = 0
)]
public sealed class GameProgressBlackboard : ScriptableObject
{
    [SerializeField]
    private BlackboardDefaults defaults;

    private readonly Dictionary<BlackboardKey, BlackboardValue> values = new();

    public event Action ValueChanged;

    public void ResetBlackboard()
    {
        values.Clear();

        if (defaults == null)
        {
            DebugConsole.LogWarning("[GameProgressBlackboard] BlackboardDefaults is not assigned.");
            return;
        }

        foreach (BlackboardDefaultEntry entry in defaults.Entries)
        {
            if (!defaults.TryGetValue(entry.Key, out object value))
            {
                continue;
            }

            values[entry.Key] = new BlackboardValue(entry.Key, value);
        }
    }

    public T Get<T>(BlackboardKey key)
    {
        if (!values.TryGetValue(key, out BlackboardValue blackboardValue))
        {
            DebugConsole.LogWarning(
                $"[GameProgressBlackboard] Blackboard value '{key}' was not found."
            );
            return default;
        }

        if (!blackboardValue.TryGet(out T value))
        {
            DebugConsole.LogWarning(
                $"[GameProgressBlackboard] Blackboard value '{key}' is {FormatType(blackboardValue.ValueType)}, not {typeof(T).Name}."
            );
            return default;
        }

        return value;
    }

    public bool TryGet<T>(BlackboardKey key, out T value)
    {
        if (values.TryGetValue(key, out BlackboardValue blackboardValue))
        {
            return blackboardValue.TryGet(out value);
        }

        value = default;
        return false;
    }

    public bool TrySet<T>(BlackboardKey key, T value)
    {
        Type nextType = typeof(T);
        if (!ValidateSchemaType(key, nextType))
        {
            return false;
        }

        if (values.TryGetValue(key, out BlackboardValue blackboardValue))
        {
            if (!ValidateCurrentType(key, blackboardValue.ValueType, nextType))
            {
                return false;
            }

            blackboardValue.Set(value);
        }
        else
        {
            values[key] = new BlackboardValue(key, value);
        }

        NotifyValueChanged();
        return true;
    }

    private void NotifyValueChanged()
    {
        ValueChanged?.Invoke();
    }

    private bool ValidateSchemaType(BlackboardKey key, Type type)
    {
        BlackboardSchema schema = defaults != null ? defaults.Schema : null;
        if (schema == null || !schema.TryGetValueKind(key, out BlackboardValueKind expectedKind))
        {
            return true;
        }

        if (
            !BlackboardValueKindUtility.TryGetKind(type, out BlackboardValueKind actualKind)
            || actualKind != expectedKind
        )
        {
            Type expectedType = BlackboardValueKindUtility.GetSystemType(expectedKind);
            DebugConsole.LogWarning(
                $"Blackboard value '{key}' expects {expectedType.Name}, not {type.Name}."
            );
            return false;
        }

        return true;
    }

    private bool ValidateCurrentType(BlackboardKey key, Type currentType, Type nextType)
    {
        if (currentType == null || nextType == currentType)
        {
            return true;
        }

        DebugConsole.LogWarning(
            $"[GameProgressBlackboard] Blackboard value '{key}' is {FormatType(currentType)}, not {FormatType(nextType)}."
        );

        return false;
    }

    private static string FormatType(Type type)
    {
        return type != null ? type.Name : "null";
    }
}
