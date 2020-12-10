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
    }
}
