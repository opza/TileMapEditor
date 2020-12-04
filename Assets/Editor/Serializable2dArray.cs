using System;

using UnityEngine;
using UnityEditor;
using Unity.Collections;

[Serializable]
public class Serializable2dArray
{
    [SerializeField]
    [HideInInspector]
    Room.Tile[] serializableArray;

    [SerializeField]
    [ReadOnly]
    int xSize;
    public int XSize => xSize;

    [SerializeField]
    [ReadOnly]
    int ySize;
    public int YSize => ySize;

    public int Length => xSize * ySize;

    public Room.Tile this[int x, int y]
    {
        get => Get(x, y);
        set => Set(value, x, y);
    }

    public Serializable2dArray(Room.Tile[,] original2dArray) 
        : this(original2dArray.GetLength(0), original2dArray.GetLength(1))
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                Set(original2dArray[x, y], x, y);
            }
        }
    }

    public Serializable2dArray(int xSize, int ySize)
    {
        this.xSize = xSize;
        this.ySize = ySize;

        serializableArray = new Room.Tile[xSize * ySize];
    }

    public void Set(Room.Tile value, int x, int y)
    {
        if (!Inside(x,y))
            throw new IndexOutOfRangeException();

        var idx = GetOffsetIndex(x, y);
        serializableArray[idx] = value;
    }

    public Room.Tile Get(int x, int y)
    {
        if(!Inside(x,y))
            throw new IndexOutOfRangeException();

        var idx = GetOffsetIndex(x, y);
        return serializableArray[idx];
    }

    public Room.Tile[,] To2dArray()
    {
        var original2dArray = new Room.Tile[xSize, ySize];
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                original2dArray[x, y] = Get(x, y);
            }
        }

        return original2dArray;
    }

    int GetOffsetIndex(int x, int y)
    {
        return (x + 1) * (y + 1) - 1; ;
    }

    bool Inside(int x, int y)
    {
        if (x < 0 || x >= xSize || y < 0 || y >= ySize)
            return false;

        return true;
    }
}