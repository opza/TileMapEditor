using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Unity.Collections;
using System.Collections;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using UnityEditor;


using Editor.Dungeon;

[Serializable]
public class Room : ScriptableObject, ISerializationCallbackReceiver
{
    public event Action updateEvent;

    Tile[,] tiles;

    [SerializeField]
    Serializable2dArray serializable2dTiles;


    public Tile this[int x, int y]=>GetTile(x,y);

    public int Width => tiles != null ? tiles.GetLength(0) : 0;
    public int Height => tiles != null ? tiles.GetLength(1) : 0;

    public void SetSize(int resizedWidth, int resizedHeight)
    {
        var resizedTiles = CreateEmptyTiles(resizedWidth, resizedHeight);
        if(tiles != null)
            PasteToAnotherTiles(resizedTiles, tiles);

        tiles = resizedTiles;
        updateEvent?.Invoke();
    }

    public void SetTile(Palette.Element element, int x, int y)
    {
        if (!Inside(x, y))
            return;

        tiles[x, y] = Tile.Create(element);
        updateEvent?.Invoke();
    }

    public Tile GetTile(int x, int y)
    {
        if (!Inside(x, y))
            return Tile.CreateEmpty();

        return tiles[x, y];
    }

    Tile[,] CreateEmptyTiles(int width, int height)
    {
        var emptyTiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                emptyTiles[x, y] = Tile.CreateEmpty();
            }
        }

        return emptyTiles;
    }

    void PasteToAnotherTiles(Tile[,] targetTiles, Tile[,] copiedTiles)
    {
        var targetTilesWidth = targetTiles.GetLength(0);
        var targetTilesHeight = targetTiles.GetLength(1);
        var copiedTilesWidth = copiedTiles.GetLength(0);
        var copiedTilesHeight = copiedTiles.GetLength(1);


        for (int x = 0; x < copiedTilesWidth; x++)
        {
            if (x >= targetTilesWidth)
                break;

            for (int y = 0; y < copiedTilesHeight; y++)
            {
                if (y >= targetTilesHeight)
                    break;

                targetTiles[x, y] = copiedTiles[x, y];
            }
        }
    }

    bool Inside(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return false;

        return true;
    }

    public void OnBeforeSerialize()
    {
        serializable2dTiles = new Serializable2dArray(tiles);
    }

    public void OnAfterDeserialize()
    {
        tiles = serializable2dTiles?.To2dArray() as Tile[,];
    }

    [Serializable]
    public class Tile
    {
        [SerializeField]
        Texture2D texture2D;
        public Texture2D Texture2D => texture2D;

        [SerializeField]
        string name;
        public string Name => name;


        public static Tile Create(Palette.Element paletteElement)
        {
            var tile = new Tile();
            tile.texture2D = paletteElement.Texture2D;
            tile.name = paletteElement.Name;     

            return tile;
        }

        public static Tile CreateEmpty()
        {
            return new Tile();
        }

        private Tile() { }
    }

    [Serializable]
    class Serializable2dArray
    {
        [SerializeField]
        [HideInInspector]
        Tile[] serializableArray;

        [SerializeField]
        [ReadOnly]
        int xSize;
        public int XSize => xSize;

        [SerializeField]
        [ReadOnly]
        int ySize;
        public int YSize => ySize;

        public int Length => xSize * ySize;

        public Tile this[int x, int y]
        {
            get => Get(x, y);
            set => Set(value, x, y);
        }

        public Serializable2dArray(Tile[,] original2dArray)
            : this(original2dArray?.GetLength(0) ?? 0, original2dArray?.GetLength(1) ?? 0)
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

            serializableArray = new Tile[xSize * ySize];
        }

        public void Set(Tile value, int x, int y)
        {
            if (!Inside(x, y))
                throw new IndexOutOfRangeException();

            var idx = GetOffsetIndex(x, y);
            serializableArray[idx] = value;
        }

        public Tile Get(int x, int y)
        {
            if (!Inside(x, y))
                throw new IndexOutOfRangeException();

            var idx = GetOffsetIndex(x, y);
            return serializableArray[idx];
        }

        public Tile[,] To2dArray()
        {
            var original2dArray = new Tile[xSize, ySize];
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
