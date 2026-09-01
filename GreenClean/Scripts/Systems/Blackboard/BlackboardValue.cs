using System;

public class BlackboardValue
{
    public readonly BlackboardKey Key;

    public Type ValueType { get; }

    public object RawValue => valueObj;

    private object valueObj;

    public BlackboardValue(BlackboardKey key, object valueObj)
    {
        Key = key;
        ValueType = valueObj?.GetType();
        this.valueObj = valueObj;
    }

    public T Get<T>()
    {
        if (valueObj is T typedValue)
        {
            return typedValue;
        }

        throw new InvalidCastException(
            $"Blackboard value '{Key}' is {FormatType(ValueType)}, not {typeof(T).Name}."
        );
    }

    public bool TryGet<T>(out T value)
    {
        if (valueObj is T typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default;
        return false;
    }

    public void Set<T>(T value)
    {
        Type nextType = typeof(T);
        if (ValueType != null && nextType != ValueType)
        {
            throw new InvalidCastException(
                $"Blackboard value '{Key}' is {FormatType(ValueType)}, not {FormatType(nextType)}."
            );
        }

        valueObj = value;
    }

    private static string FormatType(Type type)
    {
        return type != null ? type.Name : "null";
    }
}
