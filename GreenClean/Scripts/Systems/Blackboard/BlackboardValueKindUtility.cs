using System;

public static class BlackboardValueKindUtility
{
    public static bool TryGetKind(Type type, out BlackboardValueKind kind)
    {
        if (type == typeof(int))
        {
            kind = BlackboardValueKind.INT;
            return true;
        }

        if (type == typeof(float))
        {
            kind = BlackboardValueKind.FLOAT;
            return true;
        }

        if (type == typeof(bool))
        {
            kind = BlackboardValueKind.BOOL;
            return true;
        }

        if (type == typeof(string))
        {
            kind = BlackboardValueKind.STRING;
            return true;
        }

        kind = default;
        return false;
    }

    public static Type GetSystemType(BlackboardValueKind kind)
    {
        return kind switch
        {
            BlackboardValueKind.INT => typeof(int),
            BlackboardValueKind.FLOAT => typeof(float),
            BlackboardValueKind.BOOL => typeof(bool),
            BlackboardValueKind.STRING => typeof(string),
            _ => typeof(object),
        };
    }
}
