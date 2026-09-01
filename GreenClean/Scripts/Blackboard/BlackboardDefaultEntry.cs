using System;

[Serializable]
public sealed class BlackboardDefaultEntry
{
    public BlackboardKey Key;
    public BlackboardSerializedValue Value = new();

    public object GetValue(BlackboardValueKind valueKind)
    {
        return Value.GetValue(valueKind);
    }
}
