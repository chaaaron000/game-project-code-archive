using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_BlackboardDefaults", menuName = "GreenClean/Blackboard Defaults", order = 1)]
public sealed class BlackboardDefaults : ScriptableObject
{
    [SerializeField] private BlackboardSchema schema;
    [SerializeField] private List<BlackboardDefaultEntry> entries = new();

    public BlackboardSchema Schema => schema;
    public IReadOnlyList<BlackboardDefaultEntry> Entries => entries;

    public bool TryGetValue(BlackboardKey key, out object value)
    {
        foreach (BlackboardDefaultEntry entry in entries)
        {
            if (entry.Key != key)
            {
                continue;
            }

            if (schema != null && schema.TryGetValueKind(key, out BlackboardValueKind valueKind))
            {
                value = entry.GetValue(valueKind);
                return true;
            }

            value = null;
            return false;
        }

        value = null;
        return false;
    }
}
