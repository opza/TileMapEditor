using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Worlds
{
    public class World
    {
        int width;
        public int Width => width;

        int height;
        public int Height => height;

        Tile[,] tiles;

        public World(int width, int height)
        {
            this.width = width;
            this.height = height;

            tiles = CreateWorldTiles(width, height);
            SetWorldTilesNeighbor(tiles);
        }

        public void RemoveAll()
        {
            foreach (var tile in tiles)
            {
                tile.DestroyBlock();
            }
        }

        public Tile GetTile(Vector2 pos)
        {
            var x = (int)pos.x;
            var y = (int)pos.y;

            return GetTile(x, y);
        }

        public Tile GetTile(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return null;

            return tiles[x, y];
        }

        Tile[,] CreateWorldTiles(int width, int heigth)
        {
            var tiles = new Tile[width, heigth];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < heigth; y++)
                {
                    tiles[x, y] = new Tile(x, y);
                }
            }

            return tiles;
        }

        void SetWorldTilesNeighbor(Tile[,] tiles)
        {
            foreach (var tile in tiles)
            {
                var neighborTiles = new List<Tile>();
                for (int y = 1; y >= -1; y--)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        if (x == 0 && y == 0)
                            continue;

                        neighborTiles.Add(GetTile(tile.X + x, tile.Y + y));
                    }
                }
                tile.SetNeighborTiles(neighborTiles.ToArray());
            }
        }
    }

    
}
