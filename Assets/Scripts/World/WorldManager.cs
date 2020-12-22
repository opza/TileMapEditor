using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Tilemaps;

using Util;

namespace Worlds
{
    public class WorldManager : MonoBehaviour
    {
        
        World world;

        // Use this for initialization
        void Start()
        {
            world = new World(100, 100);
            new MapGenerator(world, "MapSets").Generate("MapSet");
        }

        // Update is called once per frame
        void Update()
        {
            // TEST : 블럭 생성
            if (Input.GetMouseButtonDown(0))
            {
                var mouseWorldPoint = MousePoint.GetWorldPoint();
                var tile = world.GetTile(mouseWorldPoint);

                tile?.OnCreatedBlock(BlockManager.Instance.GetBlockInfo("Block"));
            }
        }

       
    }
}