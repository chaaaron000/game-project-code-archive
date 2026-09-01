using System;

[Serializable]
public sealed class BlackboardSchemaEntry
{
    public BlackboardKey Key;
    public BlackboardValueKind ValueKind;
    public string Description;
}
