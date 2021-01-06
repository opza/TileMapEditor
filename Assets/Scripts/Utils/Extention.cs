using System;
using System.Collections.Generic;
using System.Linq;

namespace Util
{
    public static class Extention
    {
        public static byte ToByte(this bool[] boolMask)
        {
            byte b = 0x00;
            byte mask = 0x01;

            for (int i = 0; i < boolMask.Length; i++)
            {
                if (boolMask[i])
                    b |= mask;
                mask <<= 1;
            }

            return b;
        }

        public static byte ToByte(this bool[,] boolMask)
        {
            byte b = 0;

            for (int i = 0; i < boolMask.GetLength(0); i++)
            {
                for (int j = 0; j < boolMask.GetLength(1); j++)
                {
                    if (boolMask[i, j])
                        b |= 1;
                    b <<= 1;
                }
            }

            return b;
        }

        public static IEnumerable<T> RandomSwap<T>(this IEnumerable<T> enumerable)
        {
            var swapArray = enumerable.ToArray();

            if (swapArray.Length <= 1)
                return swapArray;

            var random = new Random(Environment.TickCount);
            for (int i = swapArray.Length; i > 1; i--)
            {
                var ranIndex = random.Next(i);
                swapArray.Swap(i - 1, ranIndex);
            }

            return swapArray;
        }   

        public static void Swap<T>(this T[] swapArray, int idxA, int idxB)
        {
            var temp = swapArray[idxA];
            swapArray[idxA] = swapArray[idxB];
            swapArray[idxB] = temp;
        }

        public static T GetRandomItem<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable.Count() <= 0)
                return default(T);

            var random = new Random(Environment.TickCount);

            var ranIdx = random.Next(0, enumerable.Count());
            var currIdx = 0;

            var enumeraotr = enumerable.GetEnumerator();

            do
            {
                enumeraotr.MoveNext();              

            } while (ranIdx != currIdx++);

            return enumeraotr.Current;
        }
    }
}
