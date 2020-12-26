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

        public static IList<T> RandomSwap<T>(this IList<T> list)
        {
            return list.ToArray().RandomSwap().ToList();
        }

        public static T[] RandomSwap<T>(this T[] array)
        {         

            var swapArray = array.Clone() as T[];

            if (array.Length <= 1)
                return swapArray;
            
            var random = new Random(Environment.TickCount);

            for (int i = array.Length - 1; i > 0 ; i--)
            {
                var ranValue = random.Next(i);
                swapArray.Swap(ranValue, i);
            }

            return swapArray;

        }

        public static void Swap<T>(this T[] swapArray, int idxA, int idxB)
        {
            var temp = swapArray[idxA];
            swapArray[idxB] = swapArray[idxA];
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
