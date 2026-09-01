using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 이 Class는 여러 스크립트에서 중복 사용할 것 같은 메소드를 정의해둡니다.
   원하는 스크립트에 using Nostal.Util; 을 선언하고
   Utility.메소드명() 으로 사용할 수 있습니다. */

namespace Nostal.Util
{
    public class Utility
    {
        /// <summary>
        /// min 부터 max 까지의 값으로 겹치지 않는 length길이의 랜덤 배열 생성 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int[] GetRandomIntArray(int length, int min, int max)
        {
            int[] result = new int[length];

            List<int> list = new List<int>();
            for (int i = min; i <= max; ++i)
            {
                list.Add(i);
            }

            for (int i = 0; i < length; ++i)
            {
                int a = UnityEngine.Random.Range(0, max-min+1-i);

                result[i] = list[a];
                list.RemoveAt(a);
            }

            return result;
        }

        /// <summary>
        /// 배열을 무작위로 섞고 Queue로 반환합니다.
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Queue<T> ArrayToShuffledQueue<T>(T[] array)
        {
            T[] result = (T[])array.Clone();

            // Fisher-Yates 셔플
            System.Random rng = new System.Random();
            int n = result.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                (result[n], result[k]) = (result[k], result[n]);
            }

            return new Queue<T>(result);
        }
    }
}

