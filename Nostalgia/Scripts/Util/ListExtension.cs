using System.Collections.Generic;

namespace Nostal.Util
{
    public static class ListExtension
    {
        /// <summary>
        /// 리스트를 Knuth Shuffle 알고리즘을 사용하여 제자리에서 섞습니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">섞을 List</param>
        public static void KnuthShuffle<T>(this IList<T> list)
        {
            int n = list.Count;

            for (int i = n - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);

                T tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }
    }
}
