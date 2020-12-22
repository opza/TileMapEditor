using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using Unity.Collections;

namespace Util.SerializableObjects
{
    [Serializable]
    public class Serializable2dArray<T> where T : class
    {
        [SerializeField]
        [HideInInspector]
        T[] serializableArray;

        [SerializeField]
        [HideInInspector]
        int xSize;
        public int XSize => xSize;

        [SerializeField]
        [HideInInspector]
        int ySize;
        public int YSize => ySize;

        public int Length => xSize * ySize;

        public T this[int x, int y]
        {
            get => GetTile(x, y);
            set => SetTile(value, x, y);
        }

        public Serializable2dArray(T[,] original2dArray)
            : this(original2dArray?.GetLength(0) ?? 0, original2dArray?.GetLength(1) ?? 0)
        {
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    SetTile(original2dArray[x, y], x, y);
                }
            }
        }

        public Serializable2dArray(int xSize, int ySize)
        {
            this.xSize = xSize;
            this.ySize = ySize;

            serializableArray = new T[xSize * ySize];
        }

        public void SetTile(T value, int x, int y)
        {
            if (!Inside(x, y))
                throw new IndexOutOfRangeException();

            var idx = GetOffsetIndex(x, y);
            serializableArray[idx] = value;
        }

        public T GetTile(int x, int y)
        {
            if (!Inside(x, y))
                throw new IndexOutOfRangeException();

            var idx = GetOffsetIndex(x, y);
            return serializableArray[idx];
        }

        public T[,] To2dArray()
        {
            var original2dArray = new T[xSize, ySize];
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    original2dArray[x, y] = GetTile(x, y);
                }
            }

            return original2dArray;
        }

        int GetOffsetIndex(int x, int y)
        {
            return xSize * x + y;
        }

        bool Inside(int x, int y)
        {
            if (x < 0 || x >= xSize || y < 0 || y >= ySize)
                return false;

            return true;
        }
    }
}
