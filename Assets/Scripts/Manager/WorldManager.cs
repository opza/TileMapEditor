using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Tilemaps;

using Util;

namespace Worlds
{
    public class WorldManager : MonoBehaviour
    {
        
        static World world;
        public static World World => world;

        // Use this for initialization
        void Awake()
        {
            if (world == null)
                world = new World(100, 100); 
        }

        // Update is called once per frame
        void Update()
        {
            // TEST : 블럭 생성
            if (Input.GetMouseButtonDown(0))
            {
                var mouseWorldPoint = MousePoint.GetWorldPoint();
                var tile = world.GetTile(mouseWorldPoint);

                tile?.BuildBlock(BlockManager.Instance.GetBlockInfo("Block"));
            }

            if (Input.GetMouseButton(1))
            {
                var mouseWorldPoint = MousePoint.GetWorldPoint();
                var tile = world.GetTile(mouseWorldPoint);

                tile?.DestroyBlock();
            }
        }

       
    }
}