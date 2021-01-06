using UnityEngine;
using UnityEngine.Tilemaps;

using System.Collections;
using System.Collections.Generic;

using Util;

namespace Worlds
{

    public class BlockManager : MonoBehaviour
    {
        static BlockManager instance;
        public static BlockManager Instance => instance;

        [SerializeField]
        Tilemap tileMap;
        Dictionary<Sprite, UnityEngine.Tilemaps.Tile> gridTiles = new Dictionary<Sprite, UnityEngine.Tilemaps.Tile>();

        Dictionary<string, BlockInfo> blockInfos;
        public BlockInfo GetBlockInfo(string name) => blockInfos?[name];

        Dictionary<Block, GameObject> linkedObjects = new Dictionary<Block, GameObject>();

        // Use this for initialization
        void Awake()
        {
            if (instance != null)
                return;

            instance = this;

            blockInfos = ResourceLoader.LoadBlockInfos("BlockInfo");

            Tile.createdBlockEvent += OnCreateBlock;
            Tile.destroyedBlockEvent += OnDestroyBlock;
            Tile.changedBlockSpriteEvent += OnChangeBlockSprite;
        }

       

        void OnCreateBlock(object sender, CreateBlcokObjectArgs e)
        {
            if (e.Block.BlockInfo.Light == null)
                return;

            
            var obj = Instantiate(e.Block.BlockInfo.Light, new Vector3(e.X,e.Y), Quaternion.identity);
            linkedObjects[e.Block] = obj;
        }

        void OnDestroyBlock(object sender, DestrotyBlockOjectArgs e)
        {
            if (!linkedObjects.ContainsKey(e.Block))
                return;

            Destroy(linkedObjects[e.Block]);
        }

        void OnChangeBlockSprite(object sender, ChangeBlockSpriteArgs e)
        {
            if (e.Sprite == null)
            {
                tileMap.SetTile(e.Position, null);
                return;
            }

            if (!gridTiles.ContainsKey(e.Sprite))
            {
                var gridTile = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
                gridTile.sprite = e.Sprite;

                gridTiles[e.Sprite] = gridTile;
            }

            tileMap.SetTile(e.Position, gridTiles[e.Sprite]);

        }
    }
}