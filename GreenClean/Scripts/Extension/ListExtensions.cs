using System;
using System.Collections.Generic;

public static class ListExtensions
{
    public static T GetRandom<T>(this IReadOnlyList<T> list)
    {
        if (list == null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        if (list.Count == 0)
        {
            throw new InvalidOperationException("Cannot select a random item from an empty list.");
        }

        return list[UnityEngine.Random.Range(0, list.Count)];
    }
}
