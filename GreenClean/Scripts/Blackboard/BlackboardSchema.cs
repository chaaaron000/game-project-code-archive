using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_BlackboardSchema", menuName = "GreenClean/Blackboard Schema", order = 0)]
public sealed class BlackboardSchema : ScriptableObject
{
    [SerializeField] private List<BlackboardSchemaEntry> entries = new();

    public IReadOnlyList<BlackboardSchemaEntry> Entries => entries;

    public bool TryGetValueKind(BlackboardKey key, out BlackboardValueKind valueKind)
    {
        foreach (BlackboardSchemaEntry entry in entries)
        {
            if (entry.Key != key)
            {
                continue;
            }

            valueKind = entry.ValueKind;
            return true;
        }

        valueKind = default;
        return false;
    }
}
