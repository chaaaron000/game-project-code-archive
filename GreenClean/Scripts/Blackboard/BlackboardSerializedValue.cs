using System;

[Serializable]
public sealed class BlackboardSerializedValue
{
    public int intValue;
    public float floatValue;
    public bool boolValue;
    public string stringValue;

    public object GetValue(BlackboardValueKind valueKind)
    {
        return valueKind switch
        {
            BlackboardValueKind.INT => intValue,
            BlackboardValueKind.FLOAT => floatValue,
            BlackboardValueKind.BOOL => boolValue,
            BlackboardValueKind.STRING => stringValue,
            _ => null,
        };
    }
}
