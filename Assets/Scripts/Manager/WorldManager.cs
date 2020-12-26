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


        // TODO : 나중에 Custom Insepctor 추가해야함
        [SerializeField]
        List<MapSet> level0Maps = new List<MapSet>();

        [SerializeField]
        List<MapSet> level1Maps = new List<MapSet>();

        [SerializeField]
        List<MapSet> level2Maps = new List<MapSet>();

        // Use this for initialization
        void Awake()
        {
            world = new World(100, 100);
            
        }

        void Start()
        {
            new MapGenerator(world, level0Maps, level1Maps, level2Maps).Generate(1);
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