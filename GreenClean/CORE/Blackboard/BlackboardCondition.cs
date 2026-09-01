using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BlackboardConditionBase
{
    public abstract bool Evaluate(GameProgressBlackboard blackboard);
}

public static class BlackboardCondition
{
    [System.Serializable]
    public sealed class CheckBlackboardValue : BlackboardConditionBase
    {
        public BlackboardKey targetKey;
        public CompareType compareType;
        public BlackboardSerializedValue value = new();

        public override bool Evaluate(GameProgressBlackboard blackboard)
        {
            if (blackboard == null)
            {
                DebugConsole.LogWarning(
                    "[BlackboardCondition.CheckBlackboardValue] blackboard is null"
                );
                return false;
            }

            if (value == null)
            {
                value = new BlackboardSerializedValue();
            }

            if (blackboard.TryGet(targetKey, out int intValue))
            {
                return Compare(intValue, value.intValue);
            }

            if (blackboard.TryGet(targetKey, out float floatValue))
            {
                return Compare(floatValue, value.floatValue);
            }

            if (blackboard.TryGet(targetKey, out bool boolValue))
            {
                return Compare(boolValue, value.boolValue);
            }

            if (blackboard.TryGet(targetKey, out string stringValue))
            {
                return Compare(stringValue, value.stringValue);
            }

            DebugConsole.LogWarning(
                $"[BlackboardCondition.CheckBlackboardValue] blackboard value '{targetKey}' was not found."
            );
            return false;
        }

        private bool Compare(int left, int right)
        {
            return compareType switch
            {
                CompareType.EQUAL => left == right,
                CompareType.NOT_EQUAL => left != right,
                CompareType.GREATER => left > right,
                CompareType.GREATER_OR_EQUAL => left >= right,
                CompareType.LESS => left < right,
                CompareType.LESS_OR_EQUAL => left <= right,
                _ => false,
            };
        }

        private bool Compare(float left, float right)
        {
            return compareType switch
            {
                CompareType.EQUAL => Mathf.Approximately(left, right),
                CompareType.NOT_EQUAL => !Mathf.Approximately(left, right),
                CompareType.GREATER => left > right,
                CompareType.GREATER_OR_EQUAL => left >= right,
                CompareType.LESS => left < right,
                CompareType.LESS_OR_EQUAL => left <= right,
                _ => false,
            };
        }

        private bool Compare(bool left, bool right)
        {
            return compareType switch
            {
                CompareType.EQUAL => left == right,
                CompareType.NOT_EQUAL => left != right,
                _ => false,
            };
        }

        private bool Compare(string left, string right)
        {
            return compareType switch
            {
                CompareType.EQUAL => left == right,
                CompareType.NOT_EQUAL => left != right,
                CompareType.GREATER => string.CompareOrdinal(left, right) > 0,
                CompareType.GREATER_OR_EQUAL => string.CompareOrdinal(left, right) >= 0,
                CompareType.LESS => string.CompareOrdinal(left, right) < 0,
                CompareType.LESS_OR_EQUAL => string.CompareOrdinal(left, right) <= 0,
                _ => false,
            };
        }
    }

    [System.Serializable]
    public sealed class And : BlackboardConditionBase
    {
        [SerializeReference, SubclassSelector]
        public List<BlackboardConditionBase> conditions = new();

        public override bool Evaluate(GameProgressBlackboard blackboard)
        {
            foreach (var condition in conditions)
            {
                if (condition == null)
                {
                    DebugConsole.LogWarning($"[BlackboardCondition.And] condition is null");
                    continue;
                }

                if (!condition.Evaluate(blackboard))
                {
                    return false;
                }
            }

            return true;
        }
    }

    [System.Serializable]
    public sealed class Or : BlackboardConditionBase
    {
        [SerializeReference, SubclassSelector]
        public List<BlackboardConditionBase> conditions = new();

        public override bool Evaluate(GameProgressBlackboard blackboard)
        {
            foreach (var condition in conditions)
            {
                if (condition == null)
                {
                    DebugConsole.LogWarning($"[BlackboardCondition.Or] condition is null");
                    continue;
                }

                if (condition.Evaluate(blackboard))
                {
                    return true;
                }
            }

            return false;
        }
    }

    [System.Serializable]
    public sealed class Not : BlackboardConditionBase
    {
        [SerializeReference, SubclassSelector]
        public BlackboardConditionBase condition;

        public override bool Evaluate(GameProgressBlackboard blackboard)
        {
            if (condition == null)
            {
                DebugConsole.LogWarning($"[BlackboardCondition.Not] condition is null");
                return false;
            }

            return !condition.Evaluate(blackboard);
        }
    }

    [System.Serializable]
    public sealed class AlwaysTrue : BlackboardConditionBase
    {
        public override bool Evaluate(GameProgressBlackboard blackboard)
        {
            return true;
        }
    }

    [System.Serializable]
    public sealed class IsValueChanged : BlackboardConditionBase
    {
        public BlackboardKey targetKey;

        private bool hasPreviousValue;
        private object previousValue;

        public override bool Evaluate(GameProgressBlackboard blackboard)
        {
            if (blackboard == null)
            {
                DebugConsole.LogWarning("[BlackboardCondition.IsValueChanged] blackboard is null");
                return false;
            }

            if (!TryGetValue(blackboard, out object currentValue))
            {
                DebugConsole.LogWarning(
                    $"[BlackboardCondition.IsValueChanged] blackboard value '{targetKey}' was not found."
                );
                return false;
            }

            if (!hasPreviousValue)
            {
                previousValue = currentValue;
                hasPreviousValue = true;
                return false;
            }

            bool changed = IsChanged(previousValue, currentValue);
            previousValue = currentValue;
            return changed;
        }

        private bool TryGetValue(GameProgressBlackboard blackboard, out object value)
        {
            if (blackboard.TryGet(targetKey, out int intValue))
            {
                value = intValue;
                return true;
            }

            if (blackboard.TryGet(targetKey, out float floatValue))
            {
                value = floatValue;
                return true;
            }

            if (blackboard.TryGet(targetKey, out bool boolValue))
            {
                value = boolValue;
                return true;
            }

            if (blackboard.TryGet(targetKey, out string stringValue))
            {
                value = stringValue;
                return true;
            }

            value = null;
            return false;
        }

        private static bool IsChanged(object previous, object current)
        {
            if (previous is float previousFloat && current is float currentFloat)
            {
                return !Mathf.Approximately(previousFloat, currentFloat);
            }

            return !Equals(previous, current);
        }
    }
}
