using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public static class ListExtensions
    {
        public static T RandomPick<T>(this List<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }

        public static T RandomPickRemove<T>(this List<T> list)
        {
            var picked = list.RandomPick();
            list.Remove(picked);
            return picked;
        }

        public static void Remove<T>(this List<T> list, IEnumerable<T> toRemove)
        {
            foreach (var t in toRemove)
            {
                list.Remove(t);
            }
        }

        public static bool Empty<T>(this List<T> list)
        {
            return list.Count == 0;
        }
    }
}